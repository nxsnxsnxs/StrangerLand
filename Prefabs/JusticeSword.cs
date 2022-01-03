using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Prefab
{
    using Components;
    public class JusticeSword : MonoBehaviour
    {
        void Awake()
        {
            Weapon weapon = gameObject.AddGameComponent<Weapon>();
            weapon.damage = Constants.justice_sword_damage;
            weapon.attackDuration = Constants.sword_attack_duration;
            FiniteUse finiteUse = gameObject.AddGameComponent<FiniteUse>();
            finiteUse.maxDurability = 100;
            finiteUse.durability = 100;
        }
    }
}