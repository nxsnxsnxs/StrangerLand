using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Components
{
    public class LootSet
    {
        public List<KeyValuePair<GameObject, float>> loots;
        public LootSet()
        {
            loots = new List<KeyValuePair<GameObject, float>>();
        }
        public void AddLoot(GameObject loot, float chance)
        {
            if(loot && chance > 0)
            {
                loots.Add(new KeyValuePair<GameObject, float>(loot, chance));
            }
        }
    }
    public class LootSpawner : GameComponent
    {
        private Vector3 spawnPos;
        private Dictionary<string, LootSet> lootTable;

        void Awake()
        {
            lootTable = new Dictionary<string, LootSet>();
            spawnPos = GetComponent<Collider>().bounds.center;
        }
        public void AddLootSet(string name, LootSet lootSet)
        {
            if(lootTable.ContainsKey(name))
            {
                Debug.LogError("lootset name cant be same");
                return;
            }
            lootTable[name] = lootSet;
        }
        public List<GameObject> SpawnLoot(string name)
        {
            if(!lootTable.ContainsKey(name))
            {
                Debug.LogError("wrong lootset name");
                return null;
            }
            List<GameObject> result = new List<GameObject>();
            LootSet lootSet = lootTable[name];
            foreach (var item in lootSet.loots)
            {
                if(Random.value <= item.Value) result.Add(SpawnPrefab(item.Key));
            }
            return result;
        }
        public GameObject SpawnPrefab(GameObject gameObject)
        {
            GameObject go = Instantiate(gameObject, spawnPos, Quaternion.identity);
            Vector3 randomForce = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.value);
            randomForce.Normalize();
            randomForce *= Random.Range(3, 4);
            Rigidbody rb = go.GetComponent<Rigidbody>();
            rb.AddForce(randomForce, ForceMode.Impulse);
            return go;
        }
    }
}