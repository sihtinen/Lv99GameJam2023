using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class InhaleCollisionParticles : MonoBehaviour
{
    [SerializeField] private ParticleSettings m_particleSettingsClose = new ParticleSettings();
    [SerializeField] private ParticleSettings m_particleSettingsFar = new ParticleSettings();

    [Header("Object References")]
    [SerializeField] private PlayerInhaleComponent m_inhaleComponent = null;

    private ParticleSystem m_particleSystem = null;

    private void Awake()
    {
        TryGetComponent(out m_particleSystem);
        transform.SetParent(null);
    }

    private void Update()
    {
        if (m_inhaleComponent.IsInhaling == false || m_inhaleComponent.InhaleCollisionFound == false)
        {
            if (m_particleSystem.isPlaying)
                m_particleSystem.Stop(withChildren: true, stopBehavior: ParticleSystemStopBehavior.StopEmitting);

            return;
        }

        float _distance = Vector3.Distance(m_inhaleComponent.transform.position, m_inhaleComponent.InhaleCollisionHit.point);
        float _min = m_particleSettingsClose.DistanceToPlayer;
        float _max = m_particleSettingsFar.DistanceToPlayer;

        float _distanceNormalized = Mathf.Clamp01((_distance - _min) / (_max - _min));

        var _particleSettings = lerp(m_particleSettingsClose, m_particleSettingsFar, _distanceNormalized);

        var _main = m_particleSystem.main;
        var _startLifetimeCurve = _main.startLifetime;
        _startLifetimeCurve.constantMin = _particleSettings.MinLifeTime;
        _startLifetimeCurve.constantMax = _particleSettings.MaxLifeTime;
        _main.startLifetime = _startLifetimeCurve;

        transform.position = m_inhaleComponent.InhaleCollisionHit.point;
        transform.forward = -m_inhaleComponent.InhaleCollisionHit.normal;

        if (m_particleSystem.isPlaying == false)
            m_particleSystem.Play();
    }

    private ParticleSettings lerp(ParticleSettings a, ParticleSettings b, float time)
    {
        return new ParticleSettings
        {
            MinLifeTime = Mathf.Lerp(a.MinLifeTime, b.MinLifeTime, time),
            MaxLifeTime = Mathf.Lerp(a.MinLifeTime, b.MinLifeTime, time),
        };
    }

    [System.Serializable]
    public struct ParticleSettings
    {
        public float DistanceToPlayer;
        public float MinLifeTime;
        public float MaxLifeTime;
    }
}
