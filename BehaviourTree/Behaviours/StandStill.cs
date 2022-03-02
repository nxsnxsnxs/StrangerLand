using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree.Behaviours
{
    public class StandStill : ActionNode
    {
        private float standTime;
        private float startTime;
        public StandStill(GameObject _gameObject, string _name, float _standTime) : base(_gameObject, _name)
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
            else if(status == NodeState.Running)
            {
                if(startTime + standTime <= Time.time) status = NodeState.Success;
            }
        }
    }
}
