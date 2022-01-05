using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public static class Utils
    {
        public static IEnumerator FadeIn(CanvasGroup canvasGroup, float alpha, float duration)
        {
            float delta = (alpha - canvasGroup.alpha) / duration;
            while(duration > 0)
            {
                canvasGroup.alpha += delta * Time.deltaTime;
                duration -= Time.deltaTime;
                yield return null;
            }
        }
        public static IEnumerator FadeOut(CanvasGroup canvasGroup, float alpha, float duration)
        {
            float delta = (alpha - canvasGroup.alpha) / duration;
            while(duration > 0)
            {
                canvasGroup.alpha += delta * Time.deltaTime;
                duration -= Time.deltaTime;
                yield return null;
            }
        }
        public static IEnumerator Popup(GameObject panel, float duration)
        {
            panel.transform.localScale = Vector3.zero;
            float delta = 1 / duration;
            while(duration > 0)
            {
                panel.transform.localScale += delta * Vector3.one;
                duration -= Time.deltaTime;
                yield return null;
            }
        }
    }
}