using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Milan.GrassBubble.MainMenu
{
    public class OptionMenuController : MonoBehaviour
    {
        const string sfx = "sfx";
        const string bgm = "bgm";
        const string grasshopperCount = "grasshoppercount";
        const string resolution = "resolution";
        const string camSensitivity = "camsensitivity";
        const string zoomSensitivity = "zoomsensitivity";
        const string camInvertHorizontal = "camInvertHorizontal";
        const string camInvertVertical = "camInvertVertical";

        public AudioMixer audioMixer;
        public Slider sfxSlider;
        public Slider bgmSlider;
        public Slider camSensitivitySlider;
        public Slider zoomSensitivitySlider;
        public Dropdown camInvertVerticalDropdown;
        public Dropdown camInvertHorizontalDropdown;
        public Dropdown resolutionDropdown;
        public Dropdown grasshopperCountDropdown;
        // Start is called before the first frame update
        void Start()
        {
            float bgmValue = PlayerPrefs.GetFloat(bgm);
            bgmSlider.value = bgmValue;

            float sfxValue = PlayerPrefs.GetFloat(sfx);
            sfxSlider.value = sfxValue;

            camSensitivitySlider.value = Global.Settings.Controls.mouseDragSensitivity;
            zoomSensitivitySlider.value = Global.Settings.Controls.scrollSensitivity;
            camInvertHorizontalDropdown.value = (int)Global.Settings.Controls.lookHorizontalMode;
            camInvertVerticalDropdown.value = (int)Global.Settings.Controls.lookVerticalMode;
            grasshopperCountDropdown.value = (int)Global.Settings.spawnAmount;

            int resolutionValue = PlayerPrefs.GetInt(resolution);
            resolutionDropdown.value = resolutionValue;
            
        }

        public void OnSFXAudioChange(float value)
        {
            audioMixer.SetFloat(sfx,value);
            PlayerPrefs.SetFloat(sfx,value);
            
        }
        public void OnBGMAudioChange(float value)
        {
            audioMixer.SetFloat(bgm,value);
            PlayerPrefs.SetFloat(bgm,value);
        }
        public void OnZoomSensitivityChange(float value)
        {
            Global.Settings.Controls.scrollSensitivity = value;
            PlayerPrefs.SetFloat(zoomSensitivity,value);
        }
        public void OnCameraSensitivityChange(float value)
        {
            Global.Settings.Controls.mouseDragSensitivity = value;
            PlayerPrefs.SetFloat(camSensitivity,value);
        }
        public void OnCameraInvertVerticalChange(int value)
        {
            Global.Settings.Controls.lookVerticalMode = (MouseDragBeheaviour)value;
            PlayerPrefs.SetInt(camInvertVertical,value);
        }
        public void OnCameraInvertHorizontalChange(int value)
        {
            Global.Settings.Controls.lookHorizontalMode = (MouseDragBeheaviour)value;
            PlayerPrefs.SetInt(camInvertHorizontal,value);
        }
        public void OnResolutionChange(int value)
        {
            Global.Settings.Video.SetResolution(value);
            PlayerPrefs.SetInt(resolution,value);
        }
        public void OnGrasshopperCountChange(int value)
        {
            PlayerPrefs.SetInt(grasshopperCount,value);
            Global.Settings.spawnAmount = (GrasshopperSpawnAmount)value;
        }
    }
}