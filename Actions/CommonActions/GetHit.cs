using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
    public abstract partial class CommonAction
    {
        public class GetHit : BaseAction
        {
            public override string actionName
            {
                get => "GetHit";
            }
            protected virtual string getHitAnimStr
            {
                get => "GetHit";
            }
            protected virtual string stopGetHitAnimStr
            {
                get => "StopGetHit";
            }
            private Coroutine current;
            void Init()
            {
                RegisterTrigger("gethitfinish");
            }
            void Awake()
            {
                Init();
            }
            public override void Begin(params object[] args)
            {
                current = StartCoroutine(DoGetHit());
            }
            IEnumerator DoGetHit()
            {
                animator.SetTrigger(getHitAnimStr);
                while(!triggers["gethitfinish"]) yield return null;
                
                finish = true;
            }
            public override void Interrupted()
            {
                if(current != null)
                {
                    StopCoroutine(current);
                    current = null;
                    animator.SetTrigger(stopGetHitAnimStr);
                }
            }
        }
    }
}