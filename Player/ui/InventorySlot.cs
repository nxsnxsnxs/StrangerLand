using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.UI
{
    public abstract class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public abstract void OnPointerClick(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        protected abstract void Init();
        protected InventoryController inventoryController;
        public virtual void Awake()
        {
            inventoryController = GetComponentInParent<InventoryController>();
            Init();
        }
    }
}