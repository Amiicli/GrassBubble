using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Milan.GrassBubble
{
    [CreateAssetMenu(fileName = "Level", menuName = "Milan/Level", order = 1)]
    public class LevelData : ScriptableObject
    {
        public string namae;

        public Texture2D mapTexture;

        [Range(5,100)]
        public float minZoomDistance = 5;
        [Range(5,100)]
        public float maxZoomDistance = 40;
        [Range(5,100)]
        public float mapSize;
        [Range(0,300)]
        public int time;

        public Color color1;
        public Color color2;
        [SerializeField,Range(0,0.5f)]
        public float variance;
        
        public AudioClip track;

        [Range(10,50000)]
        public int grassHopperCount;
        public Color ambientColor;
        public Material skybox;
}
}