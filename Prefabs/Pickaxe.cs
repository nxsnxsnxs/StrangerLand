using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Components;
using Tools;

namespace Prefabs
{
    public class Pickaxe : PrefabComponent
    {
        public override string bundleName
        {
            get => "Pickaxe";
        }

        public override void DefaultInit()
        {
            AnimatorOverrideController pickaxeAnimator = ABManager.Instance.LoadAsset<AnimatorOverrideController>(bundleName, "OverrideAnim");
            Sprite inventoryIcon = ABManager.Instance.LoadAsset<Sprite>(bundleName, "InventoryIcon");

            Pickupable pickable = gameObject.AddGameComponent<Pickupable>();

            InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
            inventoryItem.icon = inventoryIcon;

            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            Combat.Config conf = new Combat.Config();
            conf.damage = Constants.pickaxe_damage;
            conf.attackDuration = Constants.tool_attack_duration;
            conf.attackDistance = Constants.tool_attack_distance;
            conf.hitDistance = Constants.tool_hit_distance;
            weapon.combatConf = conf;

            Tool tool = gameObject.AddGameComponent<Tool>();
            tool.toolTypes = new List<WorkToolType>{ WorkToolType.Pickaxe };

            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxUse = 50;
            finiteUse.currUse = 50;

            Equipable equipable = gameObject.AddGameComponent<Equipable>();
            equipable.equipSlotType = EquipSlotType.Hand;
            equipable.overrideAnimator = pickaxeAnimator;
            equipable.pos = new Vector3(0.005f, -0.019f, 0.38f);
            equipable.rot = new Quaternion(0.5f, -0.5f, -0.5f, 0.5f);
            
            Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
            inspectable.inspectStr = "nxsnb,nxsnb  nxsnxs";
            inspectable.rimLightMat = ABManager.Instance.LoadAsset<Material>("Material", Constants.default_rimLightMat_path);
        }
    }
}