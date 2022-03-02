using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree.Behaviours
{
    public class Empty : ActionNode
    {
        public Empty(GameObject _gameObject) : base(_gameObject, "empty")
        {

        }

        public override void Tick()
        {
            status = NodeState.Success;
        }
    }
}