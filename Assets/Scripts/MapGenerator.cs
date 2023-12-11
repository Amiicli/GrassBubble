using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Milan.GrassBubble.Map
{
    public class MapGenerator : MonoBehaviour
    {
        const string questionMarkPath = "Models\\QuestionMark";
        Texture2D mapTexture;
        Mesh mesh;
        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;
        [Range(5,100)]
        public float worldSize = 10;
        [Range(2,100)]
        public int density = 2;

        int mapWidth;
        int mapHeight;
        public float Ratio => (float)mapWidth / (float)mapHeight;

        Vector2[] UVs;

        void Awake() 
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            // Initialize();
        }
        void Start()
        {

        }


        public Vector2 GetDimensions()
        {
            mapWidth = mapTexture.width;
            mapHeight =  mapTexture.height;
            return mapWidth > mapHeight ? new Vector2(worldSize / Ratio,worldSize) : new Vector2(worldSize,worldSize * Ratio);  
        }
        public void SpawnQuestionMark()
        {
            GameObject questionMarkPrefab = Resources.Load<GameObject>(questionMarkPath);
            GameObject.Instantiate(questionMarkPrefab,this.transform);
        }

        public float SampleDimensions(float x, float y)
        {
            mapWidth = mapTexture.width;
            mapHeight =  mapTexture.height;
            
            Vector2 dimensions = GetDimensions();
            float centerFactorX = (dimensions.x / 2);
            float centerFactorY = (dimensions.y / 2);
            float gapSizeX = dimensions.x / density;
            float gapSizeY = dimensions.y / density;

            // float newX = (x * gapSizeX) + centerFactorX;
            // float newY = (y * gapSizeY) + centerFactorY;
            float newX = (x / (-dimensions.x )) + 0.5f;
            float newY = (y / (-dimensions.y )) + 0.5f;

            
            // Debug.Log($"X:{newX}/Y:{newY}");


            float u = x / density; 
            float v = 1 - y / density;
            return mapTexture.GetPixelBilinear(newY,1 - newX).a * 10;
        }
        public void Initialize(LevelData levelData, bool loadAllData)
        {
            mapTexture = levelData.mapTexture;
            if(loadAllData)
            {
                worldSize = levelData.mapSize;
            }
            mesh = new Mesh();
            mesh.name = levelData.namae;
            mesh.vertices = GenerateVertices();
            mesh.triangles = GenerateIndex();
            mesh.uv = UVs;
            meshFilter.mesh = mesh;
            meshRenderer.material.mainTexture = mapTexture;
            // Debug.Log(meshRenderer.material.mainTexture.name);
            if(meshCollider)
                meshCollider.sharedMesh = mesh;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
        Vector3[] GenerateVertices()
        {
            if(mapTexture == null)
                return new Vector3[]{Vector3.zero};
            Vector2 dimensions = GetDimensions();

            float centerFactorX = (dimensions.x / 2) * -1;
            float centerFactorY = (dimensions.y / 2) * -1;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            
            float gapSizeX = dimensions.x / density;
            float gapSizeY = dimensions.y / density;

            for (int x = 0; x < density + 1; x++)
            {
                for (int y = 0; y < density + 1; y++)
                {
                    Vector3 position = new Vector3();
                    position.x = (x * gapSizeX) + centerFactorX;
                    position.z = (y * gapSizeY) + centerFactorY;

                    float u = (float)x / density; 
                    float v = 1 - (float)y / density;
                    Vector2 uv = new Vector2(v,u);
                    uvs.Add(uv);
                    position.y = mapTexture.GetPixelBilinear(v,u).a * 10;
                    vertices.Add(position);
                }
            }
            UVs = uvs.ToArray();

            return vertices.ToArray();
        }
        int[] GenerateIndex()
        {
            int[] triangles = new int[density * density * 6];

            int vertIndex = 0;
            int triIndex = 0;
            for (int x = 0; x < density; x++)
            {
                for (int y = 0; y < density; y++)
                {
                    int upLeft = vertIndex;
                    int upRight = vertIndex + 1;
                    int downLeft = vertIndex + density + 1;
                    int downRight = vertIndex + density + 2;

                    triangles[triIndex] = downLeft;
                    triangles[triIndex + 1] = upLeft;
                    triangles[triIndex + 2] = downRight;

                    triangles[triIndex + 3] = downRight;
                    triangles[triIndex + 4] = upLeft;
                    triangles[triIndex + 5] = upRight;

                    vertIndex++;
                    triIndex += 6;
                }
                vertIndex++;
            }
            return triangles;
        }
        Vector2[] GenerateUVs(Vector3[] vector3s)
        {
            List<Vector2> vector2s = new List<Vector2>();

            Vector2 dimensions = mapWidth > mapHeight ? new Vector2(worldSize / Ratio,worldSize) : new Vector2(worldSize,worldSize * Ratio);

            float centerFactorX = (dimensions.x / 2) * -1;
            float centerFactorY = (dimensions.y / 2) * -1;

            List<Vector3> vertices = new List<Vector3>();
            
            float gapSizeX = dimensions.x / density;
            float gapSizeY = dimensions.y / density;
            
            foreach (Vector3 vec3 in vector3s)
            {
                Vector2 vec2 = new Vector2(vec3.x - centerFactorX,vec3.z - centerFactorY);
                vector2s.Add(vec2);
            }
            return vector2s.ToArray();
        }
        void OnDrawGizmos() 
        {
            foreach (var item in GenerateVertices())
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(item,0.1f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Vector3.zero,new Vector3(worldSize,0,worldSize));
            Gizmos.color = Color.blue;
            //height 1
            //width 2
            //4x4
            Vector3 dimensions = mapWidth > mapHeight ? new Vector3(worldSize / Ratio,0,worldSize) : new Vector3(worldSize,0,worldSize * Ratio);
            // Debug.Log(Ratio);
            // Gizmos.DrawWireCube(Vector3.zero - Vector3.up * 0.1f, dimensions);

            // float cubeSize = 2f;
            // Gizmos.DrawCube(new Vector3(testPos.x, SampleDimensions(testPos.x,testPos.y)+ cubeSize / 2,testPos.y),Vector3.one * cubeSize);
            if(mesh == null)
                return;
            Gizmos.color = Color.cyan;
            for (int i = 0; i < mesh.normals.Length; i++)
            {
                Gizmos.DrawRay(mesh.vertices[i],mesh.normals[i]);
            }
            
        }
    }
}