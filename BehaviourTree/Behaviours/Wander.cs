using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Actions;
using Tools;

namespace MyBehaviourTree.Behaviours
{
    public class Wander<MoveAction> : ActionNode where MoveAction : BaseAction
    {
        public Vector3 center;
        public float radius;
        public Wander(GameObject _gameObject, string _name, Vector3 _center, float _radius) : base(_gameObject, _name)
        {
            center = _center;
            radius = _radius;
        }
        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                Vector3 dst = GetDestination();
                if(!MapManager.Instance.CanArrive(gameObject.transform.position, dst, gameObject.GetComponent<Collider>()) || dst.PlanerDistance(gameObject.transform.position) < 1.5f)
                {
                    status = NodeState.Failure;
                    return;
                }
                gameObject.GetComponent<ActionController>().DoAction<MoveAction>(dst);
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
        public Vector3 GetDestination()
        {
            Vector3 dst = center + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            return dst;
        }
    }
}