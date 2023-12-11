using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Gameloop;
using Milan.GrassBubble.Map;

namespace Milan.GrassBubble.Gameloop
{
    public class GrassHopperControllerGPU : MonoBehaviour
    {
        Transform player;
        // const float Gravity = 9.8f;
        const float Gravity = 15.8f;
        const float GrasshopperSpeed = 3;
        const float GrasshopperJumpSpeed = 10f;
        const float Deceleration = 1f;
        const float JumpPrepTime = 0.1f;
        float boundX;
        float boundY;
        public Mesh grasshopperMesh;
        public Material grasshopperMat;
        public BubbleSpawn bubbleSpawn;
        public CameraController cameraController;
        public LevelList levelList;

        ComputeBuffer grasshopperBuffer;
        ComputeBuffer bubbleBuffer;
        ComputeBuffer positionsBuffer;

        public ComputeShader computeShader;

        Grasshopper[] grasshoppers;
        
        [SerializeField]
        MapGenerator mapGenerator;

        public void InstantiateGrasshoppers(LevelData levelData,int grasshopperCount)
        {
            player = cameraController.cursor;
            int grasshopperMultiplier = grasshopperCount * Global.GetGrassHopperMultiplier();
            Debug.Log($"Spawning {grasshopperCount} Grasshoppers with multiplier of" + Global.GetGrassHopperMultiplier() + " (" + Global.Settings.spawnAmount + ")");
            grasshoppers = new Grasshopper[grasshopperMultiplier];
            Vector2 dimensions = mapGenerator.GetDimensions();
            for (int i = 0; i < grasshoppers.Length; i++)
            {
                Vector3 random = UnityEngine.Random.insideUnitSphere * 10;
                random.y = 0;
                grasshoppers[i] = new Grasshopper(random,0);
                grasshoppers[i].position.y = mapGenerator.SampleDimensions(grasshoppers[i].position.x,grasshoppers[i].position.z);
                grasshoppers[i].position.x = UnityEngine.Random.Range(-dimensions.x / 2,dimensions.x / 2);
                grasshoppers[i].position.z = UnityEngine.Random.Range(-dimensions.y / 2,dimensions.y / 2);
            }
            boundX = dimensions.x / 2;
            boundY = dimensions.y / 2;

            int grasshopperStructSize = Grasshopper.GetGrasshopperSize();
            int bubbleStructSize = BubbleSpawn.GetBubbleSize();
            grasshopperBuffer = new ComputeBuffer(grasshoppers.Length,grasshopperStructSize);
            grasshopperBuffer.SetData(grasshoppers);
            
            bubbleBuffer = new ComputeBuffer(BubbleSpawn.maxBubbles,bubbleStructSize);
            bubbleBuffer.SetData(BubbleSpawn.bubbles);
            
            positionsBuffer = new ComputeBuffer(grasshoppers.Length,3 * 4);
            computeShader.SetBuffer(0,"grasshoppers",grasshopperBuffer);
            computeShader.SetFloat("boundX",boundX);
            computeShader.SetFloat("boundY",boundY);

            computeShader.SetFloat("bubbleLength",BubbleSpawn.bubbles.Length);
            computeShader.SetVector("_color1",ColorToVector4(levelData.color1));
            computeShader.SetVector("_color2",ColorToVector4(levelData.color2));
            computeShader.SetFloat("_variance",levelData.variance);
            computeShader.SetFloat("playerPosX",player.position.x);
            computeShader.SetFloat("playerPosY",player.position.y);
            computeShader.SetFloat("playerPosZ",player.position.z);
            computeShader.SetFloat("elapsedTime",Time.timeSinceLevelLoad);

            computeShader.SetFloat("deltaTime",Time.deltaTime);

            // computeShader.SetTexture(0,"mapTexture",mapGenerator.mapTexture);
            Vector2 mapDimensions = mapGenerator.GetDimensions();
            computeShader.SetFloat("mapX",mapDimensions.x);
            computeShader.SetFloat("ratio",mapGenerator.Ratio);
            computeShader.SetFloat("mapY",mapDimensions.y);
            computeShader.SetFloat("worldSize",mapGenerator.worldSize);
            computeShader.SetInt("density",mapGenerator.density);
            computeShader.SetTexture(0,"map",levelData.mapTexture);
        }
        void OnDisable() 
        {
            grasshopperBuffer.Dispose();
            bubbleBuffer.Dispose();
            positionsBuffer.Dispose();
        }

        Vector4 ColorToVector4(Color color)
        {
            return new Vector4(color.r,color.g,color.b,color.a);
        }
        void Update()
        {
            int groups = Mathf.CeilToInt(grasshoppers.Length / 10);
            // computeShader.SetBuffer(0,"grasshoppers",grasshopperBuffer);
            Vector3 playerPos = player.position;
            computeShader.SetFloat("playerPosX",player.position.x);
            computeShader.SetFloat("playerPosY",player.position.y);
            computeShader.SetFloat("playerPosZ",player.position.z);
            computeShader.SetFloat("deltaTime",Time.deltaTime);
            bubbleBuffer.SetData(BubbleSpawn.bubbles);
            computeShader.SetBuffer(0,"bubbles",bubbleBuffer);
            
            Vector3 playerDir = player.rotation.eulerAngles;
            int bubbleLength = BubbleSpawn.maxBubbles;
            computeShader.Dispatch(0,groups,1,1);
            grasshopperMat.SetBuffer("grasshoppers",grasshopperBuffer);
            Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 100);
            Graphics.DrawMeshInstancedProcedural(grasshopperMesh, 0, grasshopperMat, bounds,grasshoppers.Length);
            // Debug.Log(data[0]);
    
        }
    }

}