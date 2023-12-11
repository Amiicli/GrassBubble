using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Milan.GrassBubble;

namespace Milan.GrassBubble.Cutscene
{
    public class GameIntroManager : MonoBehaviour
    {
        [SerializeField]
        TextFadeManager textFadeManager;
        bool allowFadeout;
        [SerializeField,Range(0,5)]
        public float waitTime;
        // Start is called before the first frame update
        void Start()
        {
            FadeController.Fade(FadeController.FadeColor.White,FadeController.FadeColor.Clear, FadeController.FadeType.EaseOutQuart, 2);
            allowFadeout = true;
            Global.currentLevel = 0;
            Global.debugSettings.isEnabled = false;
            TextFadeManager.TextFadeCompleted += OnTextFadeFinish;
            textFadeManager.Initialize(waitTime);
        }

        void OnDisable() 
        {
            TextFadeManager.TextFadeCompleted -= OnTextFadeFinish;
        }
        // Update is called once per frame
        void Update()
        {
            if(Input.anyKeyDown && allowFadeout == true)
                NextMap();
            
        }
        void OnTextFadeFinish()
        {
            if(allowFadeout == true)
                NextMap();
            
        }
        void NextMap()
        {
            allowFadeout = false;
            FadeController.Fade(FadeController.FadeColor.Clear,FadeController.FadeColor.White,FadeController.FadeType.EaseOutSine,3,CallBackForFade);
        }
        void CallBackForFade()
        {
            SceneManager.LoadScene(Global.Scenes.LevelMain);
        }
    }
}