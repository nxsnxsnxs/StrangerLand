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
        public static int TwoEightRoundToInt(float num)
        {
            float temp = Mathf.Floor(num);
            return (int)(num - temp >= 0.2f ? temp + 1 : temp);
        }
        public static int EightTwoRoundToInt(float num)
        {
            float temp = Mathf.Floor(num);
            return (int)(num - temp >= 0.8f ? temp + 1 : temp);
        }
        public static Vector3 GetRandomPosInRange(Vector3 center, float minRadius, float maxRadius)
        {
            Vector3 result = Vector3.zero;
            result.x = Random.value > 0.5f ? Random.Range(center.x + minRadius, center.x + maxRadius) : Random.Range(center.x - maxRadius, center.x - minRadius);
            result.z = Random.value > 0.5f ? Random.Range(center.z + minRadius, center.z + maxRadius) : Random.Range(center.z - maxRadius, center.z - minRadius);
            return result;
        }
        public static T GetRandomChoice<T>(T[] choices, float[] weight)
        {
            if(choices == null || weight == null || choices.Length != weight.Length) return default;
            float val = UnityEngine.Random.value;
            for (int i = 0; i < weight.Length; i++)
            {
                if(val <= weight[i]) return choices[i];
            }
            return default;
        }
    }
}