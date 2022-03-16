using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Components;

namespace UI
{
    public abstract class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        protected InventoryPanel inventoryPanel;
        protected Image image;
        protected Vector3 originalPos;
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalPos = image.GetComponent<RectTransform>().position;
            image.transform.SetParent(inventoryPanel.dragPanel);
            image.raycastTarget = false;
        }
        public void OnDrag(PointerEventData eventData)
        {
            image.GetComponent<RectTransform>().position = Input.mousePosition;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            InventorySlot dst = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>();
            image.GetComponent<RectTransform>().position = originalPos;
            image.transform.SetParent(transform);
            image.raycastTarget = true;
            if(dst != null)
            {
                inventoryPanel.OnDragItem(this, dst);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryPanel.OnHoverSlot(this);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryPanel.StopHoverSlot();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left) inventoryPanel.OnLeftClickSlot(this);
            else if(eventData.button == PointerEventData.InputButton.Right) inventoryPanel.OnRightClickSlot(this);
        }
        public virtual void Init(InventoryPanel _inventoryPanel)
        {
            inventoryPanel = _inventoryPanel;
        }
        public abstract void UpdateUI(InventoryItem item);
    }
}