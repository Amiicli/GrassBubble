using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Milan.GrassBubble;

namespace Milan.GrassBubble.Cutscene
{    
    public class SplashScreenManager : MonoBehaviour
    {
        bool skipAllowed = false;
        // Start is called before the first frame update
        void Start()
        {
            FadeController.Fade(FadeController.FadeColor.White,FadeController.FadeColor.Clear,FadeController.FadeType.EaseInOutCubic,5,AllowSkip);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.anyKeyDown && skipAllowed == true)
            {
                FadeToWhite();
            }
        }
        IEnumerator WaitWatch()
        {
            new WaitForSeconds(3);
            FadeToWhite();
            
            yield return null;
        }
        void AllowSkip()
        {
            skipAllowed = true;
            StartCoroutine(WaitWatch());
        }
        void FadeToWhite()
        {
            skipAllowed = false;
            FadeController.Fade(FadeController.FadeColor.Clear,FadeController.FadeColor.White,FadeController.FadeType.EaseInOutCubic,2,SwitchToMenu);
        }
        void SwitchToMenu()
        {
            SceneManager.LoadScene(Global.Scenes.MainMenu);
        }
        
    }
}
