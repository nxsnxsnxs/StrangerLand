using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyBehaviourTree
{
    public abstract class RandomSelector : Composite
    {
        public int runningChild;
        private List<float> weights;
        public RandomSelector(BehaviourTree tree, List<BehaviourTreeNode> _children) : base(tree, _children)
        {
            weights = new List<float>(children.Count);
            for(int i = 0; i < weights.Count; ++i)
            {
                weights[i] = (i + 1) / weights.Count;
            }
        }
        public void SetWeights(List<float> _weights)
        {
            if(_weights.Count != weights.Count) return;
            foreach (var item in _weights)
            {
                if(item > 1) return;
            }
            _weights[_weights.Count - 1] = 1;
            weights = _weights;
        }
        public override void Tick()
        {
            while(status == NodeState.Ready)
            {
                float rand = Random.value;
                while(rand == 1) rand = Random.value;
                int index = weights.FindIndex((f) => {return f > rand;});
                children[index].Tick();
                if(children[index].status == NodeState.Running)
                {
                    runningChild = index;
                    status = NodeState.Running;
                }
                else if(children[index].status == NodeState.Success)
                {
                    runningChild = 0;
                    status = NodeState.Success;
                }
            }
            children[runningChild].Tick();
            if(children[runningChild].status != NodeState.Running)
            {
                runningChild = 0;
                status = children[runningChild].status;
                return;
            }
        }
        public override void Reset()
        {
            runningChild = 0;
            base.Reset();
        }
    }
}