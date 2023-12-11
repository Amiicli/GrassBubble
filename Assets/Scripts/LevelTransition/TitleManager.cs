using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Milan.GrassBubble.LevelTransition
{
    public class TitleManager : MonoBehaviour
    {
        int currentLevel;
        public Sprite[] levelTitles;
        public Image titleImage;
        public Image titleToSwitchTo;
        public Image x;
        public Image grassHopperBubble;
        [Range(0.1f,2)]
        public float swapAnimDuration;
        Vector3 originalScale;
        public AnimationCurve animationCurve;
        public AnimationCurve bubbleAnim;
        public Image[] numberBubbles;
        public Image levelNumberIndicator;

        void Awake() 
        { 
            originalScale = transform.localScale;
        }

        public void PreInitialize(LevelData levelData, int index)
        {
            ChangeLevelNumberIndicatorNum(index);
            titleImage.sprite = levelTitles[index];
        }
        public void Initialize(LevelData levelData, int index)
        {
            currentLevel = index;
            titleToSwitchTo.sprite = levelTitles[index];
            PlayTitleSwapAnim();
            StartCoroutine(BasicImagePopIn(x));
            StartCoroutine(BasicImagePopIn(grassHopperBubble));
            ChangeLevelNumberIndicatorNum(index);
            MakeNumberBubblesAppear(levelData);
        }

        public void PlayTitleSwapAnim()
        {
            StartCoroutine(HideOldTitle());
            StartCoroutine(ShowNewTitle());
        }
        void ChangeLevelNumberIndicatorNum(int levelNum)
        {
            levelNumberIndicator.material.SetFloat("_ManualIndex",levelNum + 1);
        }
        void MakeNumberBubblesAppear(LevelData levelData)
        {
            float delayNum = 0.75f;
            int grassHopperCount = (int)(levelData.grassHopperCount * Global.GetGrassHopperMultiplier());
            string stringCount = grassHopperCount.ToString("000000");
            
            for (int i = 0; i < stringCount.Length; i++)
            {
                int value = stringCount[i] - '0';
                StartCoroutine(ShowBubbleCount(numberBubbles[i],value,delayNum / (i + 1)));
            }

        }
        IEnumerator BasicImagePopIn(Image image)
        {
            float duration = 1f;
            float t = 0;
            Vector2 originalPosition = image.rectTransform.position;
            Vector2 originalPopInImageScale = image.rectTransform.localScale;
            image.rectTransform.localScale = Vector3.zero;
            image.gameObject.SetActive(true);
            while(t < duration)
            {
                float finalPercent = Mathf.Clamp01(t / swapAnimDuration);
                float curvePercent = EasingUtil.EaseOutQuart(finalPercent);

                // float newPosY = originalPosition.y + ((Mathf.Sin(t) * 500) * (1 - curvePercent));
                Vector3 newScale = Vector3.Lerp(Vector3.zero,originalPopInImageScale,curvePercent);
                image.rectTransform.localScale = newScale;
                // image.rectTransform.position = new Vector2(originalPosition.x,newPosY);
                yield return null;
                t += Time.deltaTime;
            }
        }
        IEnumerator ShowBubbleCount(Image image,int value,float delay)
        {
            yield return new WaitForSeconds(delay);
            float growDuration = 0.2f;
            Vector3 originalScale = image.rectTransform.localScale;
            Image child = image.transform.GetChild(0).GetComponent<Image>();
            image.rectTransform.localScale = Vector3.zero;
            image.gameObject.SetActive(true);

            float t = 0;
            while(t < growDuration)
            {
                float finalPercent = Mathf.Clamp01(t / growDuration);
                float curvePercent = EasingUtil.EaseInOutBack(finalPercent);
                Vector3 newScale = Vector3.Lerp(Vector3.zero,originalScale,curvePercent);
                if(finalPercent > 0.5)
                {
                    child.material.SetFloat("_ManualIndex",value);
                }
                yield return null;
                image.rectTransform.localScale = newScale;
                t += Time.deltaTime;
            }
            float duration = 3;
            Vector2 originalPosition = image.rectTransform.position;
            t = 0;
            while(t < duration)
            {
                float finalPercent = Mathf.Clamp01(t / swapAnimDuration);
                float curvePercent = EasingUtil.EaseOutQuart(finalPercent);

                float newPosY = originalPosition.y + ((Mathf.Sin(t) * 200) * (1 - curvePercent));
                image.rectTransform.position = new Vector2(originalPosition.x,newPosY);
                yield return null;
                t += Time.deltaTime;
            }
        }

        IEnumerator HideOldTitle()
        {   
            float t = swapAnimDuration;
            while (t > 0)
            {
                float finalPercent = Mathf.Clamp01(t / swapAnimDuration);
                float curvePercent = EasingUtil.EaseInOutQuad(finalPercent);
                titleImage.fillAmount = Mathf.Clamp01(curvePercent);
                yield return null;
                t -= Time.deltaTime;
            }   
            titleImage.fillAmount = 0;
            t = 1f;
        }
        IEnumerator ShowNewTitle()
        {   
            float t = 0;
            while (t < swapAnimDuration)
            {
                float finalPercent = Mathf.Clamp01(t / swapAnimDuration);
                float curvePercent = EasingUtil.EaseInOutQuad(finalPercent);
                titleToSwitchTo.fillAmount = Mathf.Clamp01(curvePercent);
                yield return null;
                t += Time.deltaTime;
            }   
            titleToSwitchTo.fillAmount = 1;
            t = 1f;
        }
    }
}