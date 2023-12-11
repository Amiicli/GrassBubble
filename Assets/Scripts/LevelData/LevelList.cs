using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble;


namespace Milan.GrassBubble
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "Milan/LevelList", order = 1)]
    public class LevelList : ScriptableObject
    {
        [SerializeField]
        LevelData[] list;
        public int Length => list.Length;
        
        // public LevelData this[int i] // square bracket operator with int argument
        // {
        //     get 
        //     {
        //         return list[i];
        //     }
        // }
        public int GetIndexOfLevel(LevelData levelData)
        {
            int count = 0;
            foreach (LevelData item in list)
            {
                if(item == levelData)
                    return count;
                count++;
            }
            return -9999;
        }
        public LevelData GetLevelData(int levelToGrab)
        {
            return list[levelToGrab];
        }
        
    }
}