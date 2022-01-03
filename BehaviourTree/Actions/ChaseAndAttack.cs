using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Components;

namespace MyBehaviourTree.Actions
{
    public class ChaseAndAttack : Action
    {
        private float attackDistance;
        public ChaseAndAttack(BehaviourTree _bt, float _attackDistance) : base(_bt)
        {
            attackDistance = _attackDistance;
        }
        public override void Tick()
        {
            GameObject target = bt.TryGetValue("target") as GameObject;
            if(target == null)
            {
                status = NodeState.Failure;
                return;
            } 
            if(GetAttackPos(target).PlanerDistance(bt.go.transform.position) < attackDistance * 0.9f)
            {
                Attacker attacker = bt.TryGetValue("attacker") as Attacker;
                Locomotor locomotor = bt.TryGetValue("locomotor") as Locomotor;
                locomotor.StopMove();
                attacker.Attack(target);
                status = NodeState.Success;
                bt.RemoveValue("target");
            }
            else
            {
                Locomotor locomotor = bt.TryGetValue("locomotor") as Locomotor;
                locomotor.MoveToPoint(GetAttackPos(target), true);
                status = NodeState.Running;
            }
            bt.debugStr += "ChaseAndAttack" + " " + status + "->";
        }
        //获取攻击目标的可攻击点
        //要对能与目标碰撞的最近的地方发起攻击而不是目标的中心点（因为目标可能拥有一个很大的collider）
        Vector3 GetAttackPos(GameObject attackTarget)
        {
            Transform transform = bt.go.transform;
            //float minY = Mathf.Min(transform.position.y, attackTarget.transform.position.y);
            Vector3 origin = new Vector3(transform.position.x, 0.1f, transform.position.z);
            Vector3 dst = new Vector3(attackTarget.transform.position.x, 0.1f, attackTarget.transform.position.z);
            RaycastHit[] raycastHits = Physics.RaycastAll(origin, dst - origin, Vector3.Distance(origin, dst));
            foreach(var item in raycastHits)
            {
                if(item.collider.gameObject == attackTarget.gameObject)
                {
                    //Debug.Log(item.point);
                    return item.point;
                } 
            }
            
            return transform.position;
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