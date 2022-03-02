using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Actions;

namespace MyBehaviourTree.Behaviours
{
    public class Follow<MoveAction> : ActionNode where MoveAction : BaseAction
    {
        private string targetName;
        private GameObject target;
        private float minFollowDistance;
        private float maxFollowDistance;
        private float startMoveTime;
        public Follow(GameObject _gameObject, string _name, string _targetName, float _minFollowDistance, float _maxFollowDistance) : base(_gameObject, _name)
        {
            targetName = _targetName;
            minFollowDistance = _minFollowDistance;
            maxFollowDistance = _maxFollowDistance;
        }

        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                target = gameObject.GetComponent<TargetTracker>().GetTarget(targetName);
                if(!target || !NeedFollowTarget())
                {
                    status = NodeState.Failure;
                    return;
                }
                if(DoFollowTarget()) status = NodeState.Running;
                else status = NodeState.Failure;
            }
            else if(status == NodeState.Running)
            {
                if(!target)
                {
                    gameObject.GetComponent<ActionController>().StopAction(typeof(MoveAction));
                    status = NodeState.Failure;
                    return;
                }
                if(!gameObject.GetComponent<ActionController>().isDoing<MoveAction>()) status = NodeState.Success;
            }
        }
        private bool NeedFollowTarget()
        {
            float distance = target.transform.position.PlanerDistance(gameObject.transform.position);
            return distance < minFollowDistance || distance > maxFollowDistance;
        }
        private bool DoFollowTarget()
        {
            Vector3 dst = GetFollowPos();
            if(dst == Vector3.zero) return false;
            gameObject.GetComponent<ActionController>().DoAction<MoveAction>(dst);
            return true;
        }
        private Vector3 GetFollowPos()
        {
            Vector3 dst;
            for(int i = 0; i < 3; ++i)
            {
                dst = ToolMethod.GetRandomPosInRange(target.transform.position, minFollowDistance, maxFollowDistance);
                if(MapManager.Instance.CanArrive(gameObject.transform.position, dst, gameObject.GetComponent<Collider>())) return dst;
            }
            return Vector3.zero;
        }
    }
}