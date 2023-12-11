using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble;

namespace Milan.GrassBubble
{
    public class BootUpManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        { 
            GameObject persistables = GameObject.Instantiate(Resources.Load("Persistables")) as GameObject;
            if (persistables == null)
                throw new Exception("Unable to load core processes :(");
            UnityEngine.Object.DontDestroyOnLoad(persistables);
            PersistableData persistableData = persistables.GetComponent<PersistableData>();
            Global.InitializeSettings(persistableData);
        }
    }
}