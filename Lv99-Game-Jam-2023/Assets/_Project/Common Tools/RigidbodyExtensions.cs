using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class RigidbodyExtensions
{
    public static void TorqueLookToward(this Rigidbody rigidbody, Vector3 direction, float force, float dampening = 0f)
    {
        Vector3 _forward = rigidbody.transform.forward;
        Vector3 _cross = Vector3.Cross(_forward, direction);

        float _angleDiff = Vector3.Angle(_forward, direction);
        _angleDiff = Mathf.Sqrt(_angleDiff);

        Vector3 _torque = _cross * _angleDiff * force * rigidbody.mass;
        _torque += -rigidbody.angularVelocity * dampening * rigidbody.mass;

        rigidbody.AddTorque(_torque, ForceMode.Force);
    }

    public static void TorqueUpToward(this Rigidbody rigidbody, Vector3 direction, float force, float dampening = 0f)
    {
        Vector3 _up = rigidbody.transform.up;
        Vector3 _cross = Vector3.Cross(_up, direction);

        float _angleDiff = Vector3.Angle(rigidbody.transform.up, direction);
        _angleDiff = Mathf.Sqrt(_angleDiff);

        Vector3 _torque = _cross * _angleDiff * force * rigidbody.mass;
        _torque += -rigidbody.angularVelocity * dampening * rigidbody.mass;

        rigidbody.AddTorque(_torque, ForceMode.Force);
    }
}
