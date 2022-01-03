using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Attacker : GameComponent
    {
        public float attackCD;
        private Animator animator;
        private float lastAttack = 0;
        void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void Attack(GameObject target)
        {
            if(target == null || lastAttack + attackCD > Time.time) return;
            lastAttack = Time.time;
            animator.SetTrigger("Attack");
        }
    }
}