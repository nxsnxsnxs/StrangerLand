using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Prefabs
{
    using Components;
    public class JusticeSword : PrefabComponent
    {
        public override string loadPath
        {
            get => "JusticeSword";
        }

        public override void DefaultInit()
        {
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            weapon.damage = Constants.justice_sword_damage;
            weapon.attackDuration = Constants.sword_attack_duration;
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxDurability = 100;
            finiteUse.durability = 100;            
        }

        void Awake()
        {

        }
    }
}