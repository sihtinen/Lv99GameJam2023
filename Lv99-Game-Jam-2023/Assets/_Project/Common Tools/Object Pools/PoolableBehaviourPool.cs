using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(-100)]
public abstract class PoolableBehaviourPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Object Pool Settings")]
    [SerializeField] private T m_objectPrefab = null;
    [SerializeField] private int m_initialPoolSize = 64;
    [SerializeField] private int m_additionalInstantiateBatchSize = 16;
    [SerializeField] private Transform m_parentTransform = null;

    private Stack<T> m_unusedObjectsStack = null;
    private List<T> m_usedObjectsList = null;

    private static PoolableBehaviourPool<T> m_instance = null;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_instance = this;

        m_unusedObjectsStack = new Stack<T>(m_initialPoolSize);
        m_usedObjectsList = new List<T>(m_initialPoolSize);

        instantiateNewObjects(m_initialPoolSize);
    }

    private void OnDestroy()
    {
        if (m_instance != this)
            return;

        if (m_unusedObjectsStack.Count > 0)
        {
            while (m_unusedObjectsStack.Count > 0)
            {
                T _instance = m_unusedObjectsStack.Pop();
                if (_instance != null && _instance.gameObject != null)
                    Destroy(_instance.gameObject);
            }
        }
        if (m_usedObjectsList.Count > 0)
        {
            for (int i = m_usedObjectsList.Count; i --> 0;)
            {
                T _instance = m_usedObjectsList[i];
                if (_instance != null && _instance.gameObject != null)
                    Destroy(_instance.gameObject);
            }
        }
    }

    private void instantiateNewObjects(int instantiateAmount)
    {
        if (m_objectPrefab == null)
        {
            Debug.LogError($"{gameObject.name} - {GetType().Name}.instantiateNewObjects(): object prefab is null", gameObject);
            return;
        }

        for (int i = 0; i < instantiateAmount; i++)
        {
            GameObject _newObject = Instantiate(m_objectPrefab.gameObject, m_parentTransform);

            if (_newObject.TryGetComponent(out PoolableBehaviour<T> _poolableComponent))
            {
                _poolableComponent.InitializePoolable();
                m_unusedObjectsStack.Push(_poolableComponent as T);
            }
            else
            {
                Debug.LogWarning($"{GetType().Name}.instantiateNewObjects(): poolable behaviour component of type {typeof(T).Name} not found on prefab - destroying instantiated object");
                Destroy(_newObject);
            }
        }
    }

    public static T Get()
    {
        if (m_instance == null)
        {
            Debug.LogError($"PoolableBehaviourPool.Get(): pool instance of type {nameof(T)} is null!");
            return null;
        }

        return m_instance.internal_get();
    }

    private T internal_get()
    {
        if (m_unusedObjectsStack.Count == 0)
            instantiateNewObjects(m_additionalInstantiateBatchSize);

        T _instance = m_unusedObjectsStack.Pop();
        m_usedObjectsList.Add(_instance);
        return _instance;
    }

    public static void Release(T instance)
    {
        if (m_instance == null)
        {
            Debug.LogError($"PoolableBehaviourPool.Release(): pool instance of type {nameof(T)} is null!");
            return;
        }

        m_instance.internal_release(instance);
    }

    private void internal_release(T instance)
    {
        m_usedObjectsList.Remove(instance);
        m_unusedObjectsStack.Push(instance);
    }

    public static void ResetUsedObjects()
    {
        if (m_instance == null)
        {
            Debug.LogError($"PoolableBehaviourPool.ResetUsedObjects(): pool instance of type {nameof(T)} is null!");
            return;
        }

        m_instance.internal_resetUsedObjects();
    }

    private void internal_resetUsedObjects()
    {
        if (m_usedObjectsList.Count > 0)
        {
            for (int i = m_usedObjectsList.Count; i-- > 0;)
            {
                PoolableBehaviour<T> _poolable = m_usedObjectsList[i] as PoolableBehaviour<T>;
                _poolable.ResetAndReturnToPool();
            }
        }
    }
}