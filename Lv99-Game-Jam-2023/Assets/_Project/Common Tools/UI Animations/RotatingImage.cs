using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RotatingImage : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField, Min(0.1f)] private float m_animationLength = 0.1f;
    [SerializeField] private AnimationCurve m_eulerRotateCurveX = new AnimationCurve();
    [SerializeField] private AnimationCurve m_eulerRotateCurveY = new AnimationCurve();
    [SerializeField] private AnimationCurve m_eulerRotateCurveZ = new AnimationCurve();

    private float m_currentAnimTime = 0f;

    private void OnEnable()
    {
        if (Application.isPlaying == false)
            return;

        transform.localEulerAngles = Vector3.zero;
        m_currentAnimTime = 0f;
    }

    private void Update()
    {
        m_currentAnimTime += Time.unscaledDeltaTime;

        if (m_currentAnimTime > m_animationLength)
            m_currentAnimTime -= m_animationLength;

        float _animPos = m_currentAnimTime / m_animationLength;

        transform.localEulerAngles = new Vector3(
            m_eulerRotateCurveX.Evaluate(_animPos), 
            m_eulerRotateCurveY.Evaluate(_animPos),
            m_eulerRotateCurveZ.Evaluate(_animPos));
    }
}
