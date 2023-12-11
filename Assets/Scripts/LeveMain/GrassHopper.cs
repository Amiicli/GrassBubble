using UnityEngine;
public struct Grasshopper
{
    public Vector3 position;
    public Vector3 color;
    public float scale;
    public float direction;
    public float spinDirection;
    public float radians;          // reuse variables to save space
    public float forwardVelocity; // init x 
    public float upwardVelocity; // init y
    public float jumpWaitTime; // init z
    public float temp;
    public GrasshopperState state;
    public float timer;
    public int bubbleParent;
    public int seed;
    public float frame;

    public Grasshopper(Vector3 position, int state)
    {
        color = new Vector3(1,1,1);
        forwardVelocity = 0;
        upwardVelocity = 0;
        radians = 0;
        spinDirection = 0;
        direction = UnityEngine.Random.Range(0,360);
        timer = 0;
        jumpWaitTime = UnityEngine.Random.Range(1f,4f);
        seed = UnityEngine.Random.Range(0,1000);
        this.position = position;
        this.state = GrasshopperState.Idle;
        bubbleParent = -1;
        temp = 0;
        scale = 1;
        frame = 0;
    }
    public static int GetGrasshopperSize()
    {
        int floatSize = sizeof(float);
        int intSize = sizeof(int);
        int vector3Size = sizeof(float) * 3;
        int enumSize = sizeof(GrasshopperState);
        int grasshopperStructSize = (vector3Size * 2) + (floatSize * 10) + (intSize * 2) + enumSize;
        return grasshopperStructSize;
    }
}
public enum GrasshopperState
{
    Idle = 0,
    HopPrep = 1,
    Hop = 2,
    BubbleCapture = 3,
    Vanquished = 4,
}