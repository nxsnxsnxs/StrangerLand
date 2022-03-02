using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Events
{
    public abstract partial class CommonEvents
    {
        public class GetHit : BaseEvent
        {
            public override string name
            {
                get => "GetHit";
            }

            public override void fn(GameObject gameObject, params object[] args)
            {
                GameObject attacker = args[0] as GameObject;
                float damage = (float)args[1];
                gameObject.GetComponent<Combat>().GetHit(attacker, damage);
                gameObject.GetComponent<Combat>().target = attacker;
            }
        }
    }
}