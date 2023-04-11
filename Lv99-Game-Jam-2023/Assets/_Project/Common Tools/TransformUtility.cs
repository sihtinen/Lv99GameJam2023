using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class TransformUtility
{
    public static void SetRotationAroundPivot(this Transform transform, Vector3 pivotPoint, Quaternion rotation)
    {
        transform.SetPositionAndRotation(
            rotation * (transform.position - pivotPoint) + pivotPoint, 
            rotation);
    }
}
