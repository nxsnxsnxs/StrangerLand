using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class ToolMethod
    {
        //比原版多支持只寻找enable的组件
        public static List<T> FindObjectsWithComponent<T>(bool includeInactive = false, bool includeUnenable = false) where T : MonoBehaviour
        {
            List<T> result = new List<T>();
            T[] temp = GameObject.FindObjectsOfType<T>(includeInactive);
            if(includeUnenable) return new List<T>(temp);
            foreach (var item in temp)
            {
                if(item.enabled) result.Add(item);
            }
            return result;
        }
    }
}