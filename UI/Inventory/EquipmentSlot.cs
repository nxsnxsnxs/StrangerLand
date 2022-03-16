using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Components;
using Tools;
using Player;

namespace UI
{
    public class EquipmentSlot : InventorySlot, IPointerEnterHandler, IPointerExitHandler
    {

        public override void Init(InventoryPanel _inventoryPanel)
        {
            base.Init(_inventoryPanel);
            image = transform.Find("Icon").GetComponent<Image>();
        }

        public override void UpdateUI(InventoryItem item)
        {
            if(item == null)
            {
                image.gameObject.SetActive(false);
                return;
            }
            else
            {
                image.gameObject.SetActive(true);
            }
            if(image) image.sprite = item.icon;
        }
    }
}