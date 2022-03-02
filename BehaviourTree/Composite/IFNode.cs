using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actions;

namespace MyBehaviourTree
{
    public class IFNode : Sequence
    {
        public IFNode(Conditional.ConditionFn _fn, params ActionNode[] actions)
        : base(new List<BehaviourTreeNode>{new Conditional(_fn)})
        {
            foreach(var node in actions)
            {
                children.Add(node);
            }
        }
    }
}