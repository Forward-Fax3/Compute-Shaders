using Unity.Mathematics;
using UnityEngine;

public static class OldFunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    private static readonly Function[] functions =
    {
        Wave,
        MultiWave,
        Ripple,
        ScalingSphere,
        TwistingSphere,
        Torus,
    };

    public enum FunctionName
    {
        Wave,
        MultiWave,
        Ripple,
        ScalingSphere,
        TwistingSphere,
        Torus,
    }

    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static Vector3 Wave(float u, float v, float t)
    {
        Vector3 returnVal;
        returnVal.x = u;
        returnVal.y = Mathf.Sin(Mathf.PI * (u + v + t));
        returnVal.z = v;
        return returnVal;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 returnVal;
        returnVal.x = u;
        returnVal.y = Mathf.Sin(Mathf.PI * (u + 0.5f * t));
        returnVal.y += 0.5f * Mathf.Sin(2.0f * Mathf.PI * (v + t));
        returnVal.y += Mathf.Sin(Mathf.PI * (u + v + 0.25f * t));
        returnVal.y *= 0.4f;
        returnVal.z = v;
        return returnVal;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        float d = Mathf.Sqrt(u * u + v * v);
        Vector3 returnVal;
        returnVal.x = u;
        returnVal.y = Mathf.Sin(Mathf.PI * (4.0f * d - t));
        returnVal.y /= (1.0f + 10.0f * d);
        returnVal.z = v;
        return returnVal;
    }

    public static Vector3 ScalingSphere(float u, float v, float t)
    {
        float r = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * t);
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);
        Vector3 returnVal;
        returnVal.x = s * Mathf.Sin(Mathf.PI * u);
        returnVal.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        returnVal.z = s * Mathf.Cos(Mathf.PI * u);
        return returnVal;
    }

    public static Vector3 TwistingSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Mathf.Sin(Mathf.PI * (6.0f * u + 4.0f * v + t));
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);
        Vector3 returnVal;
        returnVal.x = s * Mathf.Sin(Mathf.PI * u);
        returnVal.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        returnVal.z = s * Mathf.Cos(Mathf.PI * u);
        return returnVal;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Mathf.Sin(Mathf.PI * (6.0f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Mathf.Sin(Mathf.PI * (8.0f * u + 4.0f * v + 2.0f * t));
        float s = r1 + r2 * Mathf.Cos(Mathf.PI * v);
        Vector3 returnVal;
        returnVal.x = s * Mathf.Sin(Mathf.PI * u);
        returnVal.y = r2 * Mathf.Sin(Mathf.PI * v);
        returnVal.z = s * Mathf.Cos(Mathf.PI * u);
        return returnVal;
    }
}
