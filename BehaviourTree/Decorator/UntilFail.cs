using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class UntilFail : Decorator
    {
        public UntilFail(BehaviourTree tree, BehaviourTreeNode _child) : base(tree, _child)
        {

        }
        public override void Tick()
        {
            child.Tick();
            status = child.status == NodeState.Failure ? NodeState.Failure : NodeState.Running;
        }
    }
}