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
        public PickType type;

        public InventoryItem Pick()
        {
            return gameObject.GetComponent<InventoryItem>();
        }
    }
}