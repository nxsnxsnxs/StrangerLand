using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;

namespace Prefabs
{
    public class Stone : PrefabComponent
    {
        public override string bundleName => "Stone";
        private Sprite inventoryIcon;

        public override void DefaultInit()
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
            inventoryIcon = ABManager.Instance.LoadAsset<Sprite>(bundleName, "InventoryIcon");

            Pickupable pickable = gameObject.AddGameComponent<Pickupable>();
            InventoryItem inventoryItem = gameObject.AddGameComponent<InventoryItem>();
            inventoryItem.icon = inventoryIcon;
            Stackable stackable = gameObject.AddGameComponent<Stackable>();
            stackable.count = 1;
            stackable.maxCount = Constants.normal_inventory_maxstack;
            Inspectable inspectable = gameObject.AddGameComponent<Inspectable>();
            inspectable.inspectStr = "stone";
            inspectable.rimLightMat = ABManager.Instance.LoadAsset<Material>("Material", Constants.default_rimLightMat_path);
        }
    }
}
