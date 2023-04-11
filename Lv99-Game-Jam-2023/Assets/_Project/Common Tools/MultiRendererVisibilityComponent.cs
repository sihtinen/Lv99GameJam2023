using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class MultiRendererVisibilityComponent : MonoBehaviour
{
    public UnityEvent OnAnyRendererBecameVisible = new UnityEvent();
    public UnityEvent OnAllRenderersBecameInvisible = new UnityEvent();

    private int m_visibleRenderersCount = 0;

    private void Start()
    {
        if (m_visibleRenderersCount == 0)
            OnAllRenderersBecameInvisible?.Invoke();
    }

    public void OnRendererBecameVisible()
    {
        if (m_visibleRenderersCount == 0)
            OnAnyRendererBecameVisible?.Invoke();

        m_visibleRenderersCount++;
    }

    public void OnRendererBecameInvisible()
    {
        if (m_visibleRenderersCount == 0)
            return;

        m_visibleRenderersCount--;

        if (m_visibleRenderersCount == 0)
            OnAllRenderersBecameInvisible?.Invoke();
    }
}
