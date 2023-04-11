using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(200)]
public class VFXPrefab : MonoBehaviour
{
    [Header("Pool Settings")]
    [Min(8)]
    public int PoolInitialSize = 16;
    [Min(4)]
    public int PoolExpandBatchSize = 8;

    [Header("Instance Settings")]
    [SerializeField] private float m_lifeTime = 5.0f;

    [NonSerialized] public IVFXParentParentProxy ParentProxy = null;

    private List<Transform> m_allTransforms = new List<Transform>();

    public void ActivateLifetime()
    {
        gameObject.SetActiveOptimized(true);
        StartCoroutine(coroutine_lifeTime());
    }

    private void OnDisable()
    {
        ParentProxy = null;
    }

    private void OnDestroy()
    {
        ParentProxy = null;
    }

    public void SetObjectLayer(string layerName)
    {
        if (m_allTransforms.Count == 0)
            GetComponentsInChildren(includeInactive: true, result: m_allTransforms);

        int _layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < m_allTransforms.Count; i++)
            m_allTransforms[i].gameObject.layer = _layer;
    }

    private void LateUpdate()
    {
        if (this != null && transform != null && ParentProxy != null)
            transform.SetPositionAndRotation(ParentProxy.GetPosition(), ParentProxy.GetRotation());
    }

    private IEnumerator coroutine_lifeTime()
    {
        float _timer = 0f;

        while (_timer < m_lifeTime)
        {
            yield return null;
            _timer += Time.deltaTime;
        }

        ParentProxy = null;

        if (VFXPrefabPool.Instance != null)
            VFXPrefabPool.Instance.ReturnToPool(this);
        else
            gameObject.SetActiveOptimized(false);
    }

    public interface IVFXParentParentProxy
    {
        public Vector3 GetPosition();
        public Quaternion GetRotation();
    }
}
