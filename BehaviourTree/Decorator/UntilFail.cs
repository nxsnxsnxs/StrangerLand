using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class UntilFail : Decorator
    {
        public UntilFail(BehaviourTreeNode _child) : base(_child)
        {

        }
        public override void Tick()
        {
            child.Tick();
            status = child.status == NodeState.Failure ? NodeState.Failure : NodeState.Running;
        }
    }
}