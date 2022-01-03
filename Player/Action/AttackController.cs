using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using Tools;
using Components;

namespace Player.Action
{
    //有两种攻击方式，一是按f自动搜索目标进行攻击，二是鼠标选定目标进行攻击
    //攻击分为两部分，移动到目标附近，进行攻击（动画+动画事件）
    public class AttackController : PlayerAction
    {
        public float tryAttackDistance;
        private Animator animator;
        private LocomotionController locomotionController;
        private ViewController viewController;
        private InventoryController inventoryController;
        private Attackable attackTarget;
        private Vector3 attackTargetLastPos;
        private Coroutine current;
        private Weapon weapon
        {
            get
            {
                if(inventoryController.handEquipment) return inventoryController.handEquipment.GetComponent<Weapon>();
                return null;
            }
        }
        private float damage
        {
            get
            {
                if(weapon != null) return weapon.damage;
                else return Constants.hand_damage;
            }
        }
        private float attackDistance
        {
            get
            {
                if(weapon != null) return weapon.attackDistance;
                else return Constants.default_attack_distance;
            }
        }
        private float attackDuration
        {
            get
            {
                if(weapon != null) return weapon.attackDuration;
                else return Constants.hand_attack_duration;
            }
        }

        public override string actionName
        {
            get => "Attack";
        }

        public override int priority
        {
            get => 1;
        }

        void Init()
        {
            animator = GetComponent<Animator>();
            locomotionController = GetComponent<LocomotionController>();
            viewController = GetComponent<ViewController>();
            inventoryController = GetComponent<InventoryController>();
            enabled = false;
        }
        
        void Awake()
        {
            Init();
            RegisterTrigger("attack");
            RegisterTrigger("attackfinish");
        }
        
        void FixedUpdate()
        {
            if(attackTarget) CheckUpdate();
        }
        public override void Begin(params object[] target)
        {
            finish = false;
            if(target.Length == 0) FindTarget();
            else attackTarget = target[0] as Attackable;
            current = StartCoroutine(TryAttack());
        }
        //寻找距离最近的可攻击目标（带有attackable组件）
        void FindTarget()
        {
            attackTarget = null;

            Attackable[] targets =  FindObjectsOfType<Attackable>();
            float minDistance = float.MaxValue;
            Attackable closestTarget = null;
            foreach(var target in targets)
            {
                float distance = transform.position.PlanerDistance(target.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closestTarget = target;
                }
            }
            if(minDistance <= tryAttackDistance)
            {
                attackTarget = closestTarget;
            } 
        }
        /// <summary>
        /// 开始一次Chase
        /// </summary>
        public IEnumerator MoveToEnemy()
        {
            attackTargetLastPos = attackTarget.GetComponent<Collider>().ClosestPointOnBounds(viewController.model.transform.position);
            yield return StartCoroutine(locomotionController.MoveToPoint(attackTargetLastPos, attackDistance));
        }
        IEnumerator TryAttack()
        {
            float distance = attackTarget.GetComponent<Collider>().ClosestPoint(viewController.model.transform.position).PlanerDistance(transform.position);
            if(distance > attackDistance) yield return MoveToEnemy();
            Attack();
            while(!triggers["attack"]) yield return null;
            DoDamage();
            while(!triggers["attackfinish"]) yield return null;
            finish = true;
            ResetActionTrigger();
        }

        void CheckUpdate()
        {
            Vector3 newPos = attackTarget.GetComponent<Collider>().ClosestPointOnBounds(viewController.model.transform.position);
            float distance = newPos.PlanerDistance(transform.position);
            //目标过远，丢失目标
            if(distance > 15)
            {
                attackTarget = null;
                StopCoroutine(TryAttack());
                ResetActionTrigger();
                finish = true;
                return;
            }
            //目标移动，重新寻路
            if(newPos.PlanerDistance(attackTargetLastPos) > 0.9f)
            {
                StopCoroutine(current);
                current = StartCoroutine(TryAttack());
            }
        }
        void Attack()
        {
            /*animator.SetTrigger("Attack");
            float speed =  animator.GetCurrentAnimatorStateInfo(0).length / attackDuration;
            animator.speed = speed;*/
            AnimatorController ac = (AnimatorController)animator.runtimeAnimatorController;
            AnimatorState state = ac.GetAnimatorState(0, "Attack");
            AnimationClip clip = animator.GetAnimationClip(state.motion.name);
            state.speed = clip.length / attackDuration;
            animator.SetTrigger("Attack");
        }
        void DoDamage()
        {
            if(attackTarget == null) return;
            attackTarget.GetHit();            
        }
        void AttackFinish()
        {

        }

        public override void Interrupted()
        {
            if(current != null) StopCoroutine(current);
            animator.SetTrigger("StopAttack");
            attackTarget = null;
            ResetActionTrigger();
        }
    }
}