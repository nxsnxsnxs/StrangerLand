using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public class Timer : GameComponent
    {
        private Dictionary<string, float> data;

        void Awake()
        {
            data = new Dictionary<string, float>();
        }
        public void SetTimer(string name)
        {
            data[name] = Time.time;
        }
        public float GetTimer(string name)
        {
            if(!data.ContainsKey(name)) data[name] = Time.time;
            return data[name];
        }
    }
}