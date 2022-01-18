using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace Prefabs
{
    public class Tree : PrefabComponent
    {
        public override string loadPath
        {
            get => "Tree";
        }

        public override void DefaultInit()
        {
            Pickable pickable = gameObject.AddGameComponent<Pickable>();
            pickable.type = PickType.Harvest;
            Workable workable = gameObject.AddGameComponent<Workable>();
            workable.workable = true;
            workable.toolType = WorkToolType.Axe;
            Attackable attackable = gameObject.AddGameComponent<Attackable>();
        }

        void Awake()
        {

        }
    }
}