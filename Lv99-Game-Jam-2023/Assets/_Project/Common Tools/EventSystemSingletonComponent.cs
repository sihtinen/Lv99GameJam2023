using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingletonComponent : SingletonBehaviour<EventSystemSingletonComponent>
{
    public EventSystem EventSystemComponent { get; private set; } = null;

    protected override void Awake()
    {
        base.Awake();
        EventSystemComponent = GetComponent<EventSystem>();
    }
}