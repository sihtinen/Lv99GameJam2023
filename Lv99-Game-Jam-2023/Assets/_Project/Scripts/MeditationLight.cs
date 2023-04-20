using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationLight : MonoBehaviour
{
    [SerializeField, Min(0f)] private float m_maxIntensity = 1.0f;
    [SerializeField, Min(0f)] private float m_minIntensity = 0f;
    [SerializeField] private float m_changeSpeed = 0.8f;
    private Light m_light = null;

    private void Awake()
    {
        TryGetComponent(out m_light);
        m_light.enabled = false;
        m_light.intensity = m_minIntensity;
    }

    private void Update()
    {
        float _targetIntensity = m_minIntensity;

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            _targetIntensity = m_maxIntensity;

        m_light.intensity = Mathf.MoveTowards(m_light.intensity, _targetIntensity, GameTime.DeltaTime(TimeChannel.Player) * m_changeSpeed);
        m_light.enabled = m_light.intensity > 0f;
    }
}
