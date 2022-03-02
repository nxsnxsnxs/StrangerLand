using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class TargetTracker : GameComponent
    {
        private Dictionary<string, GameObject> targets;

        void Awake()
        {
            targets = new Dictionary<string, GameObject>();
        }
        public GameObject GetTarget(string name)
        {
            if(!targets.ContainsKey(name)) return null;
            return targets[name];
        }
        public void TrackTarget(string name, GameObject target)
        {
            targets[name] = target;
        }
    }
}