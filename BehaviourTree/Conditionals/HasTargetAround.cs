using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace MyBehaviourTree.Conditions
{
    public class HasTargetAround : Conditional
    {
        public HasTargetAround(BehaviourTree _bt, string targetTag, float checkRadius)
            :
            base(_bt, 
            (bt) => {
                List<GameObject> results = new List<GameObject>(GameObject.FindGameObjectsWithTag(targetTag));
                if(results == null || results.Count == 0) return false;
                
                results.Sort(
                    (GameObject a, GameObject b) => {
                        float distanceA = a.transform.position.PlanerDistance(bt.go.transform.position);
                        float distanceB = b.transform.position.PlanerDistance(bt.go.transform.position);
                        if(distanceA < distanceB) return -1;
                        else if(distanceA == distanceB) return 0;
                        else return 1;
                    }
                );
                if(results[0].transform.position.PlanerDistance(bt.go.transform.position) <= checkRadius)
                {
                    bt.SetValue("target", results[0]);
                    return true;
                }
                return false;                
            })
        {}
        public HasTargetAround(BehaviourTree _bt, Type targetType , float checkRadius)
            :
            base(_bt,
                (bt) => {
                List<GameObject> results = new List<GameObject>((GameObject[])GameObject.FindObjectsOfType(targetType));
                if(results == null || results.Count == 0) return false;
                results.Sort(
                    (GameObject a, GameObject b) => {
                        float distanceA = a.transform.position.PlanerDistance(bt.go.transform.position);
                        float distanceB = b.transform.position.PlanerDistance(bt.go.transform.position);
                        if(distanceA < distanceB) return -1;
                        else if(distanceA == distanceB) return 0;
                        else return 1;
                    }
                );
                if(results[0].transform.position.PlanerDistance(bt.go.transform.position) <= checkRadius)
                {
                    bt.SetValue("target", results[0]);
                    return true;
                }
                return false;                
            })
        {}
    }
}