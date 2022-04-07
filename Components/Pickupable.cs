using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Pickupable : GameComponent
    {
        public InventoryItem Pickup()
        {
            return GetComponent<InventoryItem>();
        }
    }
}