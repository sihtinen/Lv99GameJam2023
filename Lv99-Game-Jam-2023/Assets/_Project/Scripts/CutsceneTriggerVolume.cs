using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class CutsceneTriggerVolume : MonoBehaviour
{
    public UnityEvent OnPlayerEnteredVolume = new();

    private PlayerCharacter m_player = null;

    public bool IsPlayerOnVolume()
    {
        if (m_player == null)
            return false;

        return m_player.MoveComponent.IsGrounded;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
        {
            m_player = _player;
            OnPlayerEnteredVolume.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player))
            m_player = null;
    }
}
