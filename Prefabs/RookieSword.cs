using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace Prefabs
{
    public class RookieSword : PrefabComponent
    {
        private Sprite inventoryIcon;
        private AnimatorOverrideController swordAnim;

        public override string bundleName
        {
            get => "RookieSword";
        }

        public override void DefaultInit()
        {
            inventoryIcon = ABManager.Instance.LoadAsset<Sprite>("RookieSword", "InventoryIcon");
            swordAnim = ABManager.Instance.LoadAsset<AnimatorOverrideController>("RookieSword", "OverrideAnim");

            Pickupable pickable = gameObject.AddGameComponent<Pickupable>();
            InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
            inventoryItem.icon = inventoryIcon;
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxUse = 40;
            finiteUse.currUse = 40;
            Equipable equipable = gameObject.AddGameComponent<Equipable>();
            equipable.overrideAnimator = swordAnim;
            equipable.equipSlotType = EquipSlotType.Hand;
            equipable.pos = Vector3.zero;
            equipable.rot = Quaternion.identity;
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            Combat.Config conf = new Combat.Config();
            conf.damage = Constants.rookie_sword_damage;
            conf.attackDistance = Constants.sword_attack_distance;
            conf.hitDistance = Constants.sword_hit_distance;
            conf.attackDuration = Constants.sword_attack_duration;
            weapon.combatConf = conf;
            Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
            inspectable.inspectStr = Constants.rookie_sword_inspect_str;
        }
    }
}