using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(100)]
public class AudioListenerPositionUpdater : MonoBehaviour
{
    [SerializeField] private float m_normalizedDistanceToPlayer = 0.7f;

    private void LateUpdate()
    {
        var _player = PlayerCharacter.Instance;
        var _mainCam = MainCameraComponent.Instance;

        if (_player == null || _mainCam == null)
            return;

        transform.SetPositionAndRotation(
            Vector3.Lerp(_mainCam.transform.position, _player.transform.position, m_normalizedDistanceToPlayer),
            _mainCam.transform.rotation);
    }
}
