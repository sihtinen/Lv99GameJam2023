using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

public class InhaleCameraNoise : MonoBehaviour
{
    [SerializeField] private float m_maxGain = 1.0f;

    private CinemachineVirtualCamera m_vcam = null;
    private CinemachineBasicMultiChannelPerlin m_noise = null;

    private void Awake()
    {
        TryGetComponent(out m_vcam);
        m_noise = m_vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        m_noise.m_AmplitudeGain = 0f;
    }

    private void Update()
    {
        float _targetGain = 0f;

        var _inhaleComponent = PlayerInhaleComponent.Instance;

        if (_inhaleComponent != null && _inhaleComponent.IsInhaling)
            _targetGain = m_maxGain;

        m_noise.m_AmplitudeGain = Mathf.MoveTowards(m_noise.m_AmplitudeGain, _targetGain, GameTime.DeltaTime(TimeChannel.Player));
    }
}
