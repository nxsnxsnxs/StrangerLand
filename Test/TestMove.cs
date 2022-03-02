using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Components;

namespace Test
{
    public class TestMove : MonoBehaviour
    {
        public GameObject target;
        void Start()
        {
            GetComponent<Locomotor>().StartMove(target, OnFinish);
        }
        void OnFinish(bool result)
        {
            Debug.Log("Finish " + result);
        }
    }
}