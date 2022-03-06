using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brains;
using Components;
using Actions;
using Tools;
using Player;
using Events;

namespace Prefabs
{
    public class GolemEarth : PrefabComponent
    {
        private GolemEarthBrain brain;
        public override string bundleName
        {
            get => "GolemEarth";
        }

        public override void DefaultInit()
        {
            AddTag(GameTag.creature);
            AddTag(GameTag.neutral);

            EventHandler eventHandler = gameObject.AddComponent<EventHandler>();
            eventHandler.ListenEvent("hasmineraround", OnHasMinerAround);
            ActionController actionController = gameObject.AddComponent<ActionController>();
            actionController.debug = true;

            EffectHandler effectHandler = gameObject.AddGameComponent<EffectHandler>();
            Locomotor locomotor = gameObject.AddGameComponent<Locomotor>();
            locomotor.defaultMoveSpeed = Constants.golem_move_speed;
            locomotor.debug = true;
            RememberLocation rememberLocation = gameObject.AddGameComponent<RememberLocation>();
            rememberLocation.Remember("spawnpoint", transform.position);
            Combat combat = gameObject.AddGameComponent<Combat>();
            InitCombat(combat);
            Health health = gameObject.AddGameComponent<Health>();
            health.maxHealth = Constants.golem_health;
            health.health = health.maxHealth;

            brain = gameObject.AddComponent<GolemEarthBrain>();
            brain.debug = true;
            brain.Begin();
        }
        private void InitCombat(Combat combat)
        {
            Combat.Config defau = new Combat.Config();
            defau.attackCD = Constants.golem_attack_cd;
            defau.attackDistance = Constants.golem_attack_distance;
            defau.hitDistance = Constants.golem_hit_distance;
            defau.attackDuration = Constants.golem_attack_duration;
            defau.damage = Constants.golem_attack_damage;
            Combat.Config spin = new Combat.Config();
            spin.attackCD = Constants.golem_spin_cd;
            spin.attackDistance = Constants.golem_spin_attack_distance;
            spin.hitDistance = Constants.golem_spin_hit_distance;
            spin.attackDuration = Constants.golem_spin_attack_duration;
            spin.damage = Constants.golem_spin_attack_damage;
            combat.CreateConfig("default", defau);
            combat.CreateConfig("spinattack", spin);
            combat.minAttackGap = Constants.normal_min_attack_gap;
        }
        private void OnHasMinerAround(params object[] args)
        {
            if(GetComponent<Combat>().IsValidTarget()) return;
            GameObject target = args[0] as GameObject;
            gameObject.GetComponent<Combat>().target = target;
        }
    }
}

