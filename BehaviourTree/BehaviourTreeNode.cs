using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBehaviourTree
{
    public abstract class BehaviourTreeNode
    {
        public enum NodeState
        {
            Ready, Failure, Success, Running
        }
        public BehaviourTree bt;
        public NodeState status;

        public BehaviourTreeNode(BehaviourTree _bt)
        {
            bt = _bt;
        }
        public abstract void Tick();
        public virtual void Reset()
        {
            status = NodeState.Ready;
        }
        public virtual void Step()
        {
            if(status != NodeState.Running) Reset();
        }
    }
}