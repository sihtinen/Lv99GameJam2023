using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class PoolableBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private Transform m_originalParent = null;

    public void InitializePoolable()
    {
        m_originalParent = transform.parent;

        resetAndClearBindings();

        gameObject.SetActiveOptimized(false);
    }

    public void ResetAndReturnToPool()
    {
        resetAndClearBindings();

        gameObject.SetActiveOptimized(false);

        transform.SetParent(m_originalParent);

        PoolableBehaviourPool<T>.Release(this as T);
    }

    protected abstract void resetAndClearBindings();
}
