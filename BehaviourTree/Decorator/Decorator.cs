using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class Decorator : BehaviourTreeNode
    {
        public BehaviourTreeNode child;
        public Decorator(BehaviourTree tree, BehaviourTreeNode _child) : base(tree)
        {
            child = _child;
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