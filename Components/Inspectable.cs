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
        //********need set************
        [HideInInspector]public string inspectStr;
        public Shader rimLightShader;
        //****************************
        private Shader originalShader;
        private bool inInspectMode;
        void Awake()
        {
            Material originalMat = GetComponent<MeshRenderer>().material;
            originalShader = originalMat.shader;
            originalMat.shader = rimLightShader;
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
            GetComponent<MeshRenderer>().material.shader = rimLightShader;
            UIManager.Instance.inspectWindow.InitInspect(this);
        }
        void StopInspect()
        {
            inInspectMode = false;
            GetComponent<MeshRenderer>().material.shader = originalShader;
            UIManager.Instance.inspectWindow.StopInspect();
        }
    }
}