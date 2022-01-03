using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;
using MyBehaviourTree.Actions;
using MyBehaviourTree.Conditions;
using Components;

using Parallel = MyBehaviourTree.Parallel;

public class GolemEarthBrain : Brain
{
    public override void InitBehaviourTree()
    {
        bt = new BehaviourTree(gameObject, 0.25f);

        bt.root = new PrioritySelector(
            bt, new List<BehaviourTreeNode>()
            {
                new Parallel(
                    bt, new List<BehaviourTreeNode>()
                    {
                        new UntilFail(bt, new HasTargetAround(bt, "Player", 6.0f)),
                        new ChaseAndAttack(bt, 2f),
                    }
                ),
                new Sequence
                (
                    bt, new List<BehaviourTreeNode>()
                    {
                        new Wander(bt, Vector3.zero, 10),
                        new StandStill(bt, 2.5f)
                    }
                )
            }
        );
    }
    public override void InitBlackBoard()
    {
        bt.SetValue("locomotor", GetComponent<Locomotor>());
        bt.SetValue("attacker", GetComponent<Attacker>());
    }
}
