using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationPoint : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform m_cameraDockTransform = null;

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

    public Vector3 GetCameraTargetPosition()
    {
        return m_cameraDockTransform.position;
    }

    public Quaternion GetCameraTargetRotation()
    {
        return m_cameraDockTransform.rotation;
    }
}
