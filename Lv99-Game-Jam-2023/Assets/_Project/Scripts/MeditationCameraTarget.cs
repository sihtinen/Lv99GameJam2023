using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MeditationCameraTarget : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_moveSpeed = 2f;
    [SerializeField] private Vector2 m_horizontalMoveLimits = Vector2.one;
    [SerializeField] private Vector2 m_depthMoveLimits = Vector2.one;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_moveInputRef = null;
    [SerializeField] private MeditationPoint m_meditationPoint = null;

    private Vector3 m_defaultPosition;
    private Vector3 m_offset = Vector3.zero;

    private void Awake()
    {
        m_meditationPoint.OnBecameActive += this.onBecameActive;
        m_defaultPosition = transform.position;
    }

    private void OnDestroy()
    {
        if (m_meditationPoint != null)
            m_meditationPoint.OnBecameActive -= this.onBecameActive;
    }

    private void onBecameActive()
    {
        transform.position = m_defaultPosition;
        m_offset = Vector3.zero;
    }

    private void Update()
    {
        if (m_meditationPoint.IsActive == false)
            return;

        var _input = m_moveInputRef.action.ReadValue<Vector2>();
        m_offset.x += _input.x * Time.deltaTime * m_moveSpeed;
        m_offset.y += _input.y * Time.deltaTime * m_moveSpeed;

        m_offset.x = Mathf.Clamp(m_offset.x, m_horizontalMoveLimits.x, m_horizontalMoveLimits.y);
        m_offset.y = Mathf.Clamp(m_offset.y, m_depthMoveLimits.x, m_depthMoveLimits.y);

        transform.position = m_defaultPosition 
            + m_offset.x * MainCameraComponent.Instance.HorizontalRightDirection 
            + m_offset.y * MainCameraComponent.Instance.HorizontalForwardDirection;
    }
}
