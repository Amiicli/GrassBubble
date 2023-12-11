using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Milan.GrassBubble;
using Milan.GrassBubble.Testing;

namespace Milan.GrassBubble.LevelTransition
{
    public class LevelSceneTransitionManager : MonoBehaviour
    {
        public Canvas mainTransitionCanvas;
        public LevelList levelList;
        public LevelTransitioner levelTransitioner;
        public TitleManager titleManager;
        int currentLevel;
        // Start is called before the first frame update
        void Start()
        {
            FadeController.KillAllFades();
            FadeController.Fade(FadeController.FadeColor.White,FadeController.FadeColor.Clear,FadeController.FadeType.EaseInOutCubic,1);
            Cursor.visible = false; 
            if(Global.debugSettings.isEnabled)
                Global.currentLevel = Global.debugSettings.levelToLoad;
            currentLevel = Global.currentLevel;
            if(Global.currentLevel >= levelList.Length - 1)
            {
                StartEndingSequence();
                return;
            }
            LevelTransitioner.SwapHalfWayComplete += OnHalfWayCompleteTransition;
            LevelTransitioner.CamAnimationComplete += OnCameraAnimationComplete;
            LevelData levelData = levelList.GetLevelData(currentLevel);
            titleManager.PreInitialize(levelData,currentLevel);
            levelTransitioner.Initialize(levelList,currentLevel);
        }
        void StartEndingSequence()
        {
            SceneManager.LoadScene(Global.Scenes.CompleteScreen);
            Debug.Log("Starting ending sequence");
        }
        void OnDisable() 
        {
            LevelTransitioner.SwapHalfWayComplete -= OnHalfWayCompleteTransition;
            LevelTransitioner.SwapHalfWayComplete -= OnCameraAnimationComplete;
        }
        void OnHalfWayCompleteTransition()
        {
            LevelTransitioner.SwapHalfWayComplete -= OnHalfWayCompleteTransition;
            currentLevel++;
            Global.currentLevel = currentLevel;
            LevelData levelData = levelList.GetLevelData(currentLevel);
            titleManager.Initialize(levelData,levelList.GetIndexOfLevel(levelData));
        }
        void OnCameraAnimationComplete()
        {
            Debug.Log("Camera animation complete");
            if(Global.debugSettings.isEnabled == false || Global.debugSettings.transitionToLevel != false)
            {
                SceneManager.LoadScene(Global.Scenes.LevelMain);
            }
        }

    }
}