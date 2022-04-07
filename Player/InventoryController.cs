using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Prefabs;
using Components;
using UI;
using Events;

namespace Player
{
    public class InventoryController : MonoBehaviour, IArchiveSave
    {
        public int itemCapacity = 8;

        private Animator animator;
        private RuntimeAnimatorController handAnim;
        private EventHandler eventHandler;

        //背包中每个格子实际对应的物品
        public InventoryItem handEquipment
        {
            get => equipments[handSlot];
        }
        public InventoryItem bodyEquipment
        {
            get => equipments[bodySlot];
        }
        public InventoryItem headEquipment
        {
            get => equipments[headSlot];
        }
        private Dictionary<ItemSlot, InventoryItem> inventoryItems;
        private Dictionary<EquipmentSlot, InventoryItem> equipments;
        //背包中每个格子对应物品的实际存放处
        private Transform hand;
        private Transform head;
        private Transform body;
        private Transform itemContainer;
        private Dictionary<EquipSlotType, Transform> equipmentContainer;

        //UI Panel
        private InventoryPanel inventoryPanel;
        //装备栏对应的ui
        public EquipmentSlot handSlot
        {
            get => equipmentSlots[(int)EquipSlotType.Hand];
        }
        public EquipmentSlot bodySlot
        {
            get => equipmentSlots[(int)EquipSlotType.Body];
        }
        public EquipmentSlot headSlot
        {
            get => equipmentSlots[(int)EquipSlotType.Head];
        }
        public List<EquipmentSlot> equipmentSlots
        {
            get => inventoryPanel.equipmentSlots;
        }
        private class PersistentData
        {
            public List<int> itemData;
            public List<int> equipmentData;
        }
        
        void Awake()
        {
            animator = GetComponent<Animator>();
            handAnim = ABManager.Instance.LoadAsset<RuntimeAnimatorController>("PlayerCommon", "Animator");
            eventHandler = GetComponent<EventHandler>();
            inventoryPanel = PanelManager.Instance.Open<InventoryPanel>("InventoryPanel", this, itemCapacity);

            //初始化背包物品栏
            inventoryItems = new Dictionary<ItemSlot, InventoryItem>(itemCapacity);
            foreach (var slot in inventoryPanel.itemSlots)
            {
                inventoryItems[slot] = null;
            }
            equipments = new Dictionary<EquipmentSlot, InventoryItem>();
            equipments[handSlot] = null;
            equipments[bodySlot] = null;
            equipments[headSlot] = null;
            //初始化背包hierarchy
            itemContainer = transform.Find("Inventory");
            equipmentContainer = new Dictionary<EquipSlotType, Transform>();
            equipmentContainer[EquipSlotType.Hand] = hand;
            equipmentContainer[EquipSlotType.Body] = hand;
            equipmentContainer[EquipSlotType.Head] = hand;
        }
        
        void Start()
        {
            ArchiveManager.Instance.RegisterSave(GetInstanceID(), this);
        }
        public bool AddItem(InventoryItem item)
        {
            if(item == null)
            {
                Debug.LogError("Fail to add inventory item");
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
                //尝试与背包中相同类型的物品堆叠
                foreach (var slot in inventoryItems.Keys)
                {
                    if(inventoryItems[slot] && inventoryItems[slot].GetComponent<PrefabComponent>().GetType() == stackable.GetComponent<PrefabComponent>().GetType())
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
            if(!equipments[inventoryPanel.equipmentSlots[(int)equipable.equipSlotType]])
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
            equipments[equipmentSlots[(int)equipable.equipSlotType]] = item;
            equipable.Equip(equipmentContainer[equipable.equipSlotType]);
            if(equipable.GetComponent<Weapon>()) GetComponent<Combat>().SetWeapon(equipable.GetComponent<Weapon>());
            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            Destroy(item.GetComponent<Pickupable>());
            //if(equipable.overrideAnimator) ReplaceAnimator(equipable.overrideAnimator.runtimeAnimatorController);
            animator.runtimeAnimatorController = equipable.overrideAnimator;
        }
        //用新装备的animator替换原有animator
        //需要注意如果使用帧动画调用改函数，意味着该帧之后的帧不会被播放，因为替换动画机会重置动画的播放
        private void ReplaceAnimator(RuntimeAnimatorController newAnimator)
        {
            animator.runtimeAnimatorController = newAnimator;
            
        }
        //保留物品用于之后的操作(物理)
        private void SaveItem(InventoryItem item)
        {
            item.transform.SetParent(itemContainer);
            item.transform.localPosition = Vector3.zero;
            item.gameObject.SetActive(false);
            item.GetComponent<Rigidbody>().isKinematic = true;
            item.GetComponent<Collider>().enabled = false;
            Destroy(item.GetComponent<Pickupable>());
        }
        //丢弃物品(物理)
        private void FreeItem(InventoryItem item)
        {
            item.transform.SetParent(null);
            item.gameObject.SetActive(true);
            item.GetComponent<Rigidbody>().isKinematic = false;
            item.GetComponent<Collider>().enabled = true;
            item.gameObject.AddGameComponent<Pickupable>();
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
        public InventoryItem GetItem(ItemSlot slot)
        {
            if(inventoryItems.ContainsKey(slot)) return inventoryItems[slot];
            return null;
        }
        public InventoryItem GetEquipment(EquipmentSlot slot)
        {
            if(equipments.ContainsKey(slot)) return equipments[slot];
            return null;
        }


        //Inventory UI call action
        //Left Click Item
        public void TryUseItem(ItemSlot slot)
        {
            InventoryItem item = inventoryItems[slot];
            if(!item) return;
            //左击可装备物品进行装备
            if(item.GetComponent<Equipable>())
            {
                Equipable equipable = item.GetComponent<Equipable>();
                //已有装备在槽位上，先卸下装备
                if(equipments[equipmentSlots[(int)equipable.equipSlotType]])
                {
                    //先装备物品，后将之前的装备移至物品栏以防背包满的情况
                    InventoryItem originalEq = equipments[equipmentSlots[(int)equipable.equipSlotType]];
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
        //Right Click Item
        public void TryDropItem(ItemSlot slot)
        {
            if(!inventoryItems[slot]) return;
            eventHandler.RaiseEvent("Drop", slot);
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
        //RightClickEquipment
        public void TryUnEquipItem(EquipmentSlot slot)
        {
            if(!equipments[slot])
            {
                Debug.LogError("no equipment to unequip");
                return;
            }
            eventHandler.RaiseEvent("UnEquip", slot);
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
            Combat combat = GetComponent<Combat>();
            combat.ApplyConfig("default");
            if(!AddNormalItem(item))
            {
                FreeItem(item);
            }
            animator.runtimeAnimatorController = handAnim;
            UpdateUI();
        }
        public void HideHandItem()
        {
            if(!handEquipment) return;
            handEquipment.gameObject.SetActive(false);
        }
        public void ShowHandItem()
        {
            if(!handEquipment) return;
            handEquipment.gameObject.SetActive(true);
        }
        public string GetPersistentData()
        {
            PersistentData data = new PersistentData();

            foreach (var item in inventoryItems.Values)
            {
                data.itemData.Add(item.GetComponent<PrefabComponent>().GetInstanceID());
            }
            foreach (var item in equipments.Values)
            {
                data.equipmentData.Add(item.GetComponent<PrefabComponent>().GetInstanceID());
            }

            return "";
        }
    }
}