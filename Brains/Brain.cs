using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;

namespace Brains
{
    public abstract class Brain : MonoBehaviour
    {
        public BehaviourTree bt;
        public bool debug;
        private List<ActionNode> leafs = new List<ActionNode>();
        private bool inWork = false;
        private float lastTick = 0;
        public abstract void InitBehaviourTree();
        public virtual void InitBlackBoard()
        {
            
        }

        void Awake()
        {
            InitBehaviourTree();
            InitBlackBoard();
        }
        void FixedUpdate()
        {
            if(inWork && lastTick + bt.interval <= Time.time)
            {
                bt.Tick();
                lastTick = Time.time;
                if(debug) ShowRunningNode();
            } 
        }
        public void Begin()
        {
            inWork = true;
        }
        public void ForceUpdate()
        {
            bt.ForceTick();
        }
        public void AddLeafs(ActionNode node)
        {
            leafs.Add(node);
        }
        public void Stop()
        {
            inWork = false;
        }
        private void ShowRunningNode()
        {
            string s = this.GetType().ToString() + "\n";
            foreach (var item in leafs)
            {
                if(item.status == BehaviourTreeNode.NodeState.Running) s += item.name + " ";
            }
            Debug.Log(s);
        }
    }
}