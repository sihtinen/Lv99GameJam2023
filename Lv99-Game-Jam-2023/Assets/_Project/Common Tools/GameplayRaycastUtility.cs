using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameplayRaycastUtility
{
    public struct Results
    {
        public bool HitFound;
        public Vector3 HitWorldPoint;
        public Vector3 HitNormal;
        public GameObject HitObject;
    }

    public static Results Raycast(
        Vector3 from, 
        Vector3 to, 
        LayerMask hitLayers, 
        List<GameObject> ignoredObjects = null,
        bool collideWithTriggers = false)
    {
        Vector3 _diff = to - from;

        Results _results = new Results()
        {
            HitFound = false,
            HitWorldPoint = to,
            HitNormal = _diff.normalized,
            HitObject = null
        };

        var _triggerInteractionMode = collideWithTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

        int _hitCount = Physics.RaycastNonAlloc(
            from,
            _diff.normalized,
            PhysicsUtility.CachedRaycastHits,
            _diff.magnitude + 0.01f,
            hitLayers,
            _triggerInteractionMode);

        if (_hitCount == 0) 
            return _results;

        RaycastHitExtensions.RaycastHitResultWrapper _resultsWrapper = PhysicsUtility.CachedRaycastHits.GetClosestHit(_hitCount, ignoredObjects);
        
        if (_resultsWrapper.HitFound == false) 
            return _results;

        _results.HitFound = true;
        _results.HitWorldPoint = _resultsWrapper.RaycastHit.point;
        _results.HitNormal = _resultsWrapper.RaycastHit.normal;
        _results.HitObject = _resultsWrapper.RaycastHit.collider.gameObject;

        return _results;
    }
}
