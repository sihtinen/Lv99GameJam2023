using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationPoint : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Transform m_cameraTargetIn = null;
    [SerializeField] private Transform m_cameraTargetOut = null;
    [SerializeField] private Transform m_playerMoveTarget = null;

    public Transform CameraTargetIn => m_cameraTargetIn;
    public Transform CameraTargetOut => m_cameraTargetOut;
    public Transform PlayerMoveTarget => m_playerMoveTarget;

    private void OnTriggerEnter(Collider other)
    {
        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem == null)
            return;

        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
            _meditationSystem.OverlappingMeditationPoint = this;
    }

    private void OnTriggerExit(Collider other)
    {
        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem == null)
            return;

        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
        {
            if (_meditationSystem.OverlappingMeditationPoint == this)
                _meditationSystem.OverlappingMeditationPoint = null;
        }
    }
}
