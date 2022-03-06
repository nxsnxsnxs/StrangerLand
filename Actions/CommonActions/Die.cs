using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brains;
using Tools;
using Components;
using Prefabs;

namespace Actions
{
    public abstract partial class CommonAction
    {
        public class Die : BaseAction
        {
            private Coroutine current;
            public override string actionName
            {
                get => "Die";
            }

            void Awake()
            {
                RegisterTrigger("diefinish");
            }
            public override void Begin(params object[] args)
            {
                current = StartCoroutine(DoDie());
            }
            private IEnumerator DoDie()
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Brain>().Stop();
                animator.SetTrigger("Die");
                Destroy(GetComponent<PrefabComponent>());
                foreach(var component in GetComponents<GameComponent>())
                {
                    Destroy(component);
                }
                while(!triggers["diefinish"]) yield return null;
                yield return new WaitForSeconds(Constants.deadbody_disappear_time);
                Destroy(gameObject);
            }

            public override void Interrupted()
            {
                
            }
            public override void Renew(params object[] args)
            {
                
            }
        }
    }
}