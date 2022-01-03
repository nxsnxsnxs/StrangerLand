using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Stackable : GameComponent
    {
        public int count;
        public int maxCount;
        public MonoBehaviour owner;
    }
}