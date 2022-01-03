using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree.Actions
{
    public class StandStill : Action
    {
        private float standTime;
        private float startTime;
        public StandStill(BehaviourTree _bt, float _standTime) : base(_bt)
        {
            standTime = _standTime;
        }
        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                startTime = Time.time;
                status = NodeState.Running;
            }
            else if(startTime + standTime <= Time.time) status = NodeState.Success;
        }
    }
}
