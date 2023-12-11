using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Milan;

namespace Milan
{
    public static class MilanUtil
    {
        #if UNITY_EDITOR
        public static AnimationClip[] GetAnimationClipsFromFBX(string modelImporterPath)
        {
            var assetRepresentationsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(modelImporterPath);
            List<AnimationClip> animationClips = new List<AnimationClip>();
            foreach (var assetRepresentation in assetRepresentationsAtPath)
            {
                var animationClip = assetRepresentation as AnimationClip;

                if (animationClip != null)
                {
                    string test = animationClip.name;
                    string[] split = test.Split("|");
                    if (split.Length > 1)
                    {
                        Debug.Log(split[1]);
                        animationClip.name = split[1];
                    }
                    animationClips.Add(animationClip);
                    Debug.Log("Found animation clip! " + animationClip);
                }
            }
            return animationClips.ToArray();
        }
        #endif
        public static float Remap (float from, float fromMin, float fromMax, float toMin,  float toMax)
        {
            float fromAbs  =  from - fromMin;
            float fromMaxAbs = fromMax - fromMin;        
            float normal = fromAbs / fromMaxAbs;
            float toMaxAbs = toMax - toMin;
            float toAbs = toMaxAbs * normal;
            float to = toAbs + toMin;
            
            return to;
        }

        // public static float Remap(float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo)
        // {
        //     return targetFrom + (source-sourceFrom)*(targetTo-targetFrom)/(sourceTo-sourceFrom);
        // }


    }
}