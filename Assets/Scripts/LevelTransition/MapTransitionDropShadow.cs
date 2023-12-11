using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Milan.GrassBubble.LevelTransition
{
    public class MapTransitionDropShadow : MonoBehaviour
    {
        [Range(0,5)]
        public float shrinkGrowRatio = 1;
        [Range(0,5)]
        public float originalSize = 2;
        public Transform target;
        [SerializeField,Range(0,1)]
        float offSetter;

        private void Start() 
        {
            transform.position = new Vector3(transform.position.x,-1f,transform.position.z);
        }

        void Update() 
        {
            float distance = Vector3.Distance(transform.position,target.position);
            distance *= (shrinkGrowRatio) / 8;
            distance = offSetter - distance;
            float targetSize = target.localScale.x;
            transform.localScale = new Vector3(distance + originalSize + targetSize,distance + originalSize + targetSize,distance + originalSize + targetSize);
        }
    }
}