using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class VectorSerializationUtility
{
    public static Vector2Double ToVector2Double(this Vector2 vector, int digits = 2)
    {
        return new Vector2Double
        {
            x = Math.Round(vector.x, digits),
            y = Math.Round(vector.y, digits),
        };
    }

    public static Vector3Double ToVector3Double(this Vector3 vector, int digits = 2)
    {
        return new Vector3Double
        {
            x = Math.Round(vector.x, digits),
            y = Math.Round(vector.y, digits),
            z = Math.Round(vector.z, digits),
        };
    }
}

[System.Serializable]
public struct Vector2Double
{
    public double x;
    public double y;

    public Vector2Double(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 ToVector2() => new Vector2((float)x, (float)y);

    public static Vector2Double Lerp(Vector2Double a, Vector2Double b, float t)
    {
        return new Vector2Double(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t);
    }

    public static readonly Vector2Double zero = new Vector2Double(0, 0);
}

[System.Serializable]
public struct Vector3Double
{
    public double x;
    public double y;
    public double z;

    public Vector3Double(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3() => new Vector3((float)x, (float)y, (float)z);

    public static Vector3Double Lerp(Vector3Double a, Vector3Double b, float t)
    {
        return new Vector3Double(
            a.x + (b.x - a.x) * t,
            a.y + (b.y - a.y) * t,
            a.z + (b.z - a.z) * t);
    }

    public static readonly Vector3Double zero = new Vector3Double(0, 0, 0);
}
