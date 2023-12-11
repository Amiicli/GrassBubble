using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EasingUtil;
using Milan.GrassBubble.Gameloop;
using Milan.GrassBubble.Testing;

namespace Milan.GrassBubble.Gameloop
{
    public class BubbleSpawn : MonoBehaviour
    {
        public GameObject bubble;
        Bubble[] bubbleModels;
        const int MaxBubbles = 10;
        [SerializeField,Range(1,20)]
        float avgDiameterSize;
        public static int maxBubbles => MaxBubbles;
        public static float AvgDiameterSize;
        public static SBubble[] bubbles;
        public LayerMask layerMask;


        const int bubbleAppearance = 5;
        const int cursorReticleIndex = 1;
        // Start is called before the first frame update
        void Awake()
        {
            AvgDiameterSize = avgDiameterSize;
        }
        public void Instantiate()
        {
            bubbles = new SBubble[MaxBubbles];
            bubbleModels = new Bubble[maxBubbles];
            for (int i = 0; i < bubbles.Length; i++)
            {
                bubbles[i] = new SBubble(new Vector3(0, -999999, 0));
                bubbleModels[i] = GameObject.Instantiate(bubble, bubbles[i].position, Quaternion.identity).GetComponent<Bubble>();
                bubbleModels[i].name = "Bubble " + i; 
            }
        }
        public static int GetBubbleSize()
        {
            int floatSize = sizeof(float);
            int intSize = sizeof(int);
            int vector3Size = sizeof(float) * 3;
            int bubbleSize = (vector3Size * 3) + (floatSize * 4) + intSize;
            return bubbleSize;
        }

        // Update is called once per frame
        void Update()
        {

            BubbleLoop();
        }




        void BubbleLoop()
        {
            if (bubbles == null)
                return;
                
            for (int i = 0; i < bubbles.Length; i++)
            {
                Vector3 addVelocity = new Vector3();
                SBubble bubble = bubbles[i];
                Vector3 lastPosition = bubble.position;
                if (bubble.position.y < -999)
                {
                    continue;
                }
                RaycastHit hit;
                
                if(Physics.SphereCast(bubble.position,bubble.radius * 10,Vector3.zero, out hit))
                {
                    Debug.DrawLine(bubble.position,hit.point,Color.red,20);
                    Debug.Log("hit at " + hit.point);
                    Vector3 oppositeDirection = -(hit.point - bubble.position);
                    addVelocity += oppositeDirection;
                }

                for (int b = 0; b < bubbles.Length; b++)
                {
    
                    float distance = Vector3.Distance(bubble.position,bubbles[b].position);
                    if(b == i || distance > 99)
                        continue;
                    if(distance < bubble.radius * bubbleAppearance / 2)
                    {
                        Vector3 oppositeDirection = -(bubbles[b].position - bubble.position);
                        addVelocity += oppositeDirection * 0.1f;

                    }


                }
                int seed = bubble.seed;
                Vector3 position = bubble.position += (new Vector3(Mathf.Sin(Time.timeSinceLevelLoad + seed), 1, Mathf.Sin(Time.timeSinceLevelLoad + seed))) * Time.deltaTime;
                bubble.delta = lastPosition - position;
                bubble.lifeTick += Time.deltaTime;
                // if(bubble.radius > 2)
                // bubble.radius -= Time.deltaTime;
                if (bubble.lifeTick >= bubble.lifeTime)
                {
                    float lifeTime = bubble.lifeTime;
                    float lifeTick = bubble.lifeTick;
                    Debug.Log($"lifetime passed at {lifeTime}, back to bubble death!");
                    bubble.position.y = -999999;
                    bubble.velocity = Vector3.zero;
                    bubble.lifeTick = 0;
                    bubble.delta = Vector3.zero;
                    bubbleModels[i].PlayParticle();
                    
                }
                bubble.velocity.y = 0;
                bubble.velocity += addVelocity * Time.deltaTime;
                bubble.velocity = Vector3.MoveTowards(bubble.velocity,Vector3.zero,0.17f * Time.deltaTime);
                bubble.position += bubble.velocity;

                float radius = bubble.radius * bubbleAppearance;

                bubbleModels[i].transform.position = bubble.position;
                bubbleModels[i].transform.localScale = new Vector3(radius, radius, radius);


                bubbles[i] = bubble;
            }
        }

        public void SpawnBubble(Vector3 positionToSpawnAt)
        {
            int bubbleToSpawn = -1;
            for (int i = 0; i < bubbles.Length; i++)
            {
                if (bubbles[i].position.y < -9999)
                {
                    bubbleToSpawn = i;
                    break;
                }
            }
            if(bubbleToSpawn == -1) // Return since we couldn't find an available bubble
                return;
            bubbles[bubbleToSpawn].radius = UnityEngine.Random.Range(BubbleSpawn.AvgDiameterSize, BubbleSpawn.AvgDiameterSize + 0.2f);
            float yPosition = (bubbles[bubbleToSpawn].radius / 2);
            positionToSpawnAt = new Vector3(positionToSpawnAt.x,positionToSpawnAt.y + yPosition,positionToSpawnAt.z);
            bubbles[bubbleToSpawn].position = positionToSpawnAt;
            bubbleModels[bubbleToSpawn].GetComponent<Bubble>().PlayBubbleAppearAnimation();
            
        }
        void OnDrawGizmos()
        {
            if (bubbles == null)
                return;
            foreach (var item in bubbles)
            {
                Gizmos.DrawWireSphere(item.position, item.radius);
            }
        }
        void OnGUI()
        {
            // GUILayout.BeginArea(new Rect(0, 0, 400, 400));
            // GUILayout.BeginVertical();
            // int counter = 0;
            // foreach (SBubble bubble in bubbles)
            // {
            //     string label = $"Bubble {counter + 1} pos: {bubble.position.ToString()}/vel: {bubble.velocity.ToString()}";
            //     GUILayout.Label(label);
            //     counter++;
            // }
            // GUILayout.EndVertical();
            // GUILayout.EndArea();
        }


    }

    public struct SBubble
    {
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 delta;
        public float lifeTick;
        public readonly float lifeTime;
        public int seed;
        public float radius;
        public float scale;

        public SBubble(Vector3 position)
        {
            velocity = Vector3.zero;
            this.position = position;
            lifeTick = 0;
            delta = Vector3.zero;
            seed = UnityEngine.Random.Range(0, 100000);
            lifeTime = UnityEngine.Random.Range(5f, 6f);
            radius = UnityEngine.Random.Range(BubbleSpawn.AvgDiameterSize, BubbleSpawn.AvgDiameterSize + 2);
            scale = 0;
            // Debug.Log(lifeTime);
        }
    }
}