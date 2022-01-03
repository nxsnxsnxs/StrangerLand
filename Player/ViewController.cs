using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player{
    public class ViewController : MonoBehaviour
    {
        public enum ViewType
        {
            thirdPerson
        }
        public Camera thirdPersonCam;
        public Transform model;
        public Camera CurrentViewCam
        {
            get 
            {
                if(viewType == ViewType.thirdPerson) return thirdPersonCam;
                else return null;
            }
        }
        private ViewType viewType;

        void Awake()
        {

        }
        void Start()
        {
            
        }
        void FixedUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                thirdPersonCam.transform.RotateAround(model.position, Vector3.up, 45);
                model.Rotate(0, 45, 0);
            }
            if(Input.GetKeyDown(KeyCode.E))
            {
                thirdPersonCam.transform.RotateAround(model.position, Vector3.up, -45);
                model.Rotate(0, -45, 0);
            }
        }
    }
}
