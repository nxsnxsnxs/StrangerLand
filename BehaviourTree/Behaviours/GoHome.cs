using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actions;
using Components;

namespace MyBehaviourTree.Behaviours
{
    public class GoHome<MoveAction> : ActionNode where MoveAction : BaseAction
    {
        private Vector3 homePoint;
        public GoHome(GameObject _gameObject, string _name, Vector3 _homePoint) : base(_gameObject, _name)
        {
            homePoint = _homePoint;
        }

        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                gameObject.GetComponent<Combat>().target = null;
                gameObject.GetComponent<ActionController>().DoAction<MoveAction>(homePoint);
                status = NodeState.Running;
            }
            else if(status == NodeState.Running)
            {
                if(!gameObject.GetComponent<ActionController>().isDoing<MoveAction>())
                {
                    status = NodeState.Success;
                }
            }
        }
    }
}