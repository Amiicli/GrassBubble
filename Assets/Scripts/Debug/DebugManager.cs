using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Milan.GrassBubble.Testing;



namespace Milan.GrassBubble.Testing
{
    public class DebugManager : MonoBehaviour
    {
        
        const float minGrasshoppers = 20;
        const int MaxGrasshoppers = 500_000;
        float resolutionX;
        float resolutionY;
        float startX;
        float endX;
        float startY;
        [SerializeField]
        public LevelList levelList;
        [SerializeField]
        float RightSideSpacing = 180;
        DebugSettings debugSettings;
        int multipier;
        // Start is called before the first frame update
        void Start()
        {
            Global.EnableDebugMode();
            resolutionX = Screen.width;
            resolutionY = Screen.height;
            startX = resolutionX / 4;
            endX = resolutionX - (startX * 2);
            debugSettings = Global.debugSettings;
            multipier = Global.GetGrassHopperMultiplier();
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void OnGUI() 
        {
            GUILayout.BeginArea(new Rect(startX,20,endX,resolutionY));
                GUILayout.Space(20);
                GUILayout.Label("Debug Menu");
                GUILayout.Space(10);

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Skip intro",GUILayout.Width(RightSideSpacing));
                    debugSettings.skipIntro = GUILayout.Toggle(debugSettings.skipIntro," ",GUILayout.Width(10));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("TransitionToLevel",GUILayout.Width(RightSideSpacing));
                    debugSettings.transitionToLevel = GUILayout.Toggle(debugSettings.transitionToLevel," ",GUILayout.Width(10));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Enable grasshoppers",GUILayout.Width(RightSideSpacing));
                    debugSettings.enableGrasshoppers = GUILayout.Toggle(debugSettings.enableGrasshoppers," ",GUILayout.Width(10));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Disable Timer",GUILayout.Width(RightSideSpacing));
                    debugSettings.disableTimer = GUILayout.Toggle(debugSettings.disableTimer," ",GUILayout.Width(10));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Use default grasshopper count",GUILayout.Width(RightSideSpacing));
                    debugSettings.useDefaultGrasshopperCount = GUILayout.Toggle(debugSettings.useDefaultGrasshopperCount," ",GUILayout.Width(50));
                GUILayout.EndHorizontal();

                if(debugSettings.useDefaultGrasshopperCount == false)
                {
                    GUILayout.BeginHorizontal();
                            GUILayout.Label("Grasshopper count",GUILayout.Width(RightSideSpacing));
                            debugSettings.grassHopperCount = (int)GUILayout.HorizontalSlider(debugSettings.grassHopperCount,minGrasshoppers,MaxGrasshoppers,GUILayout.Width(150));
                            int finalNum = debugSettings.grassHopperCount * multipier;
                            GUILayout.Label(finalNum.ToString());
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                    GUILayout.Label("Map #",GUILayout.Width(RightSideSpacing));
                    debugSettings.levelToLoad = (int)GUILayout.HorizontalSlider(debugSettings.levelToLoad,0,levelList.Length - 1,GUILayout.Width(150));
                    GUILayout.Space(5);
                    GUILayout.Label(levelList.GetLevelData(debugSettings.levelToLoad).namae,GUILayout.Width(100));
                GUILayout.EndHorizontal();
                if(GUILayout.Button("START"))
                {
                    int count = debugSettings.grassHopperCount;
                    debugSettings.grassHopperCount = count - (count % 10);
                    SceneManager.LoadScene(Global.Scenes.LevelMain);
                }
            GUILayout.EndArea();
        }
    }
}