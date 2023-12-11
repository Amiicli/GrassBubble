using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Milan.GrassBubble.Gameloop
{
    public class LevelTimer : MonoBehaviour
    {
        readonly int indexID = Shader.PropertyToID("_ManualIndex");
        public delegate void Notify();
        public static event Notify TimerOver;
        [SerializeField]
        float time;
        [SerializeField]
        Color warningRed;
        [SerializeField]
        bool isActive = false;
        [SerializeField]
        Image[] images;
        [SerializeField]
        Image timeRemainingNoko;
        [SerializeField]
        Image timeRemainingRi;
        [SerializeField]
        Image timeRemainingSeconds;
        [SerializeField]
        GameObject infinityIndicator;
        [SerializeField]
        GameObject timerIndicator;
        float alertTime;
        bool hasAlertStarted = false;

        public void DisplayInfiniteTime()
        {
            timerIndicator.SetActive(false);
            infinityIndicator.SetActive(true);
        }
        public void Initialize(LevelData data)
        {
            time = data.time;
            alertTime = time / 5;
            UpdateTimer();
        }
        public void StartTimer()
        {
            isActive = true;
        }
        // Update is called once per frame
        void Update()
        {
            if(isActive)
            {
                time -= Time.deltaTime;
                UpdateTimer();
                if(time <= 0 )
                {
                    isActive = false;
                    time = 0;
                    TimerOver?.Invoke();
                }
                if(alertTime > time && hasAlertStarted == false)
                {
                    hasAlertStarted = true;
                    SetTimerToRed();
                }
            }   
        }

        void UpdateTimer()
        {   int intTime = (int)time;

            int thirdDigit = intTime / 100;
            int secondDigit = (intTime / 10) % 10;
            int firstDigit = intTime % 10;
            
            images[0].material.SetFloat(indexID,firstDigit);
            images[1].material.SetFloat(indexID,secondDigit);
            images[2].material.SetFloat(indexID,thirdDigit);
            
        }
        int GetNthDigit(int number,int digit)
        {
            return (number / (int)Math.Pow(10,digit-1)) % 10;
        }
        void SetTimerToRed()
        {
            foreach (Image item in images)
            {
                item.color = warningRed;
            }
            timeRemainingNoko.color = warningRed;
            timeRemainingSeconds.color = warningRed;
            timeRemainingRi.color = warningRed;
        }
    }
}