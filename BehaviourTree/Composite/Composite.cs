using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class Composite : BehaviourTreeNode
    {
        protected List<BehaviourTreeNode> children;
        public Composite(BehaviourTree _bt, List<BehaviourTreeNode> _children) : base(_bt)
        {
            children = _children;
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
