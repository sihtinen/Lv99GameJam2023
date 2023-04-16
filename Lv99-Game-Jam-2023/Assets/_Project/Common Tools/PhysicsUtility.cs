using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public static class PhysicsUtility
{
    public static RaycastHit CachedRaycastHit_Single = default;
    public static readonly RaycastHit[] CachedRaycastHits = new RaycastHit[256];
    public static readonly List<RaycastHit> CachedRaycastHitList = new List<RaycastHit>();

    private static bool m_groundLayerMaskInitialized = false;
    private static LayerMask m_groundLayerMask;
    //private static Vector3 m_previousSamplePosition = Vector3.zero;
    //private static Vector3 m_previousGroundResult = Vector3.zero;
    //private static RaycastHitExtensions.RaycastHitResultWrapper m_previousHit;

    public static LayerMask GroundLayerMask
    {
        get
        {
            if (m_groundLayerMaskInitialized == false)
            {
                m_groundLayerMask = LayerMask.GetMask("Default");
                m_groundLayerMaskInitialized = true;
            }

            return m_groundLayerMask;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetGroundPosition(this Vector3 position, float startVerticalOffset = 1000f, List<Collider> ignoredColliders = null)
    {
        //if (position == m_previousSamplePosition)
        //    return m_previousGroundResult;

        if (ignoredColliders == null || ignoredColliders.Count == 0)
        {
            bool _hitFound = Physics.Raycast(
                position + new Vector3(0f, startVerticalOffset, 0f),
                Vector3.down,
                out CachedRaycastHit_Single,
                2000f,
                GroundLayerMask,
                QueryTriggerInteraction.Ignore);

            if (_hitFound)
                return CachedRaycastHit_Single.point;

            return Vector3.zero;
        }

        int _hitCount = Physics.RaycastNonAlloc(
            position + new Vector3(0f, startVerticalOffset, 0f),
            Vector3.down,
            CachedRaycastHits,
            2000f,
            GroundLayerMask,
            QueryTriggerInteraction.Ignore);

        if (_hitCount == 0)
            return position;

        var _closestHit = CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);
        return _closestHit.RaycastHit.point;

        //m_previousSamplePosition = position;
        //m_previousHit = CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);

        //if (m_previousHit.HitFound)
        //    m_previousGroundResult =  m_previousHit.RaycastHit.point;
        //else
        //    m_previousGroundResult = position;

        //return m_previousGroundResult;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 GetGroundNormal(this Vector3 position, float startVerticalOffset = 1000f, List<Collider> ignoredColliders = null)
    {
        //if (position == m_previousSamplePosition)
        //    return m_previousHit.RaycastHit.normal;

        if (ignoredColliders == null || ignoredColliders.Count == 0)
        {
            bool _hitFound = Physics.Raycast(
                position + new Vector3(0f, startVerticalOffset, 0f),
                Vector3.down,
                out CachedRaycastHit_Single,
                2000f,
                GroundLayerMask,
                QueryTriggerInteraction.Ignore);

            if (_hitFound)
                return CachedRaycastHit_Single.normal;

            return Vector3.up;
        }

        int _hitCount = Physics.RaycastNonAlloc(
            position + new Vector3(0f, startVerticalOffset, 0f),
            Vector3.down,
            CachedRaycastHits,
            2000f,
            GroundLayerMask,
            QueryTriggerInteraction.Ignore);

        if (_hitCount == 0)
            return Vector3.up;

        var _closestHit = CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);
        return _closestHit.RaycastHit.normal;

        //m_previousSamplePosition = position;
        //m_previousHit = CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);
        //return m_previousHit.RaycastHit.normal;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RaycastHitExtensions.RaycastHitResultWrapper GetGroundHit(this Vector3 position, float startVerticalOffset = 1000f, float sphereRadius = 0.1f, List<Collider> ignoredColliders = null)
    {
        //if (position == m_previousSamplePosition)
        //    return m_previousHit;

        //m_previousSamplePosition = position;

        if (ignoredColliders == null || ignoredColliders.Count == 0)
        {
            bool _hitFound = Physics.SphereCast(
                position + new Vector3(0f, startVerticalOffset, 0f),
                sphereRadius,
                Vector3.down,
                out CachedRaycastHit_Single,
                2000f,
                GroundLayerMask,
                QueryTriggerInteraction.Ignore);

            if (_hitFound)
            {
                return new RaycastHitExtensions.RaycastHitResultWrapper
                {
                    HitFound = true,
                    RaycastHit = CachedRaycastHit_Single,
                };
            }

            return new RaycastHitExtensions.RaycastHitResultWrapper
            {
                HitFound = false,
                RaycastHit = default,
            };
        }

        int _hitCount = Physics.SphereCastNonAlloc(
            position + new Vector3(0f, startVerticalOffset, 0f),
            sphereRadius,
            Vector3.down,
            CachedRaycastHits,
            2000f,
            GroundLayerMask,
            QueryTriggerInteraction.Ignore);

        if (_hitCount == 0)
        {
            return new RaycastHitExtensions.RaycastHitResultWrapper
            {
                HitFound = false,
                RaycastHit = default,
            };
        }

        return CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);

        //return m_previousHit;

        //m_previousSamplePosition = position;
        //m_previousHit = CachedRaycastHits.GetClosestHit(_hitCount, ignoredColliders);

        //return m_previousHit;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ForceToTorque(Rigidbody rigidbody, Vector3 force, Vector3 position, ForceMode forceMode = ForceMode.Force)
    {
        Vector3 t = Vector3.Cross(position - rigidbody.worldCenterOfMass, force);
        ToDeltaTorque(rigidbody, ref t, forceMode);
        return t;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ToDeltaTorque(Rigidbody rigidbody, ref Vector3 torque, ForceMode forceMode)
    {
        bool continuous = forceMode == ForceMode.Force || forceMode == ForceMode.Acceleration;
        bool useMass = forceMode == ForceMode.Force || forceMode == ForceMode.Impulse;

        if (continuous) 
            torque *= Time.fixedDeltaTime;

        if (useMass)
            ApplyInertiaTensor(rigidbody, ref torque);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplyInertiaTensor(Rigidbody rigidbody, ref Vector3 v)
    {
        v = rigidbody.rotation * Div(Quaternion.Inverse(rigidbody.rotation) * v, rigidbody.inertiaTensor);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 Div(Vector3 v, Vector3 v2)
    {
        return new Vector3(v.x / v2.x, v.y / v2.y, v.z / v2.z);
    }
}
