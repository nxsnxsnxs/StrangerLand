using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Weapon : GameComponent
    {
        public int damage = Constants.hand_damage;
        public float attackDistance = Constants.default_attack_distance;
        public float attackDuration = Constants.hand_attack_duration;
    }
}