using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PointerEventComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent OnPointerEnterEvent = null;
    public UnityEvent OnPointerExitEvent = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke();
    }
}
