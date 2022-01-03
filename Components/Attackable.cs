using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(Collider))]
    public class Attackable : GameComponent
    {
        private Brain brain;
        private Animator animator;

        void Awake()
        {
            brain = GetComponent<Brain>();
            animator = GetComponent<Animator>();
        }
        public void GetHit()
        {
            if(brain) brain.bt.Sleep(0.667f);
            if(animator) animator.SetTrigger("GetHit");
        }
    }
}