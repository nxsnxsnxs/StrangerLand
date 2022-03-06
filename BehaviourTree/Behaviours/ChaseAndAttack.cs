using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Actions;
using Tools;
using Components;

namespace MyBehaviourTree.Behaviours
{
    public class ChaseAndAttack<MoveAction, AttackAction> : ActionNode
    where MoveAction : BaseAction
    where AttackAction : BaseAction
    {
        private float maxChaseTime;
        private GameObject currTarget;
        private Vector3 targetPos;
        private bool doAttack = false;
        private float startChaseTime;
        private string combatConf;
        public ChaseAndAttack(GameObject _gameObject, string _name, float _maxChaseTime, string _combatConf) : base(_gameObject, _name)
        {
            maxChaseTime = _maxChaseTime;
            combatConf = _combatConf;
        }
        public override void Tick()
        {
            Combat combat = gameObject.GetComponent<Combat>();
            ActionController actionController = gameObject.GetComponent<ActionController>();
            if(status == NodeState.Running)
            {
                if(!combat.IsValidTarget() || combat.target != currTarget)
                {
                    status = NodeState.Failure;
                    return;
                }
                if(startChaseTime + maxChaseTime <= Time.time)
                {
                    combat.target = null;
                    status = NodeState.Success;
                    return;
                }
                if(doAttack)
                {
                    //完成了一次攻击
                    if(!actionController.isDoing<AttackAction>())
                    {
                        doAttack = false;
                        status = NodeState.Success;
                    }
                    return;
                }
                if(combat.CanDoAttack())
                {
                    gameObject.transform.PlanerLookAt(combat.target.transform.position);
                    actionController.DoAction<AttackAction>();
                    doAttack = true;
                    startChaseTime = Time.time;
                }
                else 
                {
                    if(combat.CanAttackTarget())
                    {
                        actionController.StopAction(typeof(MoveAction));
                    }
                    else if(!actionController.isDoing<MoveAction>() || combat.target.transform.position.PlanerDistance(targetPos) > 0.06)
                    {
                        targetPos = combat.target.transform.position;
                        actionController.DoAction<MoveAction>(combat.target);
                    }
                }
            }
            else if(status == NodeState.Ready && combat.IsValidTarget())
            {
                combat.ApplyConfig(combatConf);
                targetPos = combat.target.transform.position;
                currTarget = combat.target;
                startChaseTime = Time.time;
                status = NodeState.Running;
            }
            else status = NodeState.Failure;
        }
        public override void Reset()
        {
            base.Reset();
            doAttack = false;
        }
    }
}