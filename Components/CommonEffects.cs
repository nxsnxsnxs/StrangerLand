using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class CommonEffects
    {
        public class Poisoning : BaseEffect
        {
            public int damage;
            public override string name => "Poisoning";

            public override void DoEffect(GameObject gameObject)
            {
                gameObject.GetComponent<Health>().GetDamage(damage);
            }
        }
    }
}