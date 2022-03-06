using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using Tools;
using Components;
using Actions;

namespace Player.Actions
{
    //有两种攻击方式，一是按f自动搜索目标进行攻击，二是鼠标选定目标进行攻击
    //攻击分为两部分，移动到目标附近，进行攻击（动画+动画事件）
    public class AttackController : BaseAction
    {
        private ViewController viewController;
        private InventoryController inventoryController;
        private Combat combat;
        private Coroutine current;
        private Coroutine move;
        private bool inAttack;

        public override string actionName
        {
            get => "Attack";
        }

        void Init()
        {
            viewController = GetComponent<ViewController>();
            inventoryController = GetComponent<InventoryController>();
            combat = GetComponent<Combat>();
        }
        
        void Awake()
        {
            Init();
            RegisterTrigger("attack");
            RegisterTrigger("attackfinish");
        }
        
        void FixedUpdate()
        {
            //if(attackTarget && !inAttack) CheckUpdate();
        }
        public override void Begin(params object[] target)
        {
            inAttack = false;
            if(target.Length == 0) FindTarget();
            else combat.target = target[0] as GameObject;
            if(combat.target) current = StartCoroutine(TryAttack());
            else finish = true;
        }
        //寻找距离最近的可攻击目标（带有attackable组件）
        void FindTarget()
        {
            Combat target = transform.position.FindClosestTargetInRange<Combat>(Constants.try_attack_distance, self : combat);
            combat.target = target?target.gameObject:null;
        }
        // 开始一次Chase
        public IEnumerator MoveToEnemy()
        {
            Vector3 attackTargetLastPos = combat.target.transform.position;
            Vector3 newPos = attackTargetLastPos;
            animator.SetInteger("MoveState", 1);
            GetComponent<Locomotor>().StartMove(combat.target);
            
            while(!combat.CanAttackTarget())
            {
                yield return null;
                if(!combat.IsValidTarget())
                {
                    finish = true;
                    StopCoroutine(current);
                    yield break;
                }
                newPos = combat.target.transform.position;
                if(newPos.PlanerDistance(attackTargetLastPos) > 0.8f)
                {
                    GetComponent<Locomotor>().RenewMove(combat.target);
                    attackTargetLastPos = newPos;
                }
            }
        }
        IEnumerator TryAttack()
        {
            if(!combat.CanAttackTarget())
            {
                yield return MoveToEnemy();
                GetComponent<Locomotor>().StopMove();
                animator.SetInteger("MoveState", 0);
            }
            viewController.model.transform.PlanerLookAt(combat.target.transform.position);
            if(!combat.CanDoAttack())
            {
                finish = true;
                yield break;
            }
            Attack();
            while(!triggers["attack"]) yield return null;
            if(combat.CanHitTarget()) GetComponent<Combat>().DoDamage();
            while(!triggers["attackfinish"]) yield return null;
            finish = true;
            ResetActionTrigger();
        }

        void Attack()
        {
            /*animator.SetTrigger("Attack");
            float speed =  animator.GetCurrentAnimatorStateInfo(0).length / attackDuration;
            animator.speed = speed;*/
            inAttack = true;
            AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
            if(!ac) ac = (animator.runtimeAnimatorController as AnimatorOverrideController).runtimeAnimatorController as AnimatorController;
            AnimatorState state = ac.GetAnimatorState(0, "Attack");
            AnimationClip clip = animator.GetAnimationClip("Attack");
            if(state && clip)
            {
                if(combat.attackDuration != 0) state.speed = clip.length / combat.attackDuration;
                else state.speed = 1;
            }
            animator.SetTrigger("Attack");
        }

        public override void Interrupted()
        {
            if(current != null) StopCoroutine(current);
            if(move != null)
            {
                StopCoroutine(move);
                animator.SetInteger("MoveState", 0);
            }
            if(inAttack) animator.SetTrigger("StopAttack");
            combat.target = null;
        }
    }
}