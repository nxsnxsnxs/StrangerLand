using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Prefabs
{
    using Components;
    public class JusticeSword : PrefabComponent
    {
        public override string bundleName
        {
            get => "JusticeSword";
        }

        public override void DefaultInit()
        {
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            Combat.Config conf = new Combat.Config();
            conf.damage = Constants.justice_sword_damage;
            conf.attackDuration = Constants.sword_attack_duration;
            conf.attackDistance = Constants.sword_attack_distance;
            conf.hitDistance = Constants.sword_hit_distance;
            weapon.combatConf = conf;
            
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxUse = 90;
            finiteUse.currUse = 90;
        }

        void Awake()
        {

        }
    }
}