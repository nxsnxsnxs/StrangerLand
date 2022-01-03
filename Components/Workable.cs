using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public enum WorkToolType
    {
        Axe, Pickaxe, Shovel
    }
    public class Workable : GameComponent
    {
        public WorkToolType toolType;
        public bool workable;
    }
}