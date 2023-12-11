using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Milan.GrassBubble.Gameloop;
using Milan.GrassBubble.Map;

namespace Milan.GrassBubble.Obsolete
{
    public class GrasshopperController : MonoBehaviour
    {
        [SerializeField]
        Transform player;
        [SerializeField,Range(1,20000)]
        int GrasshopperCount = 20;
        // const float Gravity = 9.8f;
        const float Gravity = 15.8f;
        const float GrasshopperSpeed = 3;
        const float GrasshopperJumpSpeed = 10f;
        const float Deceleration = 1f;
        const float JumpPrepTime = 0.1f;
        float boundX;
        float boundY;
        public Mesh refMesh;

        public BubbleSpawn bubbleSpawn;

        Grasshopper[] grasshoppers;

        [SerializeField]
        MapGenerator mapGenerator;

        void Start()
        {
            bubbleSpawn.Instantiate();
            grasshoppers = new Grasshopper[GrasshopperCount];
            for (int i = 0; i < grasshoppers.Length; i++)
            {
                Vector3 random = UnityEngine.Random.insideUnitSphere * 20;
                random.y = 0;
                grasshoppers[i] = new Grasshopper(random,0);
                grasshoppers[i].position.y = mapGenerator.SampleDimensions(grasshoppers[i].position.x,grasshoppers[i].position.y);
            }
            // mapGenerator.Initialize();
            Vector2 dimensions = mapGenerator.GetDimensions();
            boundX = dimensions.x / 2;
            boundY = dimensions.y / 2;
        }


