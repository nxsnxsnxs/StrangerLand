using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBehaviourTree;
using MyBehaviourTree.Behaviours;
using MyBehaviourTree.Events;
using Actions;
using Events;

namespace Brains
{
    public class PlantChewerBrain : Brain
    {
        public override void InitBehaviourTree()
        {
            BehaviourTreeNode root = new PrioritySelector(
                new List<BehaviourTreeNode>{
                    new GetHit<CommonAction.GetHit, CommonAction.Die>(gameObject, new CommonEvents.GetHit()),
                }
            );
            bt = new BehaviourTree(gameObject, root, 0.25f);
        }
    }
}