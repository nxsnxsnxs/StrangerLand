using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class UntilSuccess : Decorator
    {
        public UntilSuccess(BehaviourTreeNode _child) : base(_child)
        {

        }
        public override void Tick()
        {
            child.Tick();
            status = child.status == NodeState.Success ? NodeState.Success : NodeState.Running;
        }
    }
}