using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Components;
using Player;

namespace Player.UI
{
    public class ItemSlot : InventorySlot, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        Image image;
        Text countText;
        Vector3 originalPos;
        protected override void Init()
        {
            image = transform.Find("Icon").GetComponent<Image>();
            countText = transform.Find("Count Text").GetComponent<Text>();
        }
        public void UpdateUI(InventoryItem item)
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

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPos = image.GetComponent<RectTransform>().position;
            image.transform.SetParent(inventoryController.dragPanel);
            image.raycastTarget = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            image.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ItemSlot dst = eventData.pointerCurrentRaycast.gameObject.GetComponent<ItemSlot>();
            image.GetComponent<RectTransform>().position = originalPos;
            image.transform.SetParent(transform);
            image.raycastTarget = true;
            if(dst != null)
            {
                inventoryController.SwitchItem(this, dst);
            }
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
            if(eventData.button == PointerEventData.InputButton.Left) inventoryController.UseItem(this);
            else if(eventData.button == PointerEventData.InputButton.Right) inventoryController.TryDropItem(this);
        }
    }
}