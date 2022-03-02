using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Actions
{
    public partial class CommonAction
    {
        public class DoubleAttack : CommonAction.Attack
        {
            void Awake()
            {
                Init();
                RegisterTrigger("attackfirst");
                RegisterTrigger("attacksecond");
                RegisterTrigger("attackfinish");
            }
            public override void Begin(params object[] args)
            {
                GetComponent<Combat>().ResetAttack();
                base.Begin(args);
            }
            protected override IEnumerator DoAttack()
            {
                SetAnimSpeed();
                animator.SetTrigger(attackAnimStr);

                while(!triggers["attackfirst"]) yield return null;
                combat.TryDoDamage();
                while(!triggers["attacksecond"]) yield return null;
                combat.TryDoDamage();
                while(!triggers["attackfinish"]) yield return null;
                finish = true;
            }
        }
    }
}