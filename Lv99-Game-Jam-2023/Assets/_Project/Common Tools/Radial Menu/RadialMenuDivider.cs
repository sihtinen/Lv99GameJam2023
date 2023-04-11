using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RadialMenuDivider : PoolableBehaviour<RadialMenuDivider>
{
    [NonSerialized] public RectTransform RectTransformComponent = null;

    private void Awake()
    {
        RectTransformComponent = transform as RectTransform;
    }

    protected override void resetAndClearBindings()
    {

    }
}
