using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;
using MyBehaviourTree.Behaviours;
using MyBehaviourTree.Events;
using Actions;
using Events;
using Components;

namespace Brains
{
    public class ScorpionBrain : Brain
    {
        public override void InitBehaviourTree()
        {
            BehaviourTreeNode root = new PrioritySelector
            (
                new List<BehaviourTreeNode>
                {
                    new GetHit<CommonAction.GetHit, CommonAction.Die>(gameObject, new CommonEvents.GetHit()),
                    new IFNode(ShouldTelsonAttack, new ChaseAndAttack<CommonAction.Move, ACScorpion.TelsonAttack>(gameObject, "telsonattack", 10, "telsonattack")),
                    new RandomNode
                    (
                        new List<BehaviourTreeNode>
                        {
                            new ChaseAndAttack<ACScorpion.Run, ACScorpion.PinchAttack>(gameObject, "attack", 100, "default"),
                            new ChaseAndAttack<ACScorpion.Run, ACScorpion.DoublePinchAttack>(gameObject, "attack", 100, "default")
                        },
                        new float[]{0.5f, 1}
                    ),
                    new Follow<ACScorpion.Run>(gameObject, "follow", "king", 5f, 8f)
                }
            );

            bt = new BehaviourTree(gameObject, root, 0.25f);
        }
        private bool ShouldTelsonAttack() 
        {
            return GetComponent<Combat>().IsReady("telsonattack") && !GetComponent<ActionController>().isDoing<CommonAction.Attack>();
        }
    }
}