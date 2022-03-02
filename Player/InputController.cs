using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Components;
using Actions;
using Player.Actions;
using Events;

namespace Player
{
    public class InputController : MonoBehaviour
    {
        public Material rimLightMat;
        private ViewController viewController;
        private EventHandler eventHandler;
        private ActionController actionController;

        void Init()
        {
            viewController = GetComponent<ViewController>();
            eventHandler = GetComponent<EventHandler>();
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
                if(raycastHit.collider.GetComponentInParent<Combat>())
                {
                    if(Input.GetMouseButtonDown(0)) eventHandler.RaiseEvent("Attack", raycastHit.collider.GetComponentInParent<Combat>());
                }
                else if(raycastHit.collider.GetComponentInParent<Pickable>())
                {
                    if(Input.GetMouseButtonDown(0)) eventHandler.RaiseEvent("Pick", raycastHit.collider.GetComponentInParent<Pickable>());
                }
                else if(raycastHit.collider.GetComponentInParent<Workable>())
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        eventHandler.RaiseEvent("Work", raycastHit.collider.GetComponentInParent<Workable>());
                    }
                }
                else if(raycastHit.collider.CompareTag("Ground"))
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        eventHandler.RaiseEvent("Move", raycastHit.point);
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
            if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !actionController.isDoing<LocomotionController>()) eventHandler.RaiseEvent("Move");
            else if(Input.GetKey(KeyCode.Space)) eventHandler.RaiseEvent("AutoCraft");
            else if(Input.GetKey(KeyCode.F)) eventHandler.RaiseEvent("AutoAttack");
        }
    }
}