using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Tools
{
    public static class ExtentionGameobject
    {
        public static T AddGameComponent<T>(this GameObject gameObject) where T : GameComponent
        {
            T temp = gameObject.AddComponent<T>();
            return temp;
        }
        public static T GetEnabledComponent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            T temp = gameObject.GetComponent<T>();
            if(temp == null || !temp.enabled) return null;
            return temp;
        }
    }
}