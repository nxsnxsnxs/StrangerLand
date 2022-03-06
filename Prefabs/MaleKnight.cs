using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Tools;
using Actions;
using Events;
using Player;

namespace Prefabs.Player
{
    public class MaleKnight : Character
    {
        public override string bundleName
        {
            get => "MaleKnight";
        }

        public override void DefaultInit()
        {
            base.DefaultInit();
            Locomotor locomotor = gameObject.AddGameComponent<Locomotor>();
            locomotor.moveSpeed = Constants.player_move_speed;
            
            Combat combat = gameObject.AddGameComponent<Combat>();
            Combat.Config conf = new Combat.Config();
            conf.attackDistance = Constants.hand_attack_distance;
            conf.hitDistance = Constants.hand_hit_distance;
            conf.attackDuration = Constants.hand_attack_duration;
            conf.damage = Constants.hand_damage;
            combat.CreateConfig("default", conf);
            combat.ApplyConfig("default");

            Health health = gameObject.AddGameComponent<Health>();
            health.maxHealth = Constants.maleknight_max_health;
            health.health = health.maxHealth;
        }
    }
}