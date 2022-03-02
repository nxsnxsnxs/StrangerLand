using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using UnityEditor.Animations;
using Tools;

namespace Actions
{
    public abstract partial class CommonAction
    {
        public class Attack : BaseAction
        {
            protected virtual string attackAnimStr
            {
                get => "Attack";
            }
            protected virtual string stopAttackAnimStr
            {
                get => "StopAttack";
            }
            protected Combat combat;
            protected Coroutine current;
            protected void Init()
            {
                animator = GetComponent<Animator>();
                combat = GetComponent<Combat>();
            }
        
            void Awake()
            {
                Init();
                RegisterTrigger("attack");
                RegisterTrigger("attackfinish");
            }
            public override string actionName
            {
                get => "Attack";
            }

            public override void Begin(params object[] args)
            {
                finish = false;
                current = StartCoroutine(DoAttack());
            }
            protected virtual IEnumerator DoAttack()
            {
                SetAnimSpeed();
                animator.SetTrigger(attackAnimStr);

                while(!triggers["attack"]) yield return null;
                DoDamage();
                while(!triggers["attackfinish"]) yield return null;
                finish = true;
            }
            protected virtual void DoDamage()
            {
                combat.TryDoDamage();
            }
            protected void SetAnimSpeed()
            {
                AnimatorController ac = (AnimatorController)animator.runtimeAnimatorController;
                AnimatorState state = ac.GetAnimatorState(0, attackAnimStr);
                if(state)
                {
                    AnimationClip clip = animator.GetAnimationClip(state.motion.name);
                    if(clip)
                    {
                        if(combat.attackDuration != 0) state.speed = clip.length / combat.attackDuration;
                        else state.speed = 1;
                    }
                }                
            }
            public override void Interrupted()
            {
                if(current != null) StopCoroutine(current);
                animator.SetTrigger(stopAttackAnimStr);
            }
        }
    }
}