using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class Conditional : BehaviourTreeNode
    {
        protected Predicate<BehaviourTree> fn;
        public Conditional(BehaviourTree _bt, Predicate<BehaviourTree> _fn) : base(_bt)
        {
            fn = _fn;
        }
        public override void Tick()
        {
            if(fn.Invoke(bt)) status = NodeState.Success;
            else status = NodeState.Failure;
            bt.debugStr += this.GetType().Name + ":" + status + "->";
        }
    }
}