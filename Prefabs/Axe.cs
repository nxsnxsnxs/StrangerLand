using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

using Debug = UnityEngine.Debug;

namespace Prefab
{
    using Components;
    public class Axe : MonoBehaviour
    {
        private AnimatorOverrideController axeAnimator;
        private Sprite inventoryIcon;
        void Awake()
        {
            axeAnimator = ABManager.Instance.LoadAsset<AnimatorOverrideController>("Axe", "AxeAnim");
            inventoryIcon = ABManager.Instance.LoadAsset<Sprite>("Axe", "InventoryIcon");
            
            Pickable pickable = gameObject.AddGameComponent<Pickable>();
            pickable.type = PickType.Pickup;
            InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
            inventoryItem.icon = inventoryIcon;
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            weapon.damage = Constants.axe_damage;
            Tool tool = gameObject.AddGameComponent<Tool>();
            tool.toolType = WorkToolType.Axe;
            tool.toolAnimator = axeAnimator;
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxDurability = 100;
            finiteUse.durability = 100;
            Equipable equipable = gameObject.AddGameComponent<Equipable>();
            equipable.equipSlotType = EquipSlotType.Hand;
            equipable.overrideAnimator = axeAnimator;
            equipable.pos = new Vector3(0.011f, -0.06f, 0.362f);
            equipable.rot = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
            Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
            inspectable.inspectStr = "nxsnb,nxsnb  nxsnxs";
            inspectable.rimLightMat = ABManager.Instance.LoadAsset<Material>("Material", Constants.default_rimLightMat_path);
        }
    }
}