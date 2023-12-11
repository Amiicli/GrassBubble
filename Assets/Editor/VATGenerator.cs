using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Milan;

namespace VAT
{
    public class VATGenerator : EditorWindow 
    {
        const UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R32G32B32_SFloat;
        const UnityEngine.Experimental.Rendering.TextureCreationFlags textureCreationFlag = UnityEngine.Experimental.Rendering.TextureCreationFlags.DontInitializePixels;
        const string saveDirectory = "Assets/Models/Grasshopper/"; //It's the only model in this game with VAT, this will suffice!
        const string saveDirectoryToAppend = "/Models/Grasshopper/";
        const int AnimationFPS = 24;
        const int AnimationLength = 32;
        UnityEngine.Object fbxObject;
        UnityEngine.Object animatorObject;
        float minBoundSize = -1;
        float maxBoundSize = 1;
        
        

        [MenuItem("Window/Milan/VAT Generator")]
        public static void ShowWindow()
        {
            GetWindow<VATGenerator>("VAT Generator");
        }

        void OnEnable()
        {
        }
        void OnGUI() 
        {            
            GUILayout.BeginHorizontal();  
                GUILayout.Label(".FBX File");
                fbxObject = EditorGUILayout.ObjectField(fbxObject,typeof(UnityEngine.Object),true);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();  
                GUILayout.Label("Gameobject w/ Animator");
                animatorObject = EditorGUILayout.ObjectField(animatorObject,typeof(UnityEngine.GameObject),true);
            GUILayout.EndHorizontal();
            GameObject selectedObject = Selection.activeGameObject;


            if(fbxObject != null)
            {
                if(GUILayout.Button("Generate Mesh With Proper UV2 Channel"))
                {
                    GenerateMesh();
                }
            }
            else
            {
                GUILayout.Label("Put a .fbx mesh in to start converting textures");
            }
            if(animatorObject != null)
            {
                GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Generate VAT Texture",GUILayout.Width(200)))
                    {
                            GenerateVATTextures(animatorObject as GameObject);
                    }
                    GUILayout.Label("Bounds",GUILayout.Width(60));
                    GUILayout.Label("Min",GUILayout.Width(30));
                    minBoundSize = EditorGUILayout.FloatField(minBoundSize,GUILayout.Width(60));
                    GUILayout.Label("Max",GUILayout.Width(30));
                    maxBoundSize = EditorGUILayout.FloatField(maxBoundSize,GUILayout.Width(60));
                    EditorGUILayout.Space(10);
                GUILayout.EndHorizontal();
            }
            if(GUILayout.Button("MakeMaterials"))
                MakeMaterialsFromFolder();
            
        }
        void GenerateVATTextures(GameObject selectedObj)
        {
            List<Texture2D> textures = new List<Texture2D>();
            //animationController.AddAnimationClips(importedAnimationClips);
            AnimationClip[] animationClips = selectedObj.GetComponent<Animator>().runtimeAnimatorController.animationClips;
            // Transform transform = selectedObj.transform.Find("leg_front_ik.l");
            GameObject childObj = selectedObj.transform.GetChild(1).gameObject;
            SkinnedMeshRenderer skinnedMeshRenderer = selectedObj.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
            Animator animator = selectedObj.GetComponent<Animator>();
            Mesh referenceMesh = skinnedMeshRenderer.sharedMesh;
            AnimationMode.StartAnimationMode();
            foreach (AnimationClip clip in animationClips)
            {
                Debug.Log($"Creating texture of size {referenceMesh.vertexCount}x{AnimationLength}");
                Texture2D texture2D = new Texture2D(referenceMesh.vertexCount,AnimationLength, graphicsFormat,textureCreationFlag);
                Debug.Log($"==={clip.name}/length: {clip.length}===");
                float incrementTime = clip.length / AnimationLength;
                Debug.Log("Framecount is " + AnimationFPS / clip.length);
                float time = 0;
                Color[] color = new Color[AnimationLength * referenceMesh.vertexCount + 2000];
                int count = 0;
                AnimationMode.BeginSampling();
                do
                {
                    AnimationMode.SampleAnimationClip(selectedObj,clip,time);
                    // animator.Play(clip.name,0,incrementTime);
                    // animator.Update(incrementTime);
                    Mesh reference = new Mesh();
                    skinnedMeshRenderer.BakeMesh(reference,false);

                    for (int i = 0; i < reference.vertexCount; i++)
                    {
                        Debug.Log(color[count]);
                        color[count] = EncodeVertexPositionToRGB(minBoundSize,maxBoundSize,reference.vertices[i]);
                        count++;
                    }

                    time += incrementTime;
                }
                while (time < clip.length);
                AnimationMode.EndSampling();
                animator.speed = 1;
                texture2D.SetPixelData(color,0,0);
                byte[] bytes = texture2D.EncodeToPNG();
                // Object.DestroyImmediate(texture2D);
                File.WriteAllBytes(Application.dataPath + saveDirectoryToAppend + clip.name + ".png", bytes);
                textures.Add(texture2D);
            }
            AnimationMode.StopAnimationMode();
            if(textures.Count <= 0)
                return;
            Texture2D combinedTexture = new Texture2D(textures[0].width,AnimationLength * textures.Count,graphicsFormat,textureCreationFlag);
            int yOffSet = 0;
            foreach (Texture2D texture in textures)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    for (int y = 0; y < texture.height; y++)
                    {
                        Color color = combinedTexture.GetPixel(x,y);
                        combinedTexture.SetPixel(x, y + yOffSet,color);
                    }
                }
                yOffSet += AnimationLength;
            }
            byte[] combinedTexturebytes = combinedTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + saveDirectoryToAppend + selectedObj.name + "_combined" + ".png", combinedTexturebytes);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            // foreach (var item in textures)
            // {
            //     Object.DestroyImmediate(item);
            // }
            // Object.DestroyImmediate(combinedTexture);
        }

        static Color EncodeVertexPositionToRGB(float positionMin, float positionMax, Vector3 position)
        {
            float r = Mathf.InverseLerp(positionMin, positionMax, position.x);
            float g = Mathf.InverseLerp(positionMin, positionMax, position.y);
            float b = Mathf.InverseLerp(positionMin, positionMax, position.z);
        
            Color color = new Color(r, g, b);
        
            return color;
        }

        void GenerateMesh()
        {
            GameObject tempObject = GameObject.Instantiate(fbxObject) as GameObject;    
            Mesh originalMesh = tempObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
            Vector2[] newUV2 = GenerateVATUV(originalMesh);

            Mesh newMesh = new Mesh();
            newMesh.vertices = originalMesh.vertices;
            newMesh.uv = originalMesh.uv;
            newMesh.uv2 = newUV2;
            newMesh.normals = originalMesh.normals;
            newMesh.tangents = originalMesh.tangents;
            newMesh.triangles = originalMesh.triangles;

            Debug.Log("vertex count is: " + newMesh.vertexCount);
            AssetDatabase.CreateAsset(newMesh,saveDirectory + "grasshopper.mesh");
            AssetDatabase.SaveAssets();

            GameObject.DestroyImmediate(tempObject);
        }
        Vector2[] GenerateVATUV(Mesh mesh)
        {
            float offset = 1.0f / mesh.vertexCount * 0.5f;
        
            Vector2[] uv = new Vector2[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                float newUVPos = MilanUtil.Remap(i, 0, mesh.vertexCount, 0, 1) + offset;
                uv[i] = new Vector2(newUVPos, 1.0f);
            }
        
            return uv;
        }
        //This technically doesn't belong here, but I didn't want to make yet another editor window
        void MakeMaterialsFromFolder()
        {
            string skyBoxpath = "Assets/Texture/Skyboxes";
            string exportPath = "Assets/Materials/Skyboxes";
            string[] paths = AssetDatabase.GetSubFolders(skyBoxpath);
            foreach (string path in paths)
            {
                Debug.Log("Looking at path: " + path);
                Material material = new Material(Shader.Find("Skybox/6 Sided"));
                
                string name = GetNameFromPath(path);
                material.name = name;
                string[] files = Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly);
                foreach (var item in files)
                {
                    Debug.Log(item);
                }
                foreach (var fileP in files)
                {                    
                    UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(fileP);
                    foreach (UnityEngine.Object obj in objects)
                    {
                        Debug.Log("Looking at texture: " + obj.name);
                        if(obj is Texture2D)
                        {
                            if(obj.name.Contains("back"))
                                material.SetTexture("_BackTex",(Texture2D)obj);
                            if(obj.name.Contains("down"))
                                material.SetTexture("_DownTex",(Texture2D)obj);
                            if(obj.name.Contains("front"))
                                material.SetTexture("_FrontTex",(Texture2D)obj);
                            if(obj.name.Contains("left"))
                                material.SetTexture("_LeftTex",(Texture2D)obj);
                            if(obj.name.Contains("right"))
                                material.SetTexture("_RightTex",(Texture2D)obj);
                            if(obj.name.Contains("up"))          
                                material.SetTexture("_UpTex",(Texture2D)obj);
                        }
                    }
                }
                AssetDatabase.CreateAsset(material,exportPath + "/" + name + ".mat");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
            }
            
        }
        string GetNameFromPath(string pathName)
        {
            List<char> constructor = new List<char>();
            for (int i = pathName.Length - 1; i >= 0 ; i--)
            {
                if(pathName[i] != '/')
                    constructor.Add(pathName[i]);
                else
                    break;
            
            }
            string export = "";
            for (int i = constructor.Count - 1; i >= 0 ; i--)
                export += constructor[i];
            
            return export;
        }

    }
}



// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using System.IO;
// using UnityEditor.SceneManagement;

// public class MenuItemMModel : MonoBehaviour
// {
//     [MenuItem("Assets/Create MModel", priority = 10)]
//     public static void CreateCustomCharacterFromAsset()
//     {
//         Object model = Selection.activeObject;
//         string assetPath = AssetDatabase.GetAssetPath(model);
//         string modelFileName = Path.GetFileName(assetPath); 
//         string modelDirectory = assetPath.Replace(modelFileName,"");

//         if(modelFileName.Substring(modelFileName.Length - 4) != ".fbx")
//         {
//             Debug.Log("Invalid file, not an .fbx!!!");
//             return;
//         }
//         string fileNameNoExtension = modelFileName.Split(".fbx")[0];
//         string prefabPath = modelDirectory + fileNameNoExtension + "_mmodel.prefab";
//         Debug.Log(assetPath);
//         Debug.Log(fileNameNoExtension);
//         Debug.Log(prefabPath);

//         Debug.Log(modelDirectory);
//         GameObject prefab;
//         GameObject gameObject = PrefabUtility.InstantiatePrefab(model) as GameObject;
//         bool canOverwrite = false;

//         if(File.Exists(prefabPath))
//         {
//             canOverwrite = EditorUtility.DisplayDialog("Overwrite Prefab", "There is already a mmodel prefab for this model, are you sure you want to overwrite it?" , "Overwrite", "Do Not Overwrite");
//             if(canOverwrite)
//             {
//                 prefab = PrefabUtility.SaveAsPrefabAsset(gameObject,prefabPath);
//             }
//             else
//             {
//                 prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
//             }
//         }
//         else
//         {
//             prefab = PrefabUtility.SaveAsPrefabAsset(gameObject,prefabPath);
//         }
//         string animPathName = modelDirectory + fileNameNoExtension + ".controller";
//         AnimationClip[] importedAnimationClips = MilanUtil.GetAnimationClipsFromFBX(assetPath);

//         AssetDatabase.SaveAssets();
        
//         MModel milanModel = prefab.AddComponent<MModel>();
//         AnimationController animationController = prefab.AddComponent<AnimationController>();
        
//         animationController.SetAnimatorController(UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(animPathName));
//         animationController.AddAnimationClips(importedAnimationClips);

//         PrefabStage stage = PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(prefab));
//         if(stage)
//         {
//             Selection.activeObject = stage.prefabContentsRoot;
//         }
//         GameObject.DestroyImmediate(gameObject);
//     }
// }
