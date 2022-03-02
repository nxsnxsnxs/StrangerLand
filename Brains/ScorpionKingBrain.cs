using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;
using MyBehaviourTree.Behaviours;
using MyBehaviourTree.Events;
using Actions;
using Events;
using Components;
using Tools;
using Prefabs;

namespace Brains
{
    public class ScorpionKingBrain : Brain
    {
        public override void InitBehaviourTree()
        {
            BehaviourTreeNode root = new PrioritySelector
            (
                new List<BehaviourTreeNode>
                {
                    new DoFunc(gameObject, "findtarget", FindTarget),
                    new IFNode(ShouldSummonScorpion, new DoAction<ACScorpionKing.Summon>(gameObject, "summon")),
                    new GetHit<CommonAction.GetHit, CommonAction.Die>(gameObject, new CommonEvents.GetHit()),
                    new IFNode(ShouldTailAttack, new ChaseAndAttack<CommonAction.Move, ACScorpionKing.TailAttack>(gameObject, "tailattack", 15, "tailattack")),
                    new ChaseAndAttack<CommonAction.Move, CommonAction.Attack>(gameObject, "attack", 15, "default"),
                    new Sequence
                    (
                        new List<BehaviourTreeNode>
                        {
                            new Wander<CommonAction.Move>(gameObject, "wander", GetComponent<RememberLocation>().locations["spawnpoint"], 10),
                            new StandStill(gameObject, "wanderbreak", 10f)
                        }
                    )
                }
            );

            bt = new BehaviourTree(gameObject, root, 0.25f);
        }
        private bool ShouldSummonScorpion()
        {
            float lastSummon = GetComponent<Timer>().GetTimer("lastsummon");
            if(lastSummon + Constants.scorpionking_summon_cd > Time.time) return false;
            if(!GetComponent<Combat>().IsValidTarget()) return false;
            if(GetComponent<Commander>().SoliderCount >= Constants.scorpionking_summon_maxcount) return false;
            ActionController actionController = GetComponent<ActionController>();
            if(actionController.isDoing<CommonAction.Attack>() || actionController.isDoing<ACScorpionKing.TailAttack>()) return false;
            return true;
        }
        private bool ShouldDodge()
        {
            return true;
        }
        private bool ShouldTailAttack()
        {
            return GetComponent<Combat>().IsReady("tailattack") && !GetComponent<ActionController>().isDoing<CommonAction.Attack>();
        }
        private bool FindTarget()
        {
            Combat combat = GetComponent<Combat>();
            if(combat.IsValidTarget()) return false;
            var targets = GetComponent<RememberLocation>().locations["spawnpoint"].FindObjectsOfTypeInRange<PrefabComponent>(Constants.scorpionking_target_range, GetComponent<PrefabComponent>());
            if(targets.Count == 0) return false;
            foreach (var t in targets)
            {
                if(t.HasTag(GameTag.neutral | GameTag.player))
                {
                    combat.target = t.gameObject;
                    GetComponent<Commander>().ShareTarget();
                    return true;
                }
            }
            return false;
        }
    }
}