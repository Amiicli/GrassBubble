using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Milan.GrassBubble;
using Milan.GrassBubble.Testing;

public class DebugMenu : EditorWindow 
{
    const int minGrasshoppers = 20;
    const int MaxGrasshoppers = 500_000;
    const float RightSideSpacing = 180;
    readonly Color LightGray = new Color(0.75f,0.75f,0.75f,1);
    DebugSettings debugSettings;
    LevelList levelList;
    Vector2 scrollVec;
    [MenuItem("Window/Milan/Debug Menu")]
    public static void ShowWindow()
    {
        GetWindow<DebugMenu>("Debug");
    }
    void OnEnable() 
    {
        debugSettings = Resources.Load<DebugSettings>("DebugSettings");    
        levelList = AssetDatabase.LoadAssetAtPath<LevelList>("Assets/Resources/LevelList.asset");
        scrollVec = new Vector2();
    }
    void OnGUI() 
    {
        Color guiColor = GUI.color;

        scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Debug",Header());
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Active",GUILayout.Width(RightSideSpacing));
                debugSettings.isEnabled = EditorGUILayout.Toggle(debugSettings.isEnabled,GUILayout.Width(10));
            EditorGUILayout.EndHorizontal();
            
            GUI.color = debugSettings.isEnabled ? GUI.color : new Color(0.80f,0.80f,0.90f,1);

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Skip intro",GUILayout.Width(RightSideSpacing));
                debugSettings.skipIntro = EditorGUILayout.Toggle(debugSettings.skipIntro,GUILayout.Width(10));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("TransitionToLevel",GUILayout.Width(RightSideSpacing));
                debugSettings.transitionToLevel = EditorGUILayout.Toggle(debugSettings.transitionToLevel,GUILayout.Width(10));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Enable grasshoppers",GUILayout.Width(RightSideSpacing));
                debugSettings.enableGrasshoppers = EditorGUILayout.Toggle(debugSettings.enableGrasshoppers,GUILayout.Width(10));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Disable Timer",GUILayout.Width(RightSideSpacing));
                debugSettings.disableTimer = EditorGUILayout.Toggle(debugSettings.disableTimer,GUILayout.Width(10));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Use default grasshopper count",GUILayout.Width(RightSideSpacing));
                debugSettings.useDefaultGrasshopperCount = EditorGUILayout.Toggle(debugSettings.useDefaultGrasshopperCount,GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            if(debugSettings.useDefaultGrasshopperCount == false)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Grasshopper count",GUILayout.Width(RightSideSpacing));
                    debugSettings.grassHopperCount = EditorGUILayout.IntSlider(debugSettings.grassHopperCount,minGrasshoppers,MaxGrasshoppers,GUILayout.Width(150));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Map #",GUILayout.Width(RightSideSpacing));
                debugSettings.levelToLoad = EditorGUILayout.IntSlider(debugSettings.levelToLoad,0,levelList.Length - 1,GUILayout.Width(150));
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField(levelList.GetLevelData(debugSettings.levelToLoad).namae,GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            GUI.color = guiColor;
            LevelData level = levelList.GetLevelData(debugSettings.levelToLoad);
            Editor editor = Editor.CreateEditor(levelList.GetLevelData(debugSettings.levelToLoad));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Map Data",Header());
            GUILayout.Space(10);
            GUILayout.Label(level.namae,SubHeader());
            GUILayout.Space(10);
            editor.OnInspectorGUI();
        EditorGUILayout.EndScrollView();
    }
    GUIStyle Header()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.normal.textColor = LightGray;
        return style;
    }
    GUIStyle SubHeader()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 15;
        style.normal.textColor = Color.gray;
        return style;
    }
}