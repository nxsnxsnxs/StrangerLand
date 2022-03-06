using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.UI;
using Actions;

namespace Player.Actions
{
    public class UnEquipController : BaseAction
    {
        private InventoryController inventoryController;
        private Coroutine current;
        public override string actionName
        {
            get => "UnEquip";
        }

        void Awake()
        {
            inventoryController = GetComponent<InventoryController>();

            RegisterTrigger("unequip");
        }
        public override void Begin(params object[] target)
        {
            if(target.Length == 0 || !(target[0] is EquipmentSlot)) return;
            finish = false;
            current = StartCoroutine(UnEquip(target[0] as EquipmentSlot));
        }
        IEnumerator UnEquip(EquipmentSlot slot)
        {
            animator.SetTrigger("UnEquip");
            while(!triggers["unequip"]) yield return null;
            inventoryController.UnEquipItem(slot);
            finish = true;
            ResetActionTrigger();
        }
        public override void Interrupted()
        {
            animator.SetTrigger("StopUnEquip");
            if(current != null) StopCoroutine(current);
        }
    }
}