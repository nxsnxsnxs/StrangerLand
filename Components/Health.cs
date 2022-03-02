using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Components
{
    public class Health : GameComponent
    {
        //********need set************
        public int maxHealth;
        public int health;
        //****************************
        public Action<GameObject> onDie;
        public void GetDamage(int damage)
        {
            health -= damage;
            if(health < 0)
            {
                health = 0;
                onDie?.Invoke(gameObject);
            }
        }
    }
}