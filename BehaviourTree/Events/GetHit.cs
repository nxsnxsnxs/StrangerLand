using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Components;
using Actions;

namespace MyBehaviourTree.Events
{
    public class GetHit<GetHitAction, DieAction> : EventNode
    where GetHitAction : BaseAction
    where DieAction : BaseAction
    {
        public GetHit(GameObject gameObject, BaseEvent e)
        :base(gameObject, e, new PrioritySelector(
            new List<BehaviourTreeNode>{
                new IFNode(
                    () => gameObject.GetComponent<Health>().health == 0 && !gameObject.GetComponent<ActionController>().isDoing<CommonAction.Die>(),
                    new DoAction<DieAction>(gameObject, "die")
                ),
                new Sequence
                (
                    new List<BehaviourTreeNode>
                    {
                        new DoAction<GetHitAction>(gameObject, "gethit")
                    }
                )
            }
        ))
        {}
    }
}