using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Map;

namespace Milan.GrassBubble.LevelTransition
{
    [RequireComponent(typeof(MapGenerator))]
    public class MapHoverEffect : MonoBehaviour
    {
        [Range(0f,100)]
        public float rotateSpeed = 1;
        [Range(0f,2)]
        public float frequency = 1;
        [Range(0f,2)]
        public float amplitude = 1;
        Vector3 OGPos;
        Bounds bounds;
        Vector3 tempPos;
        Vector3 posOffset;
        float randomSeed;
        // Update is called once per frame
        void Start() 
        {
            randomSeed = UnityEngine.Random.Range(0f,10000f);
            OGPos = new Vector3(transform.position.x,0,transform.position.z);
            transform.eulerAngles = new Vector3(0,randomSeed,0);

        }
        void Update()
        {

            Vector3 rot = new Vector3(0f,Time.deltaTime * rotateSpeed,0f);
            transform.Rotate(rot,Space.World);

            tempPos = OGPos;
            tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency + randomSeed) * amplitude;

            transform.position = new Vector3(transform.position.x,tempPos.y,transform.position.z);
        }
    }
}