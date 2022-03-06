using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Actions;
using Components;
using Brains;
using Tools;

namespace Prefabs
{
    public class Scorpion : PrefabComponent
    {
        private ScorpionBrain brain;
        public override string bundleName
        {
            get => "Scorpion";
        }

        public override void DefaultInit()
        {
            AddTag(GameTag.creature);
            AddTag(GameTag.evil);

            EventHandler eventHandler = gameObject.AddComponent<EventHandler>();
            eventHandler.ListenEvent("setcommander", OnGotKing);
            ActionController actionController = gameObject.AddComponent<ActionController>();
            //actionController.debug = true;

            Locomotor locomotor = gameObject.AddGameComponent<Locomotor>();
            locomotor.defaultMoveSpeed = Constants.scorpion_move_speed;
            //locomotor.debug = true;
            Combat combat = gameObject.AddGameComponent<Combat>();
            InitCombat(combat);
            TargetTracker targetTracker = gameObject.AddGameComponent<TargetTracker>();
            Health health = gameObject.AddGameComponent<Health>();
            health.maxHealth = Constants.scorpion_health;
            health.health = health.maxHealth;

            brain = gameObject.AddComponent<ScorpionBrain>();
            //brain.debug = true;
            brain.Begin();
        }
        void InitCombat(Combat combat)
        {
            Combat.Config defau = new Combat.Config();
            defau.attackCD = Constants.scorpion_attack_cd;
            defau.attackDistance = Constants.scorpion_attack_distance;
            defau.hitDistance = Constants.scorpion_hit_distance;
            defau.attackDuration = Constants.scorpion_attack_duration;
            defau.damage = Constants.scorpion_attack_damage;
            Combat.Config telsonattack = new Combat.Config();
            telsonattack.isProjectile = true;
            telsonattack.projectilePrefab = ABManager.Instance.LoadAsset<GameObject>(bundleName, "Telson");
            telsonattack.shootPoint = transform.Find("TelsonShootPoint");
            telsonattack.flySpeed = Constants.normal_projectile_fly_speed;
            telsonattack.maxFlyDistance = Constants.normal_projectile_fly_maxdistance;
            telsonattack.attackCD = Constants.scorpion_telson_attack_cd;
            telsonattack.attackDistance = Constants.scorpion_telson_attack_distance;
            telsonattack.attackDuration = Constants.scorpion_telson_attack_duration;
            telsonattack.damage = Constants.scorpion_telson_attack_damage;
            var poison = new CommonEffects.Poisoning();
            poison.damage = 10;
            poison.duration = 4;
            poison.gapTime = 1;
            telsonattack.effect = poison;
            
            combat.CreateConfig("default", defau);
            combat.CreateConfig("telsonattack", telsonattack);
            combat.minAttackGap = Constants.normal_min_attack_gap;
        }
        private void OnGotKing(params object[] args)
        {
            GetComponent<TargetTracker>().TrackTarget("king", args[0] as GameObject);
        }
    }
}