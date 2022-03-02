using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Brains;

namespace MyBehaviourTree
{
    public class EventNode : BehaviourTreeNode
    {
        public BehaviourTreeNode child;
        public bool triggered;
        public GameObject gameObject;
        private BaseEvent e;
        public EventNode(GameObject _gameObject, BaseEvent _event, BehaviourTreeNode _child)
        {
            child = _child;
            child.parent = this;
            gameObject = _gameObject;
            e = _event;
            gameObject.GetComponent<EventHandler>().ListenEvent(e.name, OnEvent);
        }
        public void OnEvent(params object[] args)
        {
            if(status == NodeState.Running)
            {
                child.Reset();
            }
            triggered = true;
            e.fn(gameObject, args);
            gameObject.GetComponent<Brain>().ForceUpdate();
        }
        public override void Tick()
        {
            if(status == NodeState.Ready && triggered) status = NodeState.Running;
            if(status == NodeState.Running)
            {
                child.Tick();
                status = child.status;
            }
            else status = NodeState.Failure;
        }
        public override void Step()
        {
            triggered = false;
            if(status != NodeState.Running) Reset();
            else child.Step();
        }
        public override void Reset()
        {
            triggered = false;
            base.Reset();
            child.Reset();
        }
    }
}