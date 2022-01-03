using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace Prefab
{
    public class Tree : MonoBehaviour
    {
        
        void Awake()
        {
            Pickable pickable = gameObject.AddGameComponent<Pickable>();
            pickable.type = PickType.Harvest;
            Workable workable = gameObject.AddGameComponent<Workable>();
            workable.workable = true;
            workable.toolType = WorkToolType.Axe;
            Attackable attackable = gameObject.AddGameComponent<Attackable>();
        }
    }
}