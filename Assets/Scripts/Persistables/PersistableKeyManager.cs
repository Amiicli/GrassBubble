using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Milan.GrassBubble
{
    public class PersistableKeyManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKey(KeyCode.F))
            {
                Screen.fullScreen =! Screen.fullScreen;
            }
        }
    }
}
