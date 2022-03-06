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
            else if(target[0] is Pickable) current = StartCoroutine(DoPick(target[0] as Pickable));
        }
        void AutoDoCraft()
        {
            Pickable pickTarget = transform.position.FindClosestTargetInRange<Pickable>(Constants.try_craft_distance);
            if(inventoryController.handEquipment != null && inventoryController.handEquipment.GetComponent<Tool>() != null)
            {
                Workable workTarget = transform.position.FindClosestTargetInRange<Workable>(Constants.try_craft_distance, 
                    (w) => { return inventoryController.handEquipment.GetComponent<Tool>().toolTypes.Contains(w.toolType); });
                //同一个物体包含Workable和Pickable，优先Workable，其余情况按照就近原则
                if(workTarget != null)
                {
                    if(pickTarget != null)
                    {
                        if(workTarget.gameObject == pickTarget.gameObject) current = StartCoroutine(DoWork(workTarget));
                        else if(transform.position.PlanerDistance(pickTarget.transform.position) < 
                            transform.position.PlanerDistance(workTarget.transform.position)) current = StartCoroutine(DoPick(pickTarget));
                        else current = StartCoroutine(DoWork(workTarget));
                    }
                    else current = StartCoroutine(DoWork(workTarget));
                    return;
                }
            }
            if(pickTarget != null) current = StartCoroutine(DoPick(pickTarget));
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
            target.OnWork(gameObject);
            finish = true;
        }
        IEnumerator DoPick(Pickable target)
        {
            yield return MoveToTarget(target.gameObject);
            //隐藏手部工具
            if(inventoryController.handEquipment != null)
            {
                inventoryController.handEquipment.gameObject.SetActive(false);
            }

            if(target.type == PickType.Pickup)
            {
                animator.SetTrigger("Pickup");
                while(!triggers["pickup"]) yield return null;
                inventoryController.AddItem(target.Pick());
                while(!triggers["pickfinish"]) yield return null;
                if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
            }
            else
            {
                animator.SetTrigger("Harvest");
                yield return StartCoroutine(HarvestTimer(target.harvestTime));
                inventoryController.AddItem(target.Pick());
                if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
            }
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