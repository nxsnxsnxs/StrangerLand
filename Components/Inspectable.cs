using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Components
{
    public enum InspectType
    {
        Hover, LeftClick, RightClick
    }
    public class Inspectable : GameComponent
    {
        [HideInInspector]public string inspectStr;
        public Material rimLightMat;
        private Material originalMat;
        private bool inInspectMode;
        void Awake()
        {
            originalMat = GetComponent<MeshRenderer>().material;
        }

        void Update()
        {
            if(inInspectMode)
            {
                Ray ray = FindObjectOfType<ViewController>().CurrentViewCam.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if(!Physics.Raycast(ray, out raycastHit) || raycastHit.collider != GetComponent<Collider>()) StopInspect();
            }
        }
        public void Inspect()
        {
            if(inInspectMode) return;
            inInspectMode = true;
            GetComponent<MeshRenderer>().material = rimLightMat;
            UIManager.Instance.inspectWindow.InitInspect(this);
        }
        void StopInspect()
        {
            inInspectMode = false;
            GetComponent<MeshRenderer>().material = originalMat;
            UIManager.Instance.inspectWindow.StopInspect();
        }
    }
}