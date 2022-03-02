using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class DoFunc : ActionNode
    {
        private ActionNode.BehaviourFn fn;
        public DoFunc(GameObject _gameObject, string _name, ActionNode.BehaviourFn _fn) : base(_gameObject, _name)
        {
            fn = _fn;
        }

        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                bool result = fn();
                if(result) status = NodeState.Success;
                else status = NodeState.Failure;
            }
        }
    }
}