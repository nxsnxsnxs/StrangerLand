using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public class WhileNode : Parallel
    {
        public WhileNode(Conditional.ConditionFn _fn, ActionNode action)
        : base(new List<BehaviourTreeNode>{new Conditional(_fn), action})
        {

        }
    }
}