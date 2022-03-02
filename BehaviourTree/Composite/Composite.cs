using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class Composite : BehaviourTreeNode
    {
        protected List<BehaviourTreeNode> children;

        public Composite(List<BehaviourTreeNode> _children)
        {
            children = _children;
            foreach (var child in children)
            {
                child.parent = this;
            }
        }
        public override void Reset()
        {
            status = NodeState.Ready;
            foreach (var item in children)
            {
                item.Reset();
            }
        }
        public override void Step()
        {
            if(status != NodeState.Running) Reset();
            else
            {
                foreach (var child in children)
                {
                    child.Step();
                }
            }
        }
    }
}
