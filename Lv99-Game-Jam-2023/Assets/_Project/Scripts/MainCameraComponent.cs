using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

public class MainCameraComponent : SingletonBehaviour<MainCameraComponent>
{
    [NonSerialized] public Vector3 HorizontalForwardDirection;
    [NonSerialized] public Vector3 HorizontalRightDirection;

    private void Start()
    {
        var _cinemachineImpulseManager = CinemachineImpulseManager.Instance;

        if (_cinemachineImpulseManager != null)
            _cinemachineImpulseManager.IgnoreTimeScale = true;
    }

    private void Update()
    {
        calculateDirections();
    }

    private void LateUpdate()
    {
        calculateDirections();
    }

    private void calculateDirections()
    {
        HorizontalForwardDirection = transform.forward;
        HorizontalForwardDirection.y = 0f;
        HorizontalForwardDirection.Normalize();

        HorizontalRightDirection = transform.right;
        HorizontalRightDirection.y = 0;
        HorizontalRightDirection.Normalize();
    }
}
