using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Testing;

namespace Milan.GrassBubble.Testing
{    
    [CreateAssetMenu(fileName = "DebugSettings", menuName = "Milan/DebugSettings", order = 1)]
    public class DebugSettings : ScriptableObject
    {
        public bool isEnabled;
        public int levelToLoad;
        //Level Specific
        public bool enableGrasshoppers;
        public bool disableTimer;
        public int grassHopperCount;
        public bool useDefaultGrasshopperCount;
        public bool skipIntro;
        //Transition screen specific 
        public bool transitionToLevel;
    }
}
