using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Testing;


namespace Milan.GrassBubble
{
    public static class Global 
    {

        const string sfx = "sfx";
        const string bgm = "bgm";
        const string grasshopperCount = "grasshoppercount";
        const string resolution = "resolution";
        const string camSensitivity = "camsensitivity";
        const string zoomSensitivity = "zoomsensitivity";
        const string camInvertHorizontal = "camInvertHorizontal";
        const string camInvertVertical = "camInvertVertical";

        public static int currentLevel = 0;
        public static DebugSettings debugSettings;
        public static void EnableDebugMode()
        {
            debugSettings.isEnabled = true;
        }
        public static void DisableDebugMode()
        {
            debugSettings.isEnabled = false;
        }
        public static void InitializeSettings(PersistableData data)
        {
            float bgmValue = PlayerPrefs.GetFloat(bgm);
            data.audioMixer.SetFloat(bgm,bgmValue);

            float sfxValue = PlayerPrefs.GetFloat(sfx);
            data.audioMixer.SetFloat(sfx,sfxValue);

            float camSensitivityValue = PlayerPrefs.GetFloat(camSensitivity);
            Settings.Controls.mouseDragSensitivity = camSensitivityValue;

            float zoomSensitivityValue = PlayerPrefs.GetFloat(zoomSensitivity);
            Settings.Controls.scrollSensitivity = zoomSensitivityValue;

            int camInvertHorizontalValue = PlayerPrefs.GetInt(camInvertHorizontal);
            Settings.Controls.lookHorizontalMode = (MouseDragBeheaviour)camInvertHorizontalValue;

            int camInvertVerticalValue = PlayerPrefs.GetInt(camInvertVertical);
            Settings.Controls.lookVerticalMode = (MouseDragBeheaviour)camInvertVerticalValue;

            int resolutionValue = PlayerPrefs.GetInt(resolution);
            Settings.Video.SetResolution(resolutionValue);

            int grassHopperCountValue = PlayerPrefs.GetInt(grasshopperCount);
            Settings.spawnAmount = (GrasshopperSpawnAmount)grassHopperCountValue;
            debugSettings = data.debugSettings;
        }
        public static class Scenes
        {
            public const int SplashScreen = 0;
            public const int MainMenu = 1;
            public const int Intro = 2;
            public const int LevelMain = 3;
            public const int TransitionScreen = 4;
            public const int CompleteScreen = 5;
            public const int Debug = 6;
        }
        public static int GetGrassHopperMultiplier()
        {
            switch (Settings.spawnAmount)
            {
                case GrasshopperSpawnAmount.Low:
                return 1;
                case GrasshopperSpawnAmount.Mid:
                return 2;
                case GrasshopperSpawnAmount.High:
                return 3;
                
                default:
                return 1;
            }
        }

        public static class Settings
        {

            public static void InitSpawnAmount(int value)
            {
                spawnAmount = (GrasshopperSpawnAmount)value;
            }
            public static GrasshopperSpawnAmount spawnAmount;
            // public static Video video;
            // public static Audio audio;
            // public static Controls controls;

            public static class Video
            {
                public static void SetResolution(int value)
                {
                    Debug.Log("New resolution is: " + (Resolutions)value);
                    switch ((Resolutions)value)
                    {
                        case Resolutions._1280x720:
                            Screen.SetResolution(1280,720,true);
                        break;
                        case Resolutions._1920x1080:
                            Screen.SetResolution(1920,1080,true);
                        break;
                        case Resolutions._2560x1400:
                            Screen.SetResolution(2560,1400,true);
                        break;
                        case Resolutions._3840x2160:
                            Screen.SetResolution(3840,2160,true);
                        break;
                        default:
                            //Nada
                        break;
                    }
                }
            }

            public class Controls
            {
                public static float mouseDragSensitivity;
                public static float scrollSensitivity;
                public static MouseDragBeheaviour lookVerticalMode;
                public static MouseDragBeheaviour lookHorizontalMode;
            }
        }

    }
}