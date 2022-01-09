using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public CanvasGroup tooltip;
        public float showAlpha = 1;
        public float showDuration = 0.35f;

        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Utils.FadeIn(tooltip, showAlpha, showDuration));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(Utils.FadeOut(tooltip, 0, showDuration));
        }
    }
}