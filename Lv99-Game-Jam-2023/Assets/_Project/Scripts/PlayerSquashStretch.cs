using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerSquashStretch : MonoBehaviour
{
    [Header("Vertical Velocity Squash Settings")]
    [SerializeField] private float m_maxVerticalVelocity = 2.0f;
    [SerializeField] private float m_verticalVelocity_HorizontalScaleOffset = 0f;
    [SerializeField] private float m_verticalVelocity_VerticalScaleOffset = 0f;

    [Header("Landing Squash Settings")]
    [SerializeField] private float m_minLandingSquashVelocity = -1f;
    [SerializeField] private float m_landingSquashDuration = 0.6f;
    [SerializeField] private float m_landingSquash_HorizontalScaleOffset = 0f;
    [SerializeField] private float m_landingSquash_VerticalScaleOffset = 0f;
    [SerializeField] private AnimationCurve m_landingSquashCurve = new AnimationCurve();

    [Header("Object References")]
    [SerializeField] private PlayerMoveComponent m_moveComponent = null;

    private float m_currentLandingHorizontalScaleOffset = 0f;
    private float m_currentLandingVerticalScaleOffset = 0f;

    private void Start()
    {
        if (m_moveComponent != null)
            m_moveComponent.OnLanded.AddListener(this.onLanded);
    }

    private void onLanded()
    {
        if (m_moveComponent.CurrentVerticalVelocity < m_minLandingSquashVelocity)
            StartCoroutine(coroutine_landingSquash());
    }

    private void OnDisable()
    {
        if (Application.isPlaying)
            transform.localScale = Vector3.one;
    }

    private void Update()
    {
        if (m_moveComponent == null)
            return;

        float _verticalVelocity = Mathf.Min(Mathf.Abs(m_moveComponent.CurrentVerticalVelocity), m_maxVerticalVelocity);
        float _verticalVelocityNormalized = _verticalVelocity / m_maxVerticalVelocity;

        if (m_moveComponent.IsGrounded /*|| m_moveComponent.CurrentVerticalVelocity > 0f*/)
            _verticalVelocityNormalized = 0f;

        float _horizontalScaleOffset = _verticalVelocityNormalized * m_verticalVelocity_HorizontalScaleOffset;
        float _verticalScaleOffset = _verticalVelocityNormalized * m_verticalVelocity_VerticalScaleOffset;

        transform.localScale = new Vector3(
            1f + _horizontalScaleOffset + m_currentLandingHorizontalScaleOffset,
            1f + _verticalScaleOffset + m_currentLandingVerticalScaleOffset,
            1f + _horizontalScaleOffset + m_currentLandingHorizontalScaleOffset);
    }

    private IEnumerator coroutine_landingSquash()
    {
        float _timer = 0f;

        while (_timer < m_landingSquashDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _animTime = _timer / m_landingSquashDuration;
            float _curveVal = m_landingSquashCurve.Evaluate(_animTime);

            m_currentLandingHorizontalScaleOffset = _curveVal * m_landingSquash_HorizontalScaleOffset;
            m_currentLandingVerticalScaleOffset = _curveVal * m_landingSquash_VerticalScaleOffset;
        }

        m_currentLandingHorizontalScaleOffset = 0f;
        m_currentLandingVerticalScaleOffset = 0f;
    }
}
