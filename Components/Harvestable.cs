using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Harvestable : GameComponent
    {
        public float harvestTime;
        public delegate void HarvestDelegate();
        public HarvestDelegate onHarvest;
        
        public bool CanHarvest()
        {
            return true;
        }
        public List<InventoryItem> Harvest()
        {
            onHarvest?.Invoke();
            LootSpawner lootSpawner = GetComponent<LootSpawner>();
            List<GameObject> loots = lootSpawner.SpawnLoot("harvestloot");
            List<InventoryItem> results = new List<InventoryItem>();
            foreach (var item in loots)
            {
                if(item.GetComponent<InventoryItem>()) results.Add(item.GetComponent<InventoryItem>());
            }
            return results;
        }
    }
}