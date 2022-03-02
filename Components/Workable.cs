using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Tools;

namespace Components
{
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
        //***************************
        public void OnWork(GameObject worker)
        {
            List<EventHandler> listeners = transform.position.FindObjectsOfTypeInRange<EventHandler>(15);
            if(listeners != null)
            {
                foreach (var item in listeners)
                {
                    item.RaiseEvent("hasmineraround", worker);
                }
            }
        }
    }
}