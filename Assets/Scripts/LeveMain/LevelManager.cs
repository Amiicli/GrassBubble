using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Gameloop;
using UnityEngine.SceneManagement;
using Milan.GrassBubble.Map;
using Milan.GrassBubble.Testing;

namespace Milan.GrassBubble.Gameloop
{
    public class LevelManager : MonoBehaviour
    {   

        int currentLevel;
        [SerializeField]
        GrassHopperControllerGPU ghController;
        [SerializeField]
        BubbleSpawn bubbleSpawn;
        [SerializeField]
        MapGenerator mapGenerator;
        public LevelList levelList;
        public CameraController cameraController;
        public LevelTimer levelTimer;
        [SerializeField]
        AudioSource audioSource;
        
        // Start is called before the first frame update
        void Start()
        {
            int grasshopperCount = 0;
            DebugSettings debugSettings = Global.debugSettings;
            bool skipIntro = false;
            Cursor.visible = false; 

            LevelData levelData;
            if(debugSettings.isEnabled)
            {
                skipIntro = debugSettings.skipIntro;
                if(debugSettings.disableTimer)
                    levelTimer.DisplayInfiniteTime();
                currentLevel = debugSettings.levelToLoad;
                levelData = levelList.GetLevelData(currentLevel);
                if(debugSettings.useDefaultGrasshopperCount)
                    grasshopperCount = levelData.grassHopperCount;
                else
                    grasshopperCount = debugSettings.grassHopperCount;
            }
            else
            {
                currentLevel = Global.currentLevel;
                levelData = levelList.GetLevelData(currentLevel);
                grasshopperCount = levelData.grassHopperCount;
                skipIntro = false;
            }
            audioSource.clip = levelData.track;
            LevelTimer.TimerOver += OnTimerOver;
            CameraController.IntroCompleted += OnIntroComplete;
            CameraController.OutroCompleted += OnOutroComplete;
            cameraController.Initialize(levelData,skipIntro);

            bubbleSpawn.Instantiate();
            mapGenerator.Initialize(levelData,true);

            // if((debugSettings.isEnabled && debugSettings.enableGrasshoppers == false) || debugSettings.isEnabled == false)
            ghController.InstantiateGrasshoppers(levelData,grasshopperCount);   

            // countController.Initialize(levelData);
            // uiGrasshopperCounter.Initialize(levelData);
            RenderSettings.skybox = levelData.skybox; 
            RenderSettings.ambientSkyColor = levelData.ambientColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            levelTimer.Initialize(levelList.GetLevelData(currentLevel));
            if(debugSettings.isEnabled && debugSettings.skipIntro == true)
                levelTimer.StartTimer();
        }
        void OnDisable() 
        {
            LevelTimer.TimerOver -= OnTimerOver;
            CameraController.IntroCompleted -= OnIntroComplete;
            CameraController.OutroCompleted -= OnOutroComplete;
        }
        void OnTimerOver()
        {
            cameraController.PlayOutroAnimation();
        }
        void OnOutroComplete()
        {
            SceneManager.LoadScene(Global.Scenes.TransitionScreen);
        }
        void OnIntroComplete()
        {
            levelTimer.StartTimer();
            audioSource.Play();
        }
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(Global.Scenes.MainMenu);
            }
            if(Input.GetKey(KeyCode.N))
            {
                SceneManager.LoadScene(Global.Scenes.TransitionScreen);
            }
        }
    }
}