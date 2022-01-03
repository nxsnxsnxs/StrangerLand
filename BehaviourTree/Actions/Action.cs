using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class Action : BehaviourTreeNode
    {
        public Action(BehaviourTree _bt) : base(_bt)
        {
            
        }
        public override void Step()
        {
            if(status != NodeState.Running) Reset();
        }
    }
}