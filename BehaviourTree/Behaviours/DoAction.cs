using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actions;
using Player.Actions;

namespace MyBehaviourTree
{
    public class DoAction<T> : ActionNode where T : BaseAction
    {
        private object[] actionArgs;
        public DoAction(GameObject _gameObject, string _name,  params object[] args) : base(_gameObject, _name)
        {
            actionArgs = args;
        }

        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                gameObject.GetComponent<ActionController>().DoAction<T>(actionArgs);
                status = NodeState.Running;
            }
            else if(status == NodeState.Running)
            {
                if(!gameObject.GetComponent<ActionController>().isDoing<T>())
                {
                    status = NodeState.Success;
                }
            }            
        }
    }
}