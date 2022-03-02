using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class LoopNode : Sequence
    {
        private int maxloop;
        private int looptime;
        public LoopNode(int _maxloop, List<BehaviourTreeNode> children) : base(children)
        {
            maxloop = _maxloop;
            looptime = 0;
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
            looptime++;
            if(looptime < maxloop) status = NodeState.Running;
            else status = NodeState.Success;
            runningChild = 0;
        }
        public override void Reset()
        {
            base.Reset();
            looptime = 0;
        }
    }
}