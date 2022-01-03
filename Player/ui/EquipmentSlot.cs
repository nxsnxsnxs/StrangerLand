using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Components;
using Tools;
using Player;

namespace Player.UI
{
    public class EquipmentSlot : InventorySlot, IPointerEnterHandler, IPointerExitHandler
    {
        Image image;

        protected override void Init()
        {
            image = transform.Find("Icon").GetComponent<Image>();
        }

        public void UpdateUI(InventoryItem item)
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

        public override void OnPointerEnter(PointerEventData eventData)
        {
            inventoryController.InspectItem(this);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            inventoryController.StopInspectItem();
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left) inventoryController.TryUseEquipment(this);
            else if(eventData.button == PointerEventData.InputButton.Right) inventoryController.TryUnEquipItem(this);
        }
    }
}