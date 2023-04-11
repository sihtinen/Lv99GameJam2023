using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using UnityEngine;

public static class RaycastHitExtensions
{
    public struct RaycastHitResultWrapper
    {
        public bool HitFound;
        public RaycastHit RaycastHit;
    }

    private static List<GameObject> m_cachedGameObjectList = new List<GameObject>();

    private static readonly RaycastHit EMPTY_HIT = default;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RaycastHitResultWrapper GetClosestHit(this RaycastHit[] array, int hitCount, GameObject ignoreObject = null)
    {
        if (ignoreObject == null)
            return array.GetClosestHit(hitCount, ignoreObjects: null);
        else
        {
            m_cachedGameObjectList.Clear();
            m_cachedGameObjectList.Add(ignoreObject);
            return array.GetClosestHit(hitCount, m_cachedGameObjectList);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RaycastHitResultWrapper GetClosestHit(this RaycastHit[] array, int hitCount, List<Collider> ignoredColliders = null)
    {
        RaycastHitResultWrapper _result = new RaycastHitResultWrapper()
        {
            HitFound = false,
            RaycastHit = EMPTY_HIT,
        };

        if (hitCount == 1)
        {
            RaycastHit _raycastHit = array[0];

            if (_raycastHit.collider == null)
                return _result;

            if (Mathf.Approximately(_raycastHit.point.x, 0f) &&
                Mathf.Approximately(_raycastHit.point.y, 0f) &&
                Mathf.Approximately(_raycastHit.point.z, 0f))
                return _result;

            if (ignoredColliders != null && ignoredColliders.Count > 0)
            {
                bool _hitIgnoredObject = ignoredColliders.Contains(_raycastHit.collider);

                if (_hitIgnoredObject)
                    return _result;
            }

            _result.HitFound = true;
            _result.RaycastHit = _raycastHit;

            return _result;
        }

        float _closestDistance = float.MaxValue;
        RaycastHit? _closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit _currentHit = array[i];

            if (_currentHit.distance > _closestDistance)
                continue;

            if (Mathf.Approximately(_currentHit.point.x, 0f) &&
                Mathf.Approximately(_currentHit.point.y, 0f) &&
                Mathf.Approximately(_currentHit.point.z, 0f))
                continue;

            if (_currentHit.collider == null)
                continue;

            if (ignoredColliders != null && ignoredColliders.Count > 0)
            {
                if (ignoredColliders.Contains(_currentHit.collider))
                    continue;
            }

            _result.HitFound = true;
            _closest = _currentHit;
            _closestDistance = _currentHit.distance;
        }

        if (_closest.HasValue && _closest.Value.point != Vector3.zero)
            _result.RaycastHit = _closest.Value;

        return _result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RaycastHitResultWrapper GetClosestHit(this RaycastHit[] array, int hitCount, List<GameObject> ignoreObjects = null)
    {
        RaycastHitResultWrapper _result = new RaycastHitResultWrapper()
        {
            HitFound = false,
            RaycastHit = EMPTY_HIT,
        };

        if (hitCount == 1)
        {
            RaycastHit _raycastHit = array[0];

            if (_raycastHit.collider == null)
                return _result;

            if (Mathf.Approximately(_raycastHit.point.x, 0f) &&
                Mathf.Approximately(_raycastHit.point.y, 0f) &&
                Mathf.Approximately(_raycastHit.point.z, 0f))
                return _result;

            if (ignoreObjects != null && ignoreObjects.Count > 0)
            {
                bool _hitIgnoredObject = ignoreObjects.Contains(_raycastHit.collider.gameObject);

                if (_hitIgnoredObject)
                    return _result;
            }

            _result.HitFound = true;
            _result.RaycastHit = _raycastHit;

            return _result;
        }

        float _closestDistance = float.MaxValue;
        RaycastHit? _closest = null;

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit _currentHit = array[i];

            if (_currentHit.distance > _closestDistance)
                continue;

            if (Mathf.Approximately(_currentHit.point.x, 0f) &&
                Mathf.Approximately(_currentHit.point.y, 0f) &&
                Mathf.Approximately(_currentHit.point.z, 0f))
                continue;

            if (_currentHit.collider == null)
                continue;

            if (ignoreObjects != null && ignoreObjects.Count > 0)
            {
                if (ignoreObjects.Contains(_currentHit.collider.gameObject))
                    continue;
            }

            _result.HitFound = true;
            _closest = _currentHit;
            _closestDistance = _currentHit.distance;
        }

        if (_closest.HasValue)
            _result.RaycastHit = _closest.Value;

        return _result;
    }
}
