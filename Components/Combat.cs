using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;
using Events;

namespace Components
{
    public class Combat : GameComponent
    {
        public class Config
        {
            public float damage;
            public float attackDuration;
            public float attackCD;
            public float attackDistance;
            public float hitDistance;
            public BaseEffect effect;
            //projectile
            public bool isProjectile;
            public GameObject projectilePrefab;
            public Transform shootPoint;
            public float flySpeed;
            public float maxFlyDistance;

            private float lastAttack;
            public bool IsReady => lastAttack + attackCD <= Time.time;
            public bool CanAttackTarget(float distance)
            {
                return distance <= attackDistance;
            }
            public bool CanHitTarget(float distance)
            {
                return distance <= hitDistance;
            }
            public void ResetAttack()
            {
                lastAttack = Time.time;
            }
        }
        public float damage
        {
            get => curr.damage;
        }
        public float attackDuration
        {
            get => curr.attackDuration;
        }
        public float attackCD
        {
            get => curr.attackCD;
        }
        public float attackDistance
        {
            get => curr.attackDistance;
        }
        public float minAttackGap;
        public GameObject target;
        public GameObject attacker;
        private Config curr;
        private Dictionary<string, Config> configs;
        private Animator animator;
        private float lastAttack;

        void Awake()
        {
            animator = GetComponent<Animator>();
            configs = new Dictionary<string, Config>();
        }
        public void SetWeapon(Weapon weapon)
        {
            curr = weapon.combatConf;
        }
        public void ApplyConfig(string name)
        {
            curr = configs[name];
        }
        public void CreateConfig(string name, Config config)
        {
            configs[name] = config;
        }

        public void SuggestTarget(GameObject _target)
        {
            target = _target;
        }
        public bool IsReady(string conf = "")
        {
            if(lastAttack + minAttackGap > Time.time) return false;
            if(conf != "" && configs.ContainsKey(conf)) return configs[conf].IsReady;
            return curr.IsReady;
        }
        public bool CanAttackTarget(string conf = "")
        {
            float distance = target.GetComponent<Collider>().ClosestPointOnBounds(transform.position).PlanerDistance(transform.position);
            if(conf != "" && configs.ContainsKey(conf)) return configs[conf].CanAttackTarget(distance);
            return curr.CanAttackTarget(distance);
        }        
        public bool CanHitTarget(string conf = "")
        {
            float distance = target.GetComponent<Collider>().ClosestPointOnBounds(transform.position).PlanerDistance(transform.position);
            if(conf != "" && configs.ContainsKey(conf)) return configs[conf].CanHitTarget(distance);
            return curr.CanHitTarget(distance);
        }
        public bool IsValidTarget()
        {
            if(!target) return false;
            if(target.GetComponent<Health>().health == 0) return false;
            if(target.transform.position.PlanerDistance(transform.position) > Constants.max_chase_distance) return false;
            return true;
        }
        public bool CanDoAttack(string conf = "")
        {
            if(lastAttack + minAttackGap > Time.time) return false;
            return IsValidTarget() && CanAttackTarget(conf) && IsReady(conf);
        }
        public void ResetAttack(string conf = "")
        {
            if(conf != "" && configs.ContainsKey(conf))
            {
                configs[conf].ResetAttack();
                lastAttack = Time.time;
                return;
            }
            curr.ResetAttack();
            lastAttack = Time.time;
        }

        private float CalcDamage(Combat.Config _conf)
        {
            return 0;
        }
        public void TryDoDamage()
        {
            transform.LookAt(target.transform);
            if(IsValidTarget())
            {
                if(curr.isProjectile) InitProjectile();
                else if(CanHitTarget()) DoDamage();
            } 
            curr.ResetAttack();
        }
        void InitProjectile()
        {
            GameObject go = Instantiate(curr.projectilePrefab, curr.shootPoint.position, curr.shootPoint.rotation);
            Projectile projectile = go.GetComponent<Projectile>();
            projectile.SetTarget(curr, target);
            projectile.onHitTarget += DoDamage;
        }
        public void DoDamage(Combat.Config _conf = null)
        {
            Combat.Config conf = _conf ?? curr;
            if(conf.effect != null) target.GetComponent<EffectHandler>().AddEffect(conf.effect);
            target.GetComponent<EventHandler>().RaiseEvent("GetHit", gameObject, CalcDamage(conf));
        }

        public int CalcRealDamageGet(float damage)
        {
            return (int)damage;
        }
        public void GetHit(GameObject attacker, float damage)
        {
            Health health = GetComponent<Health>();
            health.GetDamage(CalcRealDamageGet(damage));
        }
    }
}