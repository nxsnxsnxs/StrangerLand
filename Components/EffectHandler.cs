using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public abstract class BaseEffect
    {
        //*********need set**********
        public float duration;
        public float gapTime;
        //***************************
        public float lastDone;
        private float beginTime;
        public abstract string name{get;}
        public abstract void DoEffect(GameObject gameObject);
    }    
    public class EffectHandler : GameComponent
    {
        public List<BaseEffect> effects;

        void Awake()
        {
            effects = new List<BaseEffect>();
        }
        public void AddEffect(BaseEffect effect)
        {
            effects.Add(effect);
        }

        void FixedUpdate()
        {
            foreach (var effect in effects)
            {
                if(effect.lastDone + effect.gapTime <= Time.time)
                {
                    effect.DoEffect(gameObject);
                    effect.lastDone = Time.time;
                }
            }
        }
    }
}