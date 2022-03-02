using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyBehaviourTree
{
    public class RandomNode : Composite
    {
        public int runningChild;
        private float[] weights;
        public RandomNode(List<BehaviourTreeNode> _children, float[] _weights) : base(_children)
        {
            weights = _weights;
        }
        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                float rand = Random.value;
                int index = Array.FindIndex(weights, 0, weights.Length, (f) => {return f >= rand;});
                runningChild = index;
            }
            children[runningChild].Tick();
            if(children[runningChild].status == NodeState.Running) status = NodeState.Running;
            else status = children[runningChild].status;
        }
        public override void Reset()
        {
            runningChild = 0;
            base.Reset();
        }
    }
}