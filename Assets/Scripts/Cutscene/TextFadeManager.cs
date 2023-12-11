using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Milan.GrassBubble.Cutscene
{
    public class TextFadeManager : MonoBehaviour
    {
        public delegate void Notify();
        public static event Notify TextFadeCompleted;
        public FadeText[] fadeTexts;

        void Start() 
        {
            foreach (FadeText ƒadeText in fadeTexts)
            {
                Color ogColor = ƒadeText.text.color;
                ƒadeText.text.color = new Color(ogColor.r,ogColor.g,ogColor.b,0);
            }    
        }
        public void Initialize(float waitTime)
        {
            StartCoroutine(FadeOperation(waitTime));
        }
        IEnumerator FadeOperation(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            foreach (FadeText fadeText in fadeTexts)
            {
                float t = 0;
                float duration = fadeText.fadeDuration;
                Color oldColor = new Color(fadeText.text.color.r,fadeText.text.color.g,fadeText.text.color.b,0);
                Color newColor = new Color(fadeText.text.color.r,fadeText.text.color.g,fadeText.text.color.b,1);
                while (t < duration)
                {
                    // newDistance += Time.deltaTime * 2;
                    float finalPercent = Mathf.Clamp01(t / duration);
                    float curvePercentage = EasingUtil.EaseInOutQuad(finalPercent);

                    fadeText.text.color = Color.Lerp(oldColor,newColor,curvePercentage);

                    yield return null;
                    t += Time.deltaTime;

                }
                yield return new WaitForSeconds(fadeText.waitDuration);
            }
            TextFadeCompleted?.Invoke();
        }
        public void HideAll(float waitTime)
        {
            StartCoroutine(HideAllOperation(waitTime));
        }
        IEnumerator HideAllOperation(float fadeTime)
        {
            float t = 0;
            Color oldColor = new Color(fadeTexts[0].text.color.r,fadeTexts[0].text.color.g,fadeTexts[0].text.color.b,1);
            Color newColor = new Color(fadeTexts[0].text.color.r,fadeTexts[0].text.color.g,fadeTexts[0].text.color.b,0);
            while (t < fadeTime)
            {
                foreach (FadeText fadeText in fadeTexts)
                {
                    // newDistance += Time.deltaTime * 2;
                    float finalPercent = Mathf.Clamp01(t / fadeTime);
                    float curvePercentage = EasingUtil.EaseInOutQuad(finalPercent);

                    fadeText.text.color = Color.Lerp(oldColor,newColor,curvePercentage);
                }
                yield return null;
                t += Time.deltaTime;
            }
            TextFadeCompleted?.Invoke();
        }
        [Serializable]
        public struct FadeText
        {
            public TMP_Text text;
            public float fadeDuration;
            public float waitDuration;
        }
    }
}