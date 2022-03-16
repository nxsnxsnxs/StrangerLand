using System.Diagnostics;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Components;

namespace UI
{
    public class InspectWindow : MonoBehaviour
    {
        [HideInInspector]public Inspectable current;
        Text inspectText;
        RectTransform rectTransform;

        void Awake()
        {
            inspectText = GetComponentInChildren<Text>();
            rectTransform = GetComponent<RectTransform>();  
        }

        void Update()
        {
            if(!current) return;
            rectTransform.position = Input.mousePosition + new Vector3(0, rectTransform.sizeDelta.y * 0.6f, 0);
            /*rectTransform.sizeDelta = new Vector2(
                Mathf.Max(inspectText.GetComponent<RectTransform>().sizeDelta.x * 1.1f, minWindow.x),
                Mathf.Max(inspectText.GetComponent<RectTransform>().sizeDelta.y * 1.2f, minWindow.y)
            );*/
        }

        public void InitInspect(Inspectable target)
        {
            current = target;
            inspectText.text = current.inspectStr;
            GetComponent<VerticalLayoutGroup>().enabled = true;
            StartCoroutine(ShowInspect());
        }
        private IEnumerator ShowInspect()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            GetComponentInParent<CanvasGroup>().alpha = 1;
        }

        public void StopInspect()
        {
            current = null;
            GetComponentInParent<CanvasGroup>().alpha = 0;
            GetComponent<VerticalLayoutGroup>().enabled = false;
        }
    }
}