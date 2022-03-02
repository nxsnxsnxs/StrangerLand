using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;
using MyBehaviourTree.Behaviours;
using MyBehaviourTree.Events;
using Components;
using Tools;
using Actions;
using Events;

namespace Brains
{
    public class GolemEarthBrain : Brain
    {
        public override void InitBehaviourTree()
        {
            BehaviourTreeNode root = new PrioritySelector
            (
                new List<BehaviourTreeNode>{
                    new GetHit<CommonAction.GetHit, CommonAction.Die>(gameObject, new CommonEvents.GetHit()),
                    new IFNode(ShouldBackToHome, new GoHome<CommonAction.Move>(gameObject, "backtohome", GetComponent<RememberLocation>().locations["spawnpoint"])),
                    new IFNode(ShouldSpinAttack, new ChaseAndAttack<ACGolemEarth.Dash, ACGolemEarth.SpinAttack>(gameObject, "spinattack", 5f, "spinattack")),
                    new ChaseAndAttack<ACGolemEarth.Dash, CommonAction.Attack>(gameObject, "attack", 5f, "default"),
                    new Sequence
                    (
                        new List<BehaviourTreeNode>
                        {
                            new Wander<CommonAction.Move>(gameObject, "wander", GetComponent<RememberLocation>().locations["spawnpoint"], 8),
                            new StandStill(gameObject, "wanderbreak", 2.5f)
                        }
                    )
                }
            );
            bt = new BehaviourTree(gameObject, root, 0.25f);
            
        }
        private bool ShouldBackToHome()
        {
            Vector3 spawnpoint = GetComponent<RememberLocation>().locations["spawnpoint"];
            return transform.position.PlanerDistance(spawnpoint) > Constants.golem_backhome_dist;
        }
        private bool ShouldSpinAttack()
        {
            return GetComponent<Combat>().IsReady("spinattack") && !GetComponent<ActionController>().isDoing<CommonAction.Attack>();
        }
        private bool IsDead()
        {
            return GetComponent<Health>().health == 0 && !GetComponent<ActionController>().isDoing<CommonAction.Die>();
        }
    }
}