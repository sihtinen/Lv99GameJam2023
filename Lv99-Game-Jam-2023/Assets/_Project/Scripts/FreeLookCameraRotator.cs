using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FreeLookCameraRotator : MonoBehaviour
{
    [SerializeField] private float m_speed = 1.0f;

    private Cinemachine.CinemachineFreeLook m_freeLookCam = null;

    private void Awake()
    {
        TryGetComponent(out m_freeLookCam);
    }

    private void Update()
    {
        m_freeLookCam.m_XAxis.Value += Time.deltaTime * m_speed;
    }
}
