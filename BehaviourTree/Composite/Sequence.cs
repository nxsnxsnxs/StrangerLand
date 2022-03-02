using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class Sequence : Composite
    {
        protected int runningChild = 0;
        public Sequence(List<BehaviourTreeNode> children) : base(children)
        {
            
        }
        public override void Tick()
        {
            int curr = runningChild;
            while(curr < children.Count)
            {
                children[curr].Tick();
                if(children[curr].status == NodeState.Running)
                {
                    runningChild = curr;
                    status = NodeState.Running;
                    return;
                }
                else if(children[curr].status == NodeState.Failure)
                {
                    runningChild = 0;
                    status = NodeState.Failure;
                    return;
                }
                ++curr;
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