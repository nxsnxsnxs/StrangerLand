using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class ExtensionVector3
    {
        public static float PlanerDistance(this Vector3 point1, Vector3 point2)
        {
            Vector2 p1 = new Vector2(point1.x, point1.z);
            Vector2 p2 = new Vector2(point2.x, point2.z);
            return Vector2.Distance(p1, p2);
        }
        public static T FindClosestTargetInRange<T>(this Vector3 position, float maxDistance, Predicate<T> filter = null, bool includeInactive = false) where T : MonoBehaviour
        {
            if(filter == null) filter = (_) => { return true; };
            List<T> targets = ToolMethod.FindObjectsWithComponent<T>(includeInactive);
            T closestTarget = null;
            float minDistance = float.MaxValue;
            foreach (var item in targets)
            {
                float temp = item.transform.position.PlanerDistance(position);
                if(temp < minDistance && filter(item))
                {
                    closestTarget = item;
                    minDistance = temp;
                }
            }
            if(minDistance < maxDistance) return closestTarget;
            else return null;
        }
    }
}