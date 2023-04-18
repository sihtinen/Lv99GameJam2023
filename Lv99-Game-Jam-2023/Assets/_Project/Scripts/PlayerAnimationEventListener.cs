using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimationEventListener : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private PlayerMoveComponent m_moveComponent = null;

    private void OnFootstep()
    {
        if (m_moveComponent.IsGrounded == false)
            return;

        var _footstepAudioManager = FootstepAudioManager.Instance;

        if (_footstepAudioManager == null)
            return;

        _footstepAudioManager.PlayAudio(transform.position);
    }
}
