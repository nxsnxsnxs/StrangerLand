using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class UntilSuccess : Decorator
    {
        public UntilSuccess(BehaviourTree tree, BehaviourTreeNode _child) : base(tree, _child)
        {

        }
        public override void Tick()
        {
            child.Tick();
            status = child.status == NodeState.Success ? NodeState.Success : NodeState.Running;
        }
    }
}