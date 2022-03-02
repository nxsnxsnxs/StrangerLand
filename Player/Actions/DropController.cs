using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.UI;
using Actions;

namespace Player.Actions
{
    public class DropController : BaseAction
    {
        private InventoryController inventoryController;
        private Coroutine current;
        public override string actionName
        {
            get => "Drop";
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
            current = StartCoroutine(DropItem(target[0] as ItemSlot));
        }
        IEnumerator DropItem(ItemSlot slot)
        {
            if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(false);
            animator.SetTrigger("Drop");
            while(!triggers["drop"]) yield return null;
            inventoryController.DropItem(slot);
            while(!triggers["dropfinish"]) yield return null;
            inventoryController.DropFinish();
            finish = true;
            if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
        }

        public override void Interrupted()
        {
            animator.SetTrigger("StopDrop");
            if(inventoryController.handEquipment) inventoryController.handEquipment.gameObject.SetActive(true);
            if(current != null) StopCoroutine(current);
        }
    }
}