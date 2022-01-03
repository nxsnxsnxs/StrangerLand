using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class Parallel : Composite
    {
        public Parallel(BehaviourTree bt, List<BehaviourTreeNode> children) : base(bt, children)
        {

        }
        public override void Tick()
        {
            bool success = true;
            for(int i = 0; i < children.Count; ++i)
            {
                children[i].Tick();
                if(children[i].status == NodeState.Failure)
                {
                    status = NodeState.Failure;
                    for(int j = i + 1; j < children.Count; ++j)
                    {
                        if(children[j].status == NodeState.Running) children[j].Reset();
                    }
                    return;
                }
                else if(children[i].status == NodeState.Running) success = false;
            }
            if(success) status = NodeState.Success;
            else status = NodeState.Running;
        }
    }
}