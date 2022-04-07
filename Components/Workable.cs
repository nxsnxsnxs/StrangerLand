using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Tools;

namespace Components
{
    using EventHandler = Events.EventHandler;
    public enum WorkToolType
    {
        Axe, Pickaxe, Shovel
    }
    public class Workable : GameComponent
    {
        //********need set************
        public WorkToolType toolType;
        public int maxWork;
        public int currWork;
        public Action<Workable, GameObject> onWork;
        public Action<Workable, GameObject> onWorkExhausted;
        //***************************
        public void Work(GameObject worker, int workVal)
        {
            currWork -= workVal;
            onWork?.Invoke(this, worker);
            if(currWork <= 0) onWorkExhausted?.Invoke(this, worker);
        }
    }
}