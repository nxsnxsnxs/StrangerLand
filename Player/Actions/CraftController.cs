using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Actions;

namespace Player.Actions
{
    public class CraftController : BaseAction
    {
        private InventoryController inventoryController;
        private ViewController viewController;
        private ActionController actionController;
        private Coroutine current;
        //coroutine trigger

        public override string actionName
        {
            get => "Craft";
        }

        void Awake()
        {
            inventoryController = GetComponent<InventoryController>();
            viewController = GetComponent<ViewController>();
            actionController = GetComponent<ActionController>();
            enabled = false;

            RegisterTrigger("pickup");
            RegisterTrigger("pickfinish");
            RegisterTrigger("workfinish");
        }
        /// <summary>
        /// 自动寻找符合条件的最近目标，并选择进行对应的action
        /// </summary>
        public override void Begin(params object[] target)
        {
            if(target.Length == 0) AutoDoCraft();
            else if(target[0] is Workable) current = StartCoroutine(DoWork(target[0] as Workable));
            else if(target[0] is Pickupable) current = StartCoroutine(DoPickup(target[0] as Pickupable));
            else if(target[0] is Harvestable) current = StartCoroutine(DoHarvest(target[0] as Harvestable));
        }
        //选择最近的一个可执行目标
        void AutoDoCraft()
        {
            Pickupable pickupTarget = transform.position.FindClosestTargetInRange<Pickupable>(Constants.try_craft_distance);
            Harvestable harvestTarget = transform.position.FindClosestTargetInRange<Harvestable>(Constants.try_craft_distance);
            Workable workTarget = null;
            if(inventoryController.handEquipment != null && inventoryController.handEquipment.GetComponent<Tool>() != null)
            {
                workTarget = transform.position.FindClosestTargetInRange<Workable>(Constants.try_craft_distance, 
                    (w) => { return inventoryController.handEquipment.GetComponent<Tool>().toolTypes.Contains(w.toolType); });
            }
            if(!workTarget && !harvestTarget && !workTarget) return;

            float pickupDis = pickupTarget ? transform.position.PlanerDistance(pickupTarget.transform.position) : float.MaxValue;
            float workDis = workTarget ? workTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position).PlanerDistance(transform.position) : float.MaxValue;
            float harvestDis = harvestTarget ? harvestTarget.GetComponent<Collider>().ClosestPointOnBounds(transform.position).PlanerDistance(transform.position) : float.MaxValue;
            
            if(harvestDis <= pickupDis && harvestDis <= workDis) current = StartCoroutine(DoHarvest(harvestTarget));
            else if(workDis <= harvestDis && workDis <= pickupDis) current = StartCoroutine(DoWork(workTarget));
            else current = StartCoroutine(DoPickup(pickupTarget));
        }
        IEnumerator MoveToTarget(GameObject target)
        {
            Collider coll = target.GetComponentInChildren<Collider>();
            Vector3 dst;

            if(coll) dst = coll.ClosestPointOnBounds(viewController.model.transform.position);
            else dst = target.transform.position;
            if(transform.position.PlanerDistance(dst) > Constants.craft_distance)
            {
                GetComponent<Locomotor>().StartMove(target.gameObject);
                animator.SetInteger("MoveState", 1);
            }
            while(transform.position.PlanerDistance(dst) > Constants.craft_distance)
            {
                yield return null;
                if(coll) dst = coll.ClosestPointOnBounds(viewController.model.transform.position);
                else dst = target.transform.position;
            }
            animator.SetInteger("MoveState", 0);
            viewController.model.transform.LookAt(dst);
        }
        IEnumerator DoWork(Workable target)
        {
            yield return MoveToTarget(target.gameObject);
            animator.SetTrigger("Work");
            while(!triggers["workfinish"]) yield return null;
            target.Work(gameObject, 1);
            finish = true;
        }
        IEnumerator DoPickup(Pickupable target)
        {
            yield return MoveToTarget(target.gameObject);
            //隐藏手部工具
            if(inventoryController.handEquipment != null)
            {
                inventoryController.handEquipment.gameObject.SetActive(false);
            }

            animator.SetTrigger("Pickup");
            while(!triggers["pickup"]) yield return null;
            if(!inventoryController.AddItem(target.Pickup()))
            {
                //TODO:背包已满拾取失败,播放失败音效
            }
            while(!triggers["pickfinish"]) yield return null;
            if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);

            finish = true;      
        }
        IEnumerator DoHarvest(Harvestable target)
        {
            yield return MoveToTarget(target.gameObject);
            //隐藏手部工具
            inventoryController.HideHandItem();
            if(!target.CanHarvest())
            {
                finish = true;
                inventoryController.ShowHandItem();
                yield break;
            }
            animator.SetTrigger("Harvest");
            yield return StartCoroutine(HarvestTimer(target.harvestTime));
            if(!target.CanHarvest())
            {
                finish = true;
                inventoryController.ShowHandItem();
                yield break;
            }
            foreach (var item in target.Harvest())
            {
                inventoryController.AddItem(item);
            }
            
            inventoryController.ShowHandItem();
            finish = true; 
        }
        IEnumerator HarvestTimer(float timer)
        {
            yield return new WaitForSeconds(timer);
            animator.SetTrigger("HarvestEnd");
        }
        public override void Interrupted()
        {
            if(current != null)
            {
                StopCoroutine(current);
                //不能直接使用substate名字
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Pickup") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Work") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Harvest_Begin") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Harvesting") || 
                animator.GetCurrentAnimatorStateInfo(0).IsName("Harvest_End"))
                    animator.SetTrigger("StopCraft");
                if(animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) animator.SetInteger("MoveState", 0);
                if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
                if(GetComponent<Locomotor>().inMove) GetComponent<Locomotor>().StopMove();
            }
        }
    }
}