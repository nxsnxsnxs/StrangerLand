using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public class ManagerSingleton<T> : MonoBehaviour where T:ManagerSingleton<T>
    {
        public static T Instance
        {
            get
            {
                if(instance == null) instance = FindObjectOfType<T>();
                return instance;
            }
        }
        private static T instance;
    }
}