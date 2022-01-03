using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class PrioritySelector : Composite
    {
        int runningChild;
        public PrioritySelector(BehaviourTree bt, List<BehaviourTreeNode> children) : base(bt, children)
        {
            
        }
        public override void Tick()
        {
            for(int i = 0; i < children.Count; ++i)
            {
                children[i].Tick();
                if(children[i].status != NodeState.Failure)
                {
                    if(runningChild != -1 && runningChild != i) children[runningChild].Reset();
                    if(children[i].status == NodeState.Running) runningChild = i;
                    status = children[i].status;
                    return;
                }
            }
            status = NodeState.Failure;
        }
    }
}