        void Update()
        {
            Vector3 playerPos = player.position;
            Vector3 playerDir = player.rotation.eulerAngles;
            int bubbleLength = BubbleSpawn.maxBubbles;
            for (int i = 0; i < grasshoppers.Length; i++)
            {
                Grasshopper grasshopper = grasshoppers[i]; //TODO: Change this eventually
                if(grasshoppers[i].position.y < -999999f)
                {
                    continue;
                }

                float ground = mapGenerator.SampleDimensions(grasshoppers[i].position.x,grasshoppers[i].position.z);

                switch (grasshoppers[i].state)
                {
                    case GrasshopperState.Idle:

                        if(grasshoppers[i].forwardVelocity > 0)
                        {
                            grasshoppers[i].forwardVelocity -= Time.deltaTime;
                        }
                        else
                        {
                            grasshoppers[i].forwardVelocity = 0;
                        }
                        grasshoppers[i].timer += Time.deltaTime;
                        if(Vector3.Distance(playerPos,grasshoppers[i].position) < 10)
                        {
                            // grasshoppers[i].direction = Vector3.Angle(playerPos,grasshoppers[i].position) + 180;
                            Vector3 directionToPlayer = player.position - grasshoppers[i].position;
                            float angleToPlayer = Mathf.Atan2(directionToPlayer.z, directionToPlayer.x) * Mathf.Rad2Deg;
                            grasshoppers[i].direction = angleToPlayer + 180f;
                            grasshoppers[i].timer = 0;
                            grasshoppers[i].state = GrasshopperState.HopPrep;
                            continue;
                        }
                        if(grasshoppers[i].timer > grasshoppers[i].jumpWaitTime)
                        {
                            grasshoppers[i].direction = UnityEngine.Random.Range(0,360);
                            grasshoppers[i].timer = 0;
                            grasshoppers[i].state = GrasshopperState.HopPrep;
                        }
                        for (int b = 0; b < bubbleLength; b++)
                        {
                            float radius = BubbleSpawn.bubbles[b].radius;
                            if(Vector3.Distance(BubbleSpawn.bubbles[b].position,grasshoppers[i].position) < radius)
                            {
                                grasshoppers[i].bubbleParent = b;
                                grasshoppers[i].timer = 0;  
                                grasshoppers[i].forwardVelocity = grasshoppers[i].position.x;
                                grasshoppers[i].upwardVelocity = grasshoppers[i].position.y;
                                grasshoppers[i].jumpWaitTime = grasshoppers[i].position.z;
                                grasshoppers[i].state = GrasshopperState.BubbleCapture;  
                            }
                        }
                        if (grasshoppers[i].position.x < -boundX) 
                        {
                            grasshoppers[i].position.x = -boundX;
                        }
                        else if (grasshoppers[i].position.x > boundX) 
                        {
                            grasshoppers[i].position.x = boundX;
                        }
                        if (grasshoppers[i].position.z < -boundY) 
                        {
                            grasshoppers[i].position.z = -boundY;
                        }
                        else if (grasshoppers[i].position.z > boundY) 
                        {
                            grasshoppers[i].position.z = boundY;
                        }
                        
                    break;
                    case GrasshopperState.HopPrep:

                        grasshoppers[i].timer += Time.deltaTime;

                        if(grasshoppers[i].timer >= JumpPrepTime)
                        {
                            grasshoppers[i].radians = (Mathf.Deg2Rad * (grasshoppers[i].direction));
                            grasshoppers[i].position.y += (GrasshopperJumpSpeed / 2) * Time.deltaTime; //TODO: Can probably precalculate ahead of time
                            grasshoppers[i].upwardVelocity = GrasshopperJumpSpeed;
                            grasshoppers[i].forwardVelocity = GrasshopperSpeed;
                            grasshoppers[i].state = GrasshopperState.Hop;
                        }

                    break;
                    case GrasshopperState.Hop:
                        float radians = grasshoppers[i].radians;
                        float velocity = grasshoppers[i].forwardVelocity * Time.deltaTime;

                        float deltaX = Mathf.Cos(radians) * velocity;
                        float deltaZ = Mathf.Sin(radians) * velocity;

                        // grasshoppers[i].position += (new Vector3(Mathf.Cos(radians) * velocity, 0,Mathf.Sin(radians) * velocity));
                        grasshoppers[i].position += new Vector3(deltaX, 0, deltaZ);
                        grasshoppers[i].forwardVelocity -= Time.deltaTime;

                        grasshoppers[i].position.y += grasshoppers[i].upwardVelocity * Time.deltaTime;
                        grasshoppers[i].upwardVelocity -= Time.deltaTime * Gravity;

                        if(grasshoppers[i].position.y <= ground || grasshoppers[i].forwardVelocity <= 0)
                        {
                            grasshoppers[i].position.y = ground;
                            grasshoppers[i].forwardVelocity = 0;
                            grasshoppers[i].state = GrasshopperState.Idle;
                        }
                        for (int b = 0; b < bubbleLength; b++)
                        {
                            float radius = BubbleSpawn.bubbles[b].radius;
                            if(Vector3.Distance(BubbleSpawn.bubbles[b].position,grasshoppers[i].position) < radius)
                            {
                                grasshoppers[i].bubbleParent = b;
                                grasshoppers[i].timer = 0;  
                                grasshoppers[i].forwardVelocity = grasshoppers[i].position.x;
                                grasshoppers[i].upwardVelocity = grasshoppers[i].position.y;
                                grasshoppers[i].jumpWaitTime = grasshoppers[i].position.z;
                                grasshoppers[i].state = GrasshopperState.BubbleCapture;  
                            }
                        }
                        if (grasshoppers[i].position.x < -boundX) 
                        {
                            // grasshoppers[i].position.x = -boundX;
                            grasshoppers[i].radians = grasshoppers[i].radians + 180;
                        }
                        else if (grasshoppers[i].position.x > boundX) 
                        {
                            // grasshoppers[i].position.x = boundX;
                            grasshoppers[i].radians = grasshoppers[i].radians + 180;
                        }
                        if (grasshoppers[i].position.z < -boundY) 
                        {
                            // grasshoppers[i].position.z = -boundY;
                            grasshoppers[i].radians = grasshoppers[i].radians + 180;
                        }
                        else if (grasshoppers[i].position.z > boundY) 
                        {
                            // grasshoppers[i].position.z = boundY;
                            grasshoppers[i].radians = grasshoppers[i].radians + 180;
                        }
                        if(grasshoppers[i].temp - ground >= 2)
                        {
                            grasshoppers[i].radians = grasshoppers[i].radians + 180;
                        }
                        grasshoppers[i].temp = ground;
                    break;
                    case GrasshopperState.BubbleCapture:

                        float timer = grasshoppers[i].timer + grasshoppers[i].seed;
                        int bubbleNum = grasshoppers[i].bubbleParent;
                        float bubRadius = BubbleSpawn.bubbles[bubbleNum].radius;
                        Vector3 initialPos = new Vector3(grasshoppers[i].forwardVelocity, grasshoppers[i].upwardVelocity,grasshoppers[i].jumpWaitTime);

                        float orbitX = BubbleSpawn.bubbles[bubbleNum].position.x + bubRadius * Mathf.Cos(timer * Mathf.Deg2Rad);
                        float orbitY = grasshoppers[i].position.y;  // Keep the same height (y-axis)
                        float orbitZ = BubbleSpawn.bubbles[bubbleNum].position.z + bubRadius * Mathf.Sin(timer * Mathf.Deg2Rad);

                        Vector3 orbitPos = new Vector3(orbitX,orbitY,orbitZ);
                        Vector3 finalOrbitPos = orbitPos + BubbleSpawn.bubbles[bubbleNum].delta;

                        grasshoppers[i].position = Vector3.Lerp(initialPos,finalOrbitPos,EaseOutCubic(grasshoppers[i].temp));
                        grasshoppers[i].direction += Time.deltaTime * 90;
                        grasshoppers[i].timer += Time.deltaTime * 90;
                        grasshoppers[i].temp += Time.deltaTime * 0.4f;
                        grasshoppers[i].spinDirection += Time.deltaTime * 60;

                        if(Vector3.Distance(BubbleSpawn.bubbles[bubbleNum].position,grasshoppers[i].position) > bubRadius * 2)
                        {
                            grasshoppers[i].bubbleParent = bubbleNum;
                            grasshoppers[i].timer = 0;  
                            grasshoppers[i].state = GrasshopperState.Vanquished;  
                            grasshoppers[i].position.y = -9999999f;
                        }

                    break;
                    
                    default:
                    break;
                }
                grasshoppers[i] = grasshopper;
            }
        }
        void OnDrawGizmos() 
        {
            if(grasshoppers == null)
                return;
            Color ogColor = Gizmos.color;
            foreach (var grasshopper in grasshoppers)
            {
                switch (grasshopper.state)
                {
                    case GrasshopperState.Idle:
                        Gizmos.color = Color.gray;
                    break;
                    case GrasshopperState.HopPrep:
                        Gizmos.color = Color.blue;
                    break;
                    case GrasshopperState.Hop:
                        Gizmos.color = Color.red;
                    break;
                    case GrasshopperState.BubbleCapture:
                        Gizmos.color = Color.magenta;
                    break;
                    default:
                    break;
                }
                Gizmos.DrawMesh(refMesh,-1,grasshopper.position,Quaternion.Euler(-90 + grasshopper.spinDirection,grasshopper.direction,-grasshopper.spinDirection));
                Gizmos.color = Color.white;
                Gizmos.DrawLine(grasshopper.position,grasshopper.position + Quaternion.Euler(0, grasshopper.direction, 0) * Vector3.forward * 2);
            }    
            Gizmos.color = ogColor;
        }
        float EaseOutElastic(float x)
        {
            float c4 = (2 * Mathf.PI) / 3;

            return x == 0 ? 0 : x == 1
            ? 1
            : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }
        float EaseOutCubic(float x)
        {
            return 1 - Mathf.Pow(1 - x, 3);
        }
    }
}