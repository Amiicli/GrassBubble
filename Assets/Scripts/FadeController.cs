using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Milan.GrassBubble
{
    public class FadeController : MonoBehaviour
    {
        public enum FadeColor
        {
            Clear = 0,
            White = 1,
            Black = 2
        }
        public enum FadeType
        {
            EaseInOutCubic = 0,
            FadeInOutBack = 1,
            EaseInOutQuad = 2,
            EaseOutQuart = 3,
            EaseOutSine = 4,
            EaseInQuad = 5
        }
        public Action Callback;
        delegate float Ease(float x);
        public Image image;
        public CanvasGroup canvasGroup;
        // float debugFadeLength = 2;
        // public bool debugMode = false;
        // Start is called before the first frame update
        void Start()
        {
            
        }
        public static void KillAllFades()
        {
            FadeController fadeController = GameObject.FindGameObjectWithTag("FadeController").GetComponent<FadeController>();
            fadeController.KillAllFadesInternal();
        }
        void KillAllFadesInternal()
        {
            StopAllCoroutines();
        }

        public static void Fade(FadeColor fadeFrom,FadeColor fadeTo,FadeType fadeType, float fadeDuration,Action callback)
        {
            FadeController fadeController = GameObject.FindGameObjectWithTag("FadeController").GetComponent<FadeController>();
            fadeController.InternalFade(fadeFrom,fadeTo,fadeType,fadeDuration, callback);
        }
        public static void Fade(FadeColor fadeFrom,FadeColor fadeTo,FadeType fadeType, float fadeDuration)
        {
            FadeController fadeController = GameObject.FindGameObjectWithTag("FadeController").GetComponent<FadeController>();
            fadeController.InternalFade(fadeFrom,fadeTo,fadeType,fadeDuration,()=>{});
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void InternalFade(FadeColor fadeFrom,FadeColor fadeTo,FadeType fadeType, float fadeDuration, Action callback)
        {
            StopAllCoroutines();
            Color fromColor = GetColorFromEnum(fadeFrom);
            Color toColor = GetColorFromEnum(fadeTo);
            Ease easeFunction = GetDelegateFromEnum(fadeType);
            Debug.Log("From: " + fromColor);
            Debug.Log("To: " + fromColor);
            Debug.Log("Ease: " + fadeType);
            Debug.Log("Duration: " + fadeDuration);
            StartCoroutine(FadeTransition(fromColor,toColor,easeFunction,fadeDuration,callback));
        }
        Color GetColorFromEnum(FadeColor fadeColor)
        {
            Color returnCol;
            switch (fadeColor)
            {   
                case FadeColor.Clear:
                    returnCol = Color.clear;
                break;
                case FadeColor.Black:
                    returnCol = Color.black;
                break;
                case FadeColor.White:
                    returnCol = Color.white;
                break;
                default:
                returnCol = Color.magenta;
                break;
            }
            return returnCol;
        }
        Ease GetDelegateFromEnum(FadeType fadeType)
        {
            Ease returnFunc;
            switch (fadeType)
            {   
                case FadeType.EaseInOutCubic:
                    returnFunc = EasingUtil.EaseInOutCubic;
                break;
                case FadeType.FadeInOutBack:
                    returnFunc = EasingUtil.EaseInOutBack;
                break;
                case FadeType.EaseInOutQuad:
                    returnFunc = EasingUtil.EaseInOutQuad;
                break;
                case FadeType.EaseOutSine:
                    returnFunc = EasingUtil.EaseOutSine;
                break;
                case FadeType.EaseOutQuart:
                    returnFunc = EasingUtil.EaseOutQuart;
                break;
                case FadeType.EaseInQuad:
                    returnFunc = EasingUtil.EaseInQuad;
                break;
                default:
                    returnFunc = EasingUtil.EaseInOutCubic;
                break;
            }
            return returnFunc;
        }
        IEnumerator FadeTransition(Color fromCol,Color toCol,Ease easeFunc, float fadeDuration, Action callback)
        {
            float t = 0;
            //Lerping white and clear does not work well (it goes to a grayish color inbetween for obvious reasons),
            //We'll have to do this little hack to make it look proper
            if(toCol == Color.white && fromCol == Color.clear)
                fromCol = new Color(1,1,1,0);
            else if (fromCol == Color.white && toCol == Color.clear)
                toCol = new Color(1,1,1,0);
            
            while(t < fadeDuration)
            {
                float finalPercent = Mathf.Clamp01(t / fadeDuration);
                float curvePercent = easeFunc(finalPercent);
                image.color = Color.Lerp(fromCol,toCol,curvePercent);
                yield return null;
                t += Time.deltaTime;
            }
            image.color = toCol;
            callback();
        }
        // void OnGUI() 
        // {
        //     if(!debugMode)
        //         return;
        //     GUILayout.BeginArea(new Rect(20,0,200,1000));
        //         GUILayout.BeginVertical();
        //             GUILayout.BeginHorizontal();
        //                 GUILayout.Label("Fade Length");
        //                 debugFadeLength = GUILayout.HorizontalSlider(debugFadeLength,0.1f,4);
        //             GUILayout.EndHorizontal();
        //             if(GUILayout.Button("Black To White"))
        //             {
        //                 Fade(FadeColor.Black,FadeColor.White,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
        //             if(GUILayout.Button("Black To Clear"))
        //             {
        //                 Fade(FadeColor.Black,FadeColor.Clear,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
        //             if(GUILayout.Button("White To Black"))
        //             {
        //                 Fade(FadeColor.White,FadeColor.Black,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
        //             if(GUILayout.Button("White To Clear"))
        //             {
        //                 Fade(FadeColor.White,FadeColor.Clear,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
        //             if(GUILayout.Button("Clear To White"))
        //             {
        //                 Fade(FadeColor.Clear,FadeColor.White,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
        //             if(GUILayout.Button("Clear To Black"))
        //             {
        //                 Fade(FadeColor.Clear,FadeColor.Black,FadeType.EaseInOutQuad,debugFadeLength);
        //             }
                
        //         GUILayout.EndVertical();
        //     GUILayout.EndArea();
        // }
    }
}