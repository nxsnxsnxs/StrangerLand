using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace Prefabs
{
    public class Tree : PrefabComponent
    {
        public override string bundleName
        {
            get => "Tree";
        }

        public override void DefaultInit()
        {
            Workable workable = gameObject.AddGameComponent<Workable>();
            workable.toolType = WorkToolType.Axe;
        }

        void Awake()
        {

        }
    }
}