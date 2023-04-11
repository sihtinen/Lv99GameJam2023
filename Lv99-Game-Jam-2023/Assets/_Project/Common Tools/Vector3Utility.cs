using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class Vector3Utility
{
    public static Vector3 GetClosestPointOnFiniteLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 _line = lineEnd - lineStart;
        float _lineLength = _line.magnitude;
        _line.Normalize();
        float _projectedLength = Mathf.Clamp(Vector3.Dot(point - lineStart, _line), 0f, _lineLength);
        return lineStart + _projectedLength * _line;
    }
}
