using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public enum PickType
    {
        Pickup, Harvest
    }
    public class Pickable : GameComponent
    {
        //********need set************
        public PickType type;
        public float harvestTime;
        //****************************

        public InventoryItem Pick()
        {
            return gameObject.GetComponent<InventoryItem>();
        }
    }
}