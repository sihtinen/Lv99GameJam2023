using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class GameObjectEventComponent : MonoBehaviour
{
    public UnityEvent OnEnabledEvent = new UnityEvent();
    public UnityEvent OnDisabledEvent = new UnityEvent();
    public UnityEvent OnDestroyEvent = new UnityEvent();

    private void OnEnable() => OnEnabledEvent?.Invoke();
    private void OnDisable() => OnDisabledEvent?.Invoke();
    private void OnDestroy()
    {
        if (Application.isPlaying)
            OnDestroyEvent?.Invoke();
    }
}
