using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class Sequence : Composite
    {
        private int runningChild = 0;
        public Sequence(BehaviourTree bt, List<BehaviourTreeNode> children) : base(bt, children)
        {
            
        }
        public override void Tick()
        {
            int start = runningChild;
            int i = 0;
            while(i < children.Count)
            {
                int index = (start + i) % children.Count;
                children[index].Tick();
                if(children[index].status == NodeState.Running)
                {
                    runningChild = (start + i) % children.Count;
                    status = NodeState.Running;
                    return;
                }
                else if(children[index].status == NodeState.Failure)
                {
                    runningChild = 0;
                    status = NodeState.Failure;
                    return;
                }
                ++i;
            }
            runningChild = 0;
            status = NodeState.Success;
        }
        public override void Reset()
        {
            runningChild = 0;
            base.Reset();
        }
    }
}