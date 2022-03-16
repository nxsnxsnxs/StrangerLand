using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Components;
using Player;

namespace UI
{
    public class ItemSlot : InventorySlot
    {
        Text countText;
        public override void Init(InventoryPanel _inventoryPanel)
        {
            base.Init(_inventoryPanel);
            image = transform.Find("Icon").GetComponent<Image>();
            countText = transform.Find("Count Text").GetComponent<Text>();
        }
        public override void UpdateUI(InventoryItem item)
        {
            if(item == null)
            {
                image.gameObject.SetActive(false);
                countText.gameObject.SetActive(false);
                return;
            }
            else
            {
                image.gameObject.SetActive(true);
                countText.gameObject.SetActive(true);
            }
            if(image) image.sprite = item.icon;
            Stackable stackable = item.GetComponent<Stackable>();
            if(stackable)
            {
                countText.gameObject.SetActive(true);
                countText.text = stackable.count.ToString();
            } 
            else countText.gameObject.SetActive(false);
        }
    }
}