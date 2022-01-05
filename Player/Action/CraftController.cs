using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;

namespace Player.Action
{
    public class CraftController : PlayerAction
    {
        #region init
        //parameter
        public float tryCraftDistance;
        public float craftDistance;
        public float harvestTime;
        //reference components
        private Animator animator;
        private InventoryController inventoryController;
        private ViewController viewController;
        private ActionController actionController;
        private LocomotionController locomotionController;
        #endregion
        private Coroutine current;
        //coroutine trigger

        public override string actionName
        {
            get => "Craft";
        }
        public override int priority
        {
            get => 1;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            inventoryController = GetComponent<InventoryController>();
            viewController = GetComponent<ViewController>();
            actionController = GetComponent<ActionController>();
            locomotionController = GetComponent<LocomotionController>();
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
            finish = false;
            if(target.Length == 0) AutoDoWork();
            else if(target[0] is Workable) current = StartCoroutine(DoWork(target[0] as Workable));
            else if(target[0] is Pickable) current = StartCoroutine(DoPick(target[0] as Pickable));
        }
        void AutoDoWork()
        {
            Pickable pickTarget = transform.position.FindClosestTargetInRange<Pickable>(tryCraftDistance);
            if(inventoryController.handEquipment != null && inventoryController.handEquipment.GetComponent<Tool>() != null)
            {
                Workable workTarget = transform.position.FindClosestTargetInRange<Workable>(tryCraftDistance, 
                    (w) => { return w.toolType == inventoryController.handEquipment.GetComponent<Tool>().toolType; });
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
        IEnumerator DoWork(Workable target)
        {
            Collider coll = target.GetComponent<Collider>();
            Vector3 dst;

            if(coll) dst = coll.ClosestPointOnBounds(viewController.model.transform.position);
            else dst = target.transform.position;
            if(transform.position.PlanerDistance(dst) > craftDistance)
            {
                yield return StartCoroutine(locomotionController.MoveToPoint(dst));
            }
            viewController.model.transform.LookAt(dst);
            animator.SetTrigger("Work");
            while(!triggers["workfinish"]) yield return null;
            //TODO:效果
            finish = true;
            ResetActionTrigger();
        }
        IEnumerator DoPick(Pickable target)
        {
            Collider coll = target.GetComponent<Collider>();
            Vector3 dst;
            if(coll) dst = coll.ClosestPointOnBounds(viewController.model.transform.position);
            else dst = target.transform.position;
            if(transform.position.PlanerDistance(dst) > craftDistance)
            {
                yield return StartCoroutine(locomotionController.MoveToPoint(dst));
            }

            viewController.model.transform.LookAt(dst);
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
                yield return StartCoroutine(HarvestTimer(harvestTime));
                inventoryController.AddItem(target.Pick());
                if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
            }
            finish = true;
            ResetActionTrigger();
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
                if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
                ResetActionTrigger();
            }
        }
    }
}