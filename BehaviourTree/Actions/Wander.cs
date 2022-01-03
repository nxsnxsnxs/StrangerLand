using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace MyBehaviourTree.Actions
{
    public class Wander : Action
    {
        public Vector3 center;
        public float radius;
        public Wander(BehaviourTree tree, Vector3 _center, float _radius) : base(tree)
        {
            center = _center;
            radius = _radius;
        }
        public override void Tick()
        {
            if(status == NodeState.Ready)
            {
                Vector3 dst = GetDestination();
                Locomotor locomotor = bt.TryGetValue("locomotor") as Locomotor;
                if(locomotor == null)
                {
                    status = NodeState.Failure;
                    return;
                } 
                locomotor.MoveToPoint(dst, false);
                locomotor.moveDoneCallback += OnMoveDone;
                status = NodeState.Running;
            }
            bt.debugStr += "Wander" + " " + status + "->";
        }
        public Vector3 GetDestination()
        {
            Vector3 dst = center + new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));
            return dst;
        }
        public void OnMoveDone(bool result)
        {
            status = NodeState.Success;
            Locomotor locomotor = bt.TryGetValue("locomotor") as Locomotor;
            if(locomotor != null) locomotor.moveDoneCallback -= OnMoveDone;
        }
        public override void Reset()
        {
            if(status == NodeState.Running)
            {
                Locomotor locomotor = bt.TryGetValue("locomotor") as Locomotor;
                locomotor.StopMove();
            }
            base.Reset();
        }
    }
}