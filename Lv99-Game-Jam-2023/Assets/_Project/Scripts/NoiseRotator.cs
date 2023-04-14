using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NoiseRotator : MonoBehaviour
{
    [SerializeField] private Vector2 m_noiseSpeed_X = Vector2.zero;
    [SerializeField] private Vector2 m_noiseSpeed_Y = Vector2.zero;
    [SerializeField] private Vector2 m_noiseSpeed_Z = Vector2.zero;
    [SerializeField] private Vector3 m_noiseStrength = Vector3.zero;

    private Vector3 m_defaultEuler = Vector3.zero;

    private void Awake()
    {
        m_defaultEuler = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        float _time = Time.realtimeSinceStartup;

        transform.localEulerAngles = m_defaultEuler + new Vector3(
            (-0.5f + Mathf.PerlinNoise(_time * m_noiseSpeed_X.x, _time * m_noiseSpeed_X.y)) * 2f * m_noiseStrength.x,
            (-0.5f + Mathf.PerlinNoise(_time * m_noiseSpeed_Y.x, _time * m_noiseSpeed_Y.y)) * 2f * m_noiseStrength.y,
            (-0.5f + Mathf.PerlinNoise(_time * m_noiseSpeed_Z.x, _time * m_noiseSpeed_Z.y)) * 2f * m_noiseStrength.z);
    }
}
