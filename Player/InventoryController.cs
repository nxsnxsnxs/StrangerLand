using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Prefab;
using Components;
using UI;
using Player.UI;
using Player.Action;

namespace Player
{
    public class InventoryController : MonoBehaviour
    {
        public RuntimeAnimatorController handAnim;
        private Animator animator;
        private ActionController actionController;

        //背包中每个格子实际对应的物品
        [HideInInspector]public InventoryItem handEquipment
        {
            get => equipments[handSlot];
        }
        [HideInInspector]public InventoryItem bodyEquipment
        {
            get => equipments[bodySlot];
        }
        [HideInInspector]public InventoryItem headEquipment
        {
            get => equipments[headSlot];
        }
        private Dictionary<ItemSlot, InventoryItem> inventoryItems;
        private Dictionary<EquipmentSlot, InventoryItem> equipments;
        //背包中每个格子对应物品的实际存放处
        public Transform hand;
        public Transform head;
        public Transform body;
        public Transform itemContainer;
        private Dictionary<EquipSlotType, Transform> equipmentContainer;

        //UI Panel
        public Transform inventoryPanel;
        public Transform dragPanel;
        public Transform inspectPanel;
        //装备栏对应的ui
        public EquipmentSlot handSlot
        {
            get => equipmentSlots[EquipSlotType.Hand];
        }
        public EquipmentSlot bodySlot
        {
            get => equipmentSlots[EquipSlotType.Body];
        }
        public EquipmentSlot headSlot
        {
            get => equipmentSlots[EquipSlotType.Head];
        }
        private Dictionary<EquipSlotType, EquipmentSlot> equipmentSlots;
        //物品栏和装备栏的父物体
        private Transform inventorySlotsContainer;
        private Transform equipmentSlotsContainer;
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            actionController = GetComponent<ActionController>();

            inventorySlotsContainer = inventoryPanel.transform.Find("Inventory Slots");
            equipmentSlotsContainer = inventoryPanel.transform.Find("Equipment Slots");
            //初始化背包物品栏
            inventoryItems = new Dictionary<ItemSlot, InventoryItem>();
            foreach (var slot in inventorySlotsContainer.GetComponentsInChildren<ItemSlot>())
            {
                inventoryItems[slot] = null;
            }
            
            equipmentSlots = new Dictionary<EquipSlotType, EquipmentSlot>();
            equipmentSlots[EquipSlotType.Hand] = equipmentSlotsContainer.Find("Hand Slot").GetComponent<EquipmentSlot>();
            equipmentSlots[EquipSlotType.Body] = equipmentSlotsContainer.Find("Body Slot").GetComponent<EquipmentSlot>();
            equipmentSlots[EquipSlotType.Head] = equipmentSlotsContainer.Find("Head Slot").GetComponent<EquipmentSlot>();
            //初始化装备栏
            equipments = new Dictionary<EquipmentSlot, InventoryItem>();
            equipments[handSlot] = null;
            equipments[bodySlot] = null;
            equipments[headSlot] = null;

            equipmentContainer = new Dictionary<EquipSlotType, Transform>();
            equipmentContainer[EquipSlotType.Hand] = hand;
            equipmentContainer[EquipSlotType.Body] = hand;
            equipmentContainer[EquipSlotType.Head] = hand;
        }
        
