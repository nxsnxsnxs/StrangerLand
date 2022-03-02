using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Actions;
using Events;
using Brains;
using Tools;

namespace Prefabs
{
    public class ScorpionKing : PrefabComponent
    {
        private ScorpionKingBrain brain;
        public override string bundleName
        {
            get => "ScorpionKing";
        }

        public override void DefaultInit()
        {
            AddTag(GameTag.creature);
            AddTag(GameTag.evil);
            
            EventHandler eventHandler = gameObject.AddComponent<EventHandler>();
            ActionController actionController = gameObject.AddComponent<ActionController>();
            //actionController.debug = true;

            Locomotor locomotor = gameObject.AddGameComponent<Locomotor>();
            locomotor.defaultMoveSpeed = Constants.scorpionking_move_speed;
            //locomotor.debug = true;
            RememberLocation rememberLocation = gameObject.AddGameComponent<RememberLocation>();
            rememberLocation.Remember("spawnpoint", transform.position);
            Timer timer = gameObject.AddGameComponent<Timer>();
            Combat combat = gameObject.AddGameComponent<Combat>();
            InitCombat(combat);
            Commander commander = gameObject.AddGameComponent<Commander>();
            Health health = gameObject.AddGameComponent<Health>();
            health.maxHealth = Constants.scorpionking_health;
            health.health = health.maxHealth;

            brain = gameObject.AddComponent<ScorpionKingBrain>();
            //brain.debug = true;
            brain.Begin();
        }
        private void InitCombat(Combat combat)
        {
            Combat.Config defau = new Combat.Config();
            defau.attackCD = Constants.scorpionking_attack_cd;
            defau.attackDistance = Constants.scorpionking_attack_distance;
            defau.hitDistance = Constants.scorpionking_hit_distance;
            defau.attackDuration = Constants.scorpionking_attack_duration;
            defau.damage = Constants.scorpionking_attack_damage;
            Combat.Config tail = new Combat.Config();
            tail.attackCD = Constants.scorpionking_tail_attack_cd;
            tail.attackDistance = Constants.scorpionking_tail_attack_distance;
            tail.hitDistance = Constants.scorpionking_tail_hit_distance;
            tail.attackDuration = Constants.scorpionking_tail_attack_duration;
            tail.damage = Constants.scorpionking_tail_attack_damage;
            var poison = new CommonEffects.Poisoning();
            poison.damage = 5;
            poison.duration = 3;
            poison.gapTime = 1;
            tail.effect = poison;
            
            combat.CreateConfig("default", defau);
            combat.CreateConfig("tailattack", tail);
            combat.minAttackGap = Constants.normal_min_attack_gap;
        }
    }
}