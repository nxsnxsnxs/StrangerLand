using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class Conditional : BehaviourTreeNode
    {
        public delegate bool ConditionFn();
        protected ConditionFn fn;
        public Conditional(ConditionFn _fn)
        {
            fn = _fn;
        }
        public override void Tick()
        {
            if(fn()) status = NodeState.Success;
            else status = NodeState.Failure;
        }
    }
}