        void FixedUpdate()
        {
            
        }
        public bool AddItem(InventoryItem item)
        {
            if(item == null)
            {
                Debug.Log("error");
                return false;
            }
            bool result = AddItemPhysically(item);
            UpdateUI();
            return result;
        }
        private bool AddItemPhysically(InventoryItem item)
        {
            if(item.GetComponent<Equipable>() != null) return AddEquipmentItem(item);
            else return AddNormalItem(item);            
        }
        //添加物品到物品栏
        private bool AddNormalItem(InventoryItem item)
        {
            //如果物品可堆叠，需先遍历物品栏尝试找到相同且未达到数量上限的物品
            if(item.GetComponent<Stackable>())
            {
                Stackable stackable = item.GetComponent<Stackable>();
                foreach (var slot in inventoryItems.Keys)
                {
                    if(inventoryItems[slot] && inventoryItems[slot].GetComponent(stackable.owner.GetType()))
                    {
                        Stackable oldStackable = inventoryItems[slot].GetComponent<Stackable>();
                        int remainSpace = oldStackable.maxCount - oldStackable.count;
                        oldStackable.count += Mathf.Min(remainSpace, stackable.count);
                        stackable.count -= Mathf.Min(remainSpace, stackable.count);
                        //全部堆叠完，无需保留该go，销毁
                        if(stackable.count == 0)
                        {
                            Destroy(item.gameObject);
                            return true;
                        }
                    }
                }
            }
            foreach (var slot in inventoryItems.Keys)
            {
                if(!inventoryItems[slot])
                {
                    inventoryItems[slot] = item;
                    SaveItem(item);
                    return true;
                }
            }
            return false;    
        }
        // 添加物品到装备栏，如果失败的话尝试添加到物品栏
        private bool AddEquipmentItem(InventoryItem item)
        {
            Equipable equipable = item.GetComponent<Equipable>();
            if(!equipments[equipmentSlots[equipable.equipSlotType]])
            {
                EquipItem(item);
                return true;
            }
            else return AddNormalItem(item);
        }
        //装备物品
        private void EquipItem(InventoryItem item)
        {
            Equipable equipable = item.GetComponent<Equipable>();
            equipments[equipmentSlots[equipable.equipSlotType]] = item;
            equipable.Equip(equipmentContainer[equipable.equipSlotType]);
            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            item.GetComponent<Pickable>().enabled = false;
            if(equipable.overrideAnimator) ReplaceAnimator(equipable.overrideAnimator.runtimeAnimatorController);
        }
        //用新装备的animator替换原有animator
        //需要注意如果使用帧动画调用改函数，意味着该帧之后的帧不会被播放，因为替换动画机会重置动画的播放
        private void ReplaceAnimator(RuntimeAnimatorController newAnimator)
        {
            animator.runtimeAnimatorController = newAnimator;
        }
        //保留物品用于之后的操作
        private void SaveItem(InventoryItem item)
        {
            item.transform.SetParent(itemContainer);
            item.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            item.GetComponent<Pickable>().enabled = false;
        }
        //丢弃物品
        private void FreeItem(InventoryItem item)
        {
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Collider>().enabled = true;
            item.GetComponent<Pickable>().enabled = true;
        }
        public void UpdateUI()
        {
            foreach (var slot in inventoryItems.Keys)
            {
                slot.UpdateUI(inventoryItems[slot]);
            }
            foreach (var slot in equipments.Keys)
            {
                slot.UpdateUI(equipments[slot]);
            }
        }
        //交换物品栏中的物品（仅限于物品栏和物品栏）
        public void SwitchItem(ItemSlot origin, ItemSlot dst)
        {
            InventoryItem temp = inventoryItems[origin];
            inventoryItems[origin] = inventoryItems[dst];
            inventoryItems[dst] = temp;
            UpdateUI();
        }
        public void InspectItem(ItemSlot slot)
        {
            InventoryItem item = inventoryItems[slot];
            if(!item) return;
            Inspectable inspectable = item.GetComponent<Inspectable>();
            if(!inspectable) return;
            UIManager.Instance.inspectWindow.InitInspect(inspectable);
        }
        public void InspectItem(EquipmentSlot slot)
        {
            InventoryItem item = equipments[slot];
            if(!item) return;
            Inspectable inspectable = item.GetComponent<Inspectable>();
            if(!inspectable) return;
            UIManager.Instance.inspectWindow.InitInspect(inspectable);
        }
        public void StopInspectItem()
        {
            if(UIManager.Instance.inspectWindow.current) UIManager.Instance.inspectWindow.StopInspect();
        }
        //Inventory UI call action
        
        //Left Click Item
        public void UseItem(ItemSlot slot)
        {
            InventoryItem item = inventoryItems[slot];
            if(!item) return;
            //左击可装备物品进行装备
            if(item.GetComponent<Equipable>())
            {
                Equipable equipable = item.GetComponent<Equipable>();
                //已有装备在槽位上，先卸下装备
                if(equipments[equipmentSlots[equipable.equipSlotType]])
                {
                    //先装备物品，后将之前的装备移至物品栏以防背包满的情况
                    InventoryItem originalEq = equipments[equipmentSlots[equipable.equipSlotType]];
                    EquipItem(item);
                    inventoryItems[slot] = null;
                    AddNormalItem(originalEq);
                }
                else
                {
                    EquipItem(item);
                    inventoryItems[slot] = null;
                }
            }
            else if(item.GetComponent<Usable>())
            {
                
            }
            UpdateUI();
        }
        //Right Click Item
        public void TryDropItem(ItemSlot slot)
        {
            if(!inventoryItems[slot])
            {
                Debug.LogError("no drop item");
                return;
            } 
            if(actionController.DoAction<DropController>(slot))
            {
                if(handEquipment) handEquipment.gameObject.SetActive(false);
            }
        }
        public void DropItem(ItemSlot slot)
        {
            if(!inventoryItems[slot])
            {
                Debug.LogError("no drop item");
                return;
            } 

            FreeItem(inventoryItems[slot]);
            inventoryItems[slot] = null;
            UpdateUI();
        }
        public void DropFinish()
        {
            if(handEquipment) handEquipment.gameObject.SetActive(true);
        }
        //Left Click Equipment
        public void TryUseEquipment(EquipmentSlot slot)
        {
            if(!equipments[slot])
            {
                Debug.LogError("no drop item");
                return;
            }
            if(!equipments[slot].GetComponent<Usable>()) return;
            
        }
        //RightClickEquipment
        public void TryUnEquipItem(EquipmentSlot slot)
        {
            if(!equipments[slot])
            {
                Debug.LogError("no equipment to unequip");
                return;
            } 
            actionController.DoAction<UnEquipController>(slot);
        }
        public void UnEquipItem(EquipmentSlot slot)
        {
            if(!equipments[slot])
            {
                Debug.LogError("no equipment to unequip");
                return;
            }
            InventoryItem item = equipments[slot];
            equipments[slot] = null;
            if(!AddNormalItem(item))
            {
                FreeItem(item);
            }
            ReplaceAnimator(handAnim);
            UpdateUI();
        }
    }
}