using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

public class RookieSword : MonoBehaviour
{
    private Sprite inventoryIcon;
    private AnimatorOverrideController swordAnim;
    void Awake()
    {
        inventoryIcon = ABManager.Instance.LoadAsset<Sprite>("RookieSword", "InventoryIcon");
        swordAnim = ABManager.Instance.LoadAsset<AnimatorOverrideController>("RookieSword", "SwordAnim");

        Pickable pickable = gameObject.AddGameComponent<Pickable>();
        pickable.type = PickType.Pickup;
        InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
        inventoryItem.icon = inventoryIcon;
        FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
        finiteUse.maxDurability = 100;
        finiteUse.durability = 100;
        Equipable equipable = gameObject.AddGameComponent<Equipable>();
        equipable.overrideAnimator = swordAnim;
        equipable.equipSlotType = EquipSlotType.Hand;
        equipable.pos = Vector3.zero;
        equipable.rot = Quaternion.identity;
        Weapon weapon = gameObject.AddGameComponent<Weapon>();
        weapon.damage = Constants.rookie_sword_damage;
        weapon.attackDistance = Constants.sword_attack_distance;
        weapon.attackDuration = Constants.sword_attack_duration;
        Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
        inspectable.inspectStr = Constants.rookie_sword_inspect_str;
    }
}
