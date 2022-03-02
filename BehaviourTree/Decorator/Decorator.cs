using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class Decorator : BehaviourTreeNode
    {
        public BehaviourTreeNode child;
        public Decorator(BehaviourTreeNode _child)
        {
            child = _child;
            child.parent = this;
        }
        public override void Reset()
        {
            status = NodeState.Ready;
            child.Reset();
        }
        public override void Step()
        {
            if(status != NodeState.Running) Reset();
            child.Step();
        }
    }
}