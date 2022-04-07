using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Events;

namespace Prefabs
{
    public class Rock : PrefabComponent
    {
        public override string bundleName
        {
            get => "Rock";
        }

        public override void DefaultInit()
        {
            Workable workable = gameObject.AddGameComponent<Workable>();
            workable.toolType = WorkToolType.Pickaxe;
            workable.maxWork = Constants.rock_maxwork;
            workable.currWork = workable.maxWork;
            workable.onWork += OnMined;
            workable.onWorkExhausted += OnWorkExhausted;

            SetLoot();

            Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
            Vector3 center = gameObject.GetComponent<Collider>().bounds.center;
            MapManager.Instance.RegisterBuildingLand(center, size.x, size.z);
        }
        private void OnMined(Workable self, GameObject worker)
        {
            List<EventHandler> listeners = transform.position.FindObjectsOfTypeInRange<EventHandler>(15);
            if(listeners != null)
            {
                foreach (var item in listeners)
                {
                    item.RaiseEvent("hasmineraround", worker);
                }
            }
            if(self.currWork == self.maxWork / 2)
            {
                GetComponent<LootSpawner>().SpawnLoot("midloot");
            }
            else if(self.currWork == 0)
            {
                GetComponent<LootSpawner>().SpawnLoot("finalloot");
            }
        }
        private void OnWorkExhausted(Workable self, GameObject worker)
        {
            Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
            Vector3 center = gameObject.GetComponent<Collider>().bounds.center;
            MapManager.Instance.UnRegisterBuildingLand(center, size.x, size.z);
            Destroy(gameObject);
        }
        private void SetLoot()
        {
            GameObject stoneSkin = ABManager.Instance.LoadAsset<GameObject>("Stone", "Prefab");
            LootSpawner lootSpawner = gameObject.AddGameComponent<LootSpawner>();
            LootSet midLoot = new LootSet();
            midLoot.AddLoot(stoneSkin, 1);
            midLoot.AddLoot(stoneSkin, 0.5f);
            LootSet finalLoot = new LootSet();
            finalLoot.AddLoot(stoneSkin, 1);
            finalLoot.AddLoot(stoneSkin, 1);
            finalLoot.AddLoot(stoneSkin, 1);
            finalLoot.AddLoot(stoneSkin, 0.5f);
            lootSpawner.AddLootSet("midloot", midLoot);
            lootSpawner.AddLootSet("finalloot", finalLoot);
        }
    }
}