using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasingUtil
{
    public static float EaseOutBounce(float x)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (x < 1 / d1)
            return n1 * x * x;
        else if (x < 2 / d1)
            return n1 * (x -= 1.5f / d1) * x + 0.75f;
        
        else if (x < 2.5 / d1)
            return n1 * (x -= 2.25f / d1) * x + 0.9375f;
        else
            return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    }
    public static float EaseInOutQuad(float x)
    {
        return x < 0.5 ? 2 * x * x : 1 - Mathf.Pow(-2 * x + 2, 2) / 2;
    }
    public static float EaseOutQuad(float x) 
    {
        return 1 - (1 - x) * (1 - x);
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    public static float EaseOutQuart(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4); // originally 4
    }
    public static float EaseOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2);
    }
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }
    public static float EaseOutExpo(float x) 
    {
        return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
    }
    public static float EaseInOutBack(float x) 
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
        ? (Mathf.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        : (Mathf.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }
    public static float EaseInQuad(float x)
    {
        return x * x;
    }
    public static float EaseInBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return c3 * x * x * x - c1 * x * x;

    }
    public static float EaseOutElastic(float x)
    {
        float c4 = (2 * Mathf.PI) / 3;
        return x == 0 ? 0 : x == 1 ? 1 : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
    }
    public static float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }
}
