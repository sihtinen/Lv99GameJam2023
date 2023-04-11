using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    [Header("Singleton Settings")]
    [SerializeField, Tooltip("Game object doesn't get destroyed when scene unloads")] private bool m_isPersistentObject = false;

    public static T Instance { get; protected set; } = null;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

        if (m_isPersistentObject)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
