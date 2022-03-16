using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Player;

namespace UI
{
    public class InventoryPanel : BasePanel
    {
        public override Layer layer => Layer.Panel;
        private GameObject itemSlotSkin;
        private GameObject handSlotSkin;
        private GameObject bodySlotSkin;
        private GameObject headSlotSkin;
        public List<ItemSlot> itemSlots;
        public List<EquipmentSlot> equipmentSlots;
        private Transform itemSlotsContainer;
        private Transform equipmentSlotsContainer;
        public Transform dragPanel;
        private InventoryController inventoryController;

        void Awake()
        {
            itemSlots = new List<ItemSlot>();
            equipmentSlots = new List<EquipmentSlot>();
            dragPanel = transform.Find("Drag Panel");
            itemSlotsContainer = transform.Find("Item Slots");
            equipmentSlotsContainer = transform.Find("Equipment Slots");
            itemSlotSkin = ABManager.Instance.LoadAsset<GameObject>("UIElement", "Item Slot");
            handSlotSkin = ABManager.Instance.LoadAsset<GameObject>("UIElement", "Hand Slot");
            bodySlotSkin = ABManager.Instance.LoadAsset<GameObject>("UIElement", "Body Slot");
            headSlotSkin = ABManager.Instance.LoadAsset<GameObject>("UIElement", "Head Slot");
        }
        public override void OnClose()
        {
            
        }

        public override void OnOpen(params object[] args)
        {
            inventoryController = args[0] as InventoryController;
            int capacity = (int)args[1];
            for(int i = 0; i < capacity; ++i)
            {
                CreateItemSlot(); 
            }
            CreateEquipmentSlot(handSlotSkin);
            CreateEquipmentSlot(bodySlotSkin);
            CreateEquipmentSlot(headSlotSkin);
        }
        public override void Open()
        {
            
        }
        public override void Close()
        {
            
        }
        void CreateItemSlot()
        {
            GameObject go = Instantiate(itemSlotSkin, itemSlotsContainer, false);
            ItemSlot slot = go.AddComponent<ItemSlot>();
            slot.Init(this);
            itemSlots.Add(slot);
        }
        void CreateEquipmentSlot(GameObject skin)
        {
            GameObject go = Instantiate(skin, equipmentSlotsContainer, false);
            EquipmentSlot slot = go.AddComponent<EquipmentSlot>();
            slot.Init(this);
            equipmentSlots.Add(slot);
        }
        public void UpdateItemSlots()
        {

        }
        public void UpdateEquipmentsSlots()
        {

        }
        public void OnHoverSlot(InventorySlot slot)
        {
            InventoryItem item;
            if(slot is ItemSlot) item = inventoryController.GetItem(slot as ItemSlot);
            else if(slot is EquipmentSlot) item = inventoryController.GetEquipment(slot as EquipmentSlot);
            else return;
            if(!item.GetComponent<Inspectable>()) return;
            PanelManager.Instance.Open<InspectPanel>("InspectPanel", item.GetComponent<Inspectable>());
        }
        public void StopHoverSlot()
        {
            PanelManager.Instance.Close("InspectPanel");
        }
        public void OnDragItem(InventorySlot org, InventorySlot dst)
        {
            if(org is ItemSlot && dst is ItemSlot)
            {
                inventoryController.SwitchItem(org as ItemSlot, dst as ItemSlot);
            }
        }
        public void OnLeftClickSlot(InventorySlot slot)
        {
            if(slot is ItemSlot)
            {
                inventoryController.TryUseItem(slot as ItemSlot);
            }
            else if(slot is EquipmentSlot)
            {
                inventoryController.TryUseEquipment(slot as EquipmentSlot);
            }
        }
        public void OnRightClickSlot(InventorySlot slot)
        {
            if(slot is ItemSlot)
            {
                inventoryController.TryDropItem(slot as ItemSlot);
            }
            else if(slot is EquipmentSlot)
            {
                inventoryController.TryUnEquipItem(slot as EquipmentSlot);
            }
        }
    }
}