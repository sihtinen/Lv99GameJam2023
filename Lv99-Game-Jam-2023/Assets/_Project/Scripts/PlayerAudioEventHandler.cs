using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudioEventHandler : MonoBehaviour
{
    private PlayerCharacter m_player = null;

    private void Awake()
    {
        TryGetComponent(out m_player);
        m_player.OnTakeDamage.AddListener(this.onTakeDamage);
    }

    private void onTakeDamage()
    {
        var _voiceAudioManager = PlayerVoiceAudioManager.Instance;
        if (_voiceAudioManager != null)
            _voiceAudioManager.PlayTakeDamageSound();
    }
}