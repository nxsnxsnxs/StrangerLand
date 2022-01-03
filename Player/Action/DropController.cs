using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.UI;

namespace Player.Action
{
    public class DropController : PlayerAction
    {
        private Animator animator;
        private InventoryController inventoryController;
        private Coroutine current;
        public override string actionName
        {
            get => "Drop";
        }
        public override int priority
        {
            get => 1;
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            inventoryController = GetComponent<InventoryController>();

            RegisterTrigger("drop");
            RegisterTrigger("dropfinish");
        }
        public override void Begin(params object[] target)
        {
            if(target.Length == 0 || !(target[0] is ItemSlot))
            {
                Debug.LogError("no drop target");
                return;
            }
            finish = false;
            current = StartCoroutine(DropItem(target[0] as ItemSlot));
        }
        IEnumerator DropItem(ItemSlot slot)
        {
            animator.SetTrigger("Drop");
            while(!triggers["drop"]) yield return null;
            inventoryController.DropItem(slot);
            while(!triggers["dropfinish"]) yield return null;
            inventoryController.DropFinish();
            finish = true;
            ResetActionTrigger();
        }

        public override void Interrupted()
        {
            animator.SetTrigger("StopDrop");
            inventoryController.DropFinish();
            ResetActionTrigger();
        }
    }
}