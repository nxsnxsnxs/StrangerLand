using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    public abstract class GameComponent : MonoBehaviour
    {
        public bool Active
        {
            get => active;
        }
        private bool active;
        public void SetActive()
        {
            active = true;
        }
        public void SetInactive()
        {
            active = false;
        }
        public virtual void InitData(Dictionary<string, object> data)
        {

        }
        public virtual void SaveData(Dictionary<string, object> data)
        {

        }
    }
}