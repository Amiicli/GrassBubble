using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Milan.GrassBubble.Gameloop
{
    [DisallowMultipleComponent]
    public class Player : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
        void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position,Vector3.one);
        }
    }
}