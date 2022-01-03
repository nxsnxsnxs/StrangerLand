using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class LocomotionController1 : MonoBehaviour
    {
        public float runSpeed;
        public float rotateSpeed;

        private Animator animator;

        void Awake()
        {
            Application.targetFrameRate = 60;
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            
        }
        void FixedUpdate()
        {
            if(Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * Time.deltaTime * runSpeed;
                animator.SetBool("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
            } 
            if(Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, - rotateSpeed * Time.deltaTime, 0, Space.Self);
            }
            if(Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.Self);
            }
        }
    }
}