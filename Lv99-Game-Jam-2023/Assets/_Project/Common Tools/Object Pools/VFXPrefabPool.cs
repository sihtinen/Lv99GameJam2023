using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VFXPrefabPool : SingletonBehaviour<VFXPrefabPool>
{
    private Dictionary<string, Stack<VFXPrefab>> m_prefabPools = new Dictionary<string, Stack<VFXPrefab>>();

    public static VFXPrefab GetPrefabInstance(VFXPrefab prefab) => Instance.GetInstance(prefab);

    public VFXPrefab GetInstance(VFXPrefab prefab)
    {
        if (m_prefabPools.ContainsKey(prefab.gameObject.name) == false)
            initializePool(prefab);

        var _targetStack = m_prefabPools[prefab.gameObject.name];

        if (_targetStack.Count == 0)
        {
            for (int i = 0; i < prefab.PoolExpandBatchSize; i++)
                instantiateNewPrefab(prefab, _targetStack);
        }

        return _targetStack.Pop();
    }

    private static void instantiateNewPrefab(VFXPrefab prefab, Stack<VFXPrefab> targetCollection)
    {
        var _go = Instantiate(prefab.gameObject, null);
        _go.name = prefab.gameObject.name;
        _go.SetActiveOptimized(false);
        DontDestroyOnLoad(_go);
        targetCollection.Push(_go.GetComponent<VFXPrefab>());
    }

    public void ReturnToPool(VFXPrefab instance)
    {
        instance.ParentProxy = null;
        instance.gameObject.SetActiveOptimized(false);

        var _list = m_prefabPools[instance.gameObject.name];
        _list.Push(instance);
    }

    private void initializePool(VFXPrefab prefab)
    {
        Stack<VFXPrefab> _newStack = new Stack<VFXPrefab>();

        for (int i = 0; i < prefab.PoolInitialSize; i++)
            instantiateNewPrefab(prefab, _newStack);

        m_prefabPools.Add(prefab.gameObject.name, _newStack);
    }
}
