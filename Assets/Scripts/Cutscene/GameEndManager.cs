using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Milan.GrassBubble;

namespace Milan.GrassBubble.Cutscene
{
    public class GameEndManager : MonoBehaviour
    {
        const float finalMessageAppearTime = 3;
        [SerializeField]
        TextFadeManager textFadeManager;
        [SerializeField,Range(0,5)]
        public float waitTime;
        [SerializeField]
        CanvasGroup finalMessage;
        // Start is called before the first frame update
        void Start()
        {
            finalMessage.alpha = 0;
            TextFadeManager.TextFadeCompleted += OnTextFadeFinish;
            textFadeManager.Initialize(waitTime);
        }

        void OnDisable() 
        {
            TextFadeManager.TextFadeCompleted -= OnTextFadeFinish;
        }
        // Update is called once per frame
        void OnTextFadeFinish()
        {
            TextFadeManager.TextFadeCompleted -= OnTextFadeFinish;
            textFadeManager.HideAll(finalMessageAppearTime);
            StartCoroutine(FinalMessage());
        }
        IEnumerator FinalMessage()
        {
            yield return new WaitForSeconds(finalMessageAppearTime);
            float t = 0;

            while (t < 3)
            {
                float finalPercent = Mathf.Clamp01(t / 3);
                float curvePercentage = EasingUtil.EaseInOutQuad(finalPercent);
                finalMessage.alpha = Mathf.Lerp(0,1,curvePercentage);
                t += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(8);
            FadeController.Fade(FadeController.FadeColor.Clear,FadeController.FadeColor.White,FadeController.FadeType.EaseOutSine,3,ReturnToMainMenu);
        }
        void ReturnToMainMenu()
        {
            SceneManager.LoadScene(Global.Scenes.MainMenu);
        }
    }
}