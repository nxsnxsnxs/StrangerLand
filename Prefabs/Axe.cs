using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Newtonsoft.Json;

namespace Prefabs
{
    using Components;
    public class Axe : PrefabComponent
    {
        private AnimatorOverrideController axeAnimator;
        private Sprite inventoryIcon;

        public override string bundleName
        {
            get => "Axe";
        }

        public override void DefaultInit()
        {
            axeAnimator = ABManager.Instance.LoadAsset<AnimatorOverrideController>(bundleName, "OverrideAnim");
            inventoryIcon = ABManager.Instance.LoadAsset<Sprite>(bundleName, "InventoryIcon");
            
            Pickable pickable = gameObject.AddGameComponent<Pickable>();
            pickable.type = PickType.Pickup;
            InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
            inventoryItem.icon = inventoryIcon;
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            Combat.Config conf = new Combat.Config();
            conf.damage = Constants.axe_damage;
            conf.attackDistance = Constants.tool_attack_distance;
            conf.hitDistance = Constants.tool_hit_distance;
            conf.attackDuration = Constants.tool_attack_duration;
            weapon.combatConf = conf;
            Tool tool = gameObject.AddGameComponent<Tool>();
            tool.toolTypes = new List<WorkToolType>{ WorkToolType.Axe };
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxUse = 50;
            finiteUse.currUse = 50;
            Equipable equipable = gameObject.AddGameComponent<Equipable>();
            equipable.equipSlotType = EquipSlotType.Hand;
            equipable.overrideAnimator = axeAnimator;
            equipable.pos = new Vector3(0.011f, -0.06f, 0.362f);
            equipable.rot = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
            Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
            inspectable.inspectStr = "nxsnb,nxsnb  nxsnxs";
            inspectable.rimLightMat = ABManager.Instance.LoadAsset<Material>("Material", Constants.default_rimLightMat_path);
        }

        void Save()
        {
            IArchiveSave[] components = GetComponents<IArchiveSave>();
            foreach (var item in components)
            {
                JsonConvert.SerializeObject(item);
            }
        }
    }
}