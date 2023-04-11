using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

public static class ObjectCollection<T> where T : class
{
    public static event Action<T> OnObjectRegistered = null;
    public static event Action<T> OnObjectUnregistered = null;
    public static event Action OnObjectListUpdated = null;

    public static ReadOnlyCollection<T> AllObjects => m_allObjects.AsReadOnly();
    private static List<T> m_allObjects = new List<T>();

    public static void RegisterObject(T instance)
    {
        bool _listUpdated = cleanupList();

        if (m_allObjects.Contains(instance))
        {
            if (_listUpdated)
                OnObjectListUpdated?.Invoke();

            return;
        }

        m_allObjects.Add(instance);
        OnObjectRegistered?.Invoke(instance);
        OnObjectListUpdated?.Invoke();
    }

    public static void UnregisterObject(T instance)
    {
        bool _listUpdated = cleanupList();

        if (m_allObjects.Contains(instance) == false)
        {
            if (_listUpdated)
                OnObjectListUpdated?.Invoke();

            return;
        }

        m_allObjects.Remove(instance);
        OnObjectUnregistered?.Invoke(instance);
        OnObjectListUpdated?.Invoke();
    }

    private static bool cleanupList()
    {
        bool _listUpdated = false;

        for (int i = m_allObjects.Count; i --> 0;)
        {
            if (m_allObjects[i] == null)
            {
                m_allObjects.RemoveAt(i);
                _listUpdated = true;
            }
        }

        return _listUpdated;
    }
}
