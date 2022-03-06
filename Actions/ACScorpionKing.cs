using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Events;

namespace Actions
{
    public class ACScorpionKing
    {
        public class TailAttack : CommonAction.Attack
        {
            public override string actionName
            {
                get => "TailAttack";
            }
            protected override string attackAnimStr => "TailAttack";
            protected override string stopAttackAnimStr => "StopTailAttack";
        }
        public class Summon : BaseAction
        {
            private Coroutine current;
            private GameObject scorpionPrefab;
            public override string actionName
            {
                get => "Summon";
            }

            void Awake()
            {
                scorpionPrefab = ABManager.Instance.LoadAsset<GameObject>("Scorpion", "Prefab");
                RegisterTrigger("summonfinish");
            }

            public override void Begin(params object[] args)
            {
                current = StartCoroutine(DoSummon());
            }
            IEnumerator DoSummon()
            {
                animator.SetTrigger("Summon");
                GetComponent<Timer>().SetTimer("lastsummon");
                while(!triggers["summonfinish"]) yield return null;
                SummonScorpion();
                if(GetComponent<Commander>().SoliderCount < Constants.scorpionking_summon_maxcount) SummonScorpion();
                GetComponent<Commander>().ShareTarget();
                finish = true;
            }
            void SummonScorpion()
            {
                Vector3 pos = GetSpawnPos();
                if(pos == Vector3.zero) return;
                GameObject scorpion = Instantiate(scorpionPrefab, pos, Quaternion.identity);
                scorpion.GetComponent<EventHandler>().RaiseEvent("setcommander", gameObject);
                GetComponent<Commander>().AddSoldier(scorpion);
            }
            Vector3 GetSpawnPos()
            {
                float minradius = Mathf.Max(GetComponent<Collider>().bounds.extents.x, GetComponent<Collider>().bounds.extents.z) + 
                Mathf.Max(scorpionPrefab.GetComponent<Collider>().bounds.extents.x, scorpionPrefab.GetComponent<Collider>().bounds.extents.z);
                Vector3 pos;
                for(int i = 0; i < 3; ++i)
                {
                    pos = ToolMethod.GetRandomPosInRange(transform.position, minradius, Constants.scorpionking_summon_radius);
                    if(MapManager.Instance.CanStand(pos, scorpionPrefab.GetComponent<Collider>())) return pos;
                }
                return Vector3.zero;
            }

            public override void Interrupted()
            {
                if(current != null)
                {
                    StopCoroutine(current);
                    animator.SetTrigger("StopSummon");
                }
            }
        }
    }
}