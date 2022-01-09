using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Components;
using Player.Action;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        public Material rimLightMat;
        private ViewController viewController;
        private LocomotionController locomotionController;
        private AttackController attackController;
        private ActionController actionController;

        void Init()
        {
            viewController = GetComponent<ViewController>();
            locomotionController = GetComponent<LocomotionController>();
            attackController = GetComponent<AttackController>();
            actionController = GetComponent<ActionController>();
        }
        void Awake()
        {
            Init();
        }

        void FixedUpdate()
        {
            MouseUpdate();
            KeyboardUpdate();
        }
        void MouseUpdate()
        {
            if(EventSystem.current.IsPointerOverGameObject()) return;
            Ray ray = viewController.CurrentViewCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if(Physics.Raycast(ray, out raycastHit))
            {
                if(raycastHit.collider.CompareTag("Evil") && raycastHit.collider.GetComponentInParent<Attackable>())
                {
                    if(Input.GetMouseButtonDown(0)) actionController.DoAction<AttackController>(true, raycastHit.collider.GetComponentInParent<Attackable>());
                }
                else if(raycastHit.collider.GetComponentInParent<Pickable>())
                {
                    if(Input.GetMouseButtonDown(0)) actionController.DoAction<CraftController>(true, raycastHit.collider.GetComponentInParent<Pickable>());
                }
                else if(raycastHit.collider.GetComponentInParent<Workable>())
                {
                    if(Input.GetMouseButtonDown(0)) actionController.DoAction<CraftController>(true, raycastHit.collider.GetComponentInParent<Workable>());
                }
                else if(raycastHit.collider.CompareTag("Ground"))
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        actionController.DoAction<LocomotionController>(true, raycastHit.point);
                    }
                }                
                //if(raycastHit.collider.CompareTag("Building"))
                {
                    
                }

                if(raycastHit.collider.GetComponent<Inspectable>()) raycastHit.collider.GetComponent<Inspectable>().Inspect();
            }
        }
        void KeyboardUpdate()
        {
            if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) actionController.DoAction<LocomotionController>(false);
            else if(Input.GetKey(KeyCode.Space)) actionController.DoAction<CraftController>(false);
            else if(Input.GetKey(KeyCode.F)) actionController.DoAction<AttackController>(false);
        }
    }
}