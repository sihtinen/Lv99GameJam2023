using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameFeelTester : MonoBehaviour
{
    [Header("Hit Stop Settings")]
    [SerializeField] private float m_hitStopDuration = 0.2f;
    [SerializeField] private float m_hitStopTimeScale = 0.2f;

    [Header("Screen Shake Settings")]
    [SerializeField] private float m_screenShakeIntensity = 0.5f;

    private Cinemachine.CinemachineImpulseSource m_impulseSource = null;

    private void Awake()
    {
        TryGetComponent(out m_impulseSource);
    }

    public void PlayHitStop()
    {
        HitStop.Play(timeScale: m_hitStopTimeScale, duration: m_hitStopDuration);
    }

    public void PlayScreenShake()
    {
        m_impulseSource.GenerateImpulseWithVelocity(new Vector3(
            UnityEngine.Random.Range(-m_screenShakeIntensity, m_screenShakeIntensity),
            UnityEngine.Random.Range(-m_screenShakeIntensity, m_screenShakeIntensity),
            UnityEngine.Random.Range(-m_screenShakeIntensity, m_screenShakeIntensity)));
    }
}
