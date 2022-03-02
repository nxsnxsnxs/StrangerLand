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
            Vector3 size = gameObject.GetComponent<Collider>().bounds.size;
            Vector3 center = gameObject.GetComponent<Collider>().bounds.center;
            MapManager.Instance.RegisterBuildingLand(center, size.x, size.z);
        }
    }
}