using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

public class PlayerTriggerVolumeCamera : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private CinemachineVirtualCamera m_cam = null;

    private void Awake()
    {
        m_cam.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
            m_cam.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
            m_cam.enabled = false;
    }
}
