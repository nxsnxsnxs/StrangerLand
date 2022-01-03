using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public float hoverAlpha = 0.5f;
        public float hoverDurantion = 0.35f;
        public UnityEvent OnButtonClick;
        private CanvasGroup canvasGroup;
        
        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Utils.FadeOut(canvasGroup, hoverAlpha, hoverDurantion));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Utils.FadeIn(canvasGroup, 1, hoverDurantion));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            StopAllCoroutines();
            canvasGroup.alpha = 1;
            if(OnButtonClick != null) OnButtonClick.Invoke();
        }
    }
}