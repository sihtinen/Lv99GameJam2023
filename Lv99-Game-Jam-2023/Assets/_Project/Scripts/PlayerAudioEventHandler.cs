using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudioEventHandler : MonoBehaviour
{
    private PlayerCharacter m_player = null;
    private PlayerMeleeComponent m_meleeComponent = null;

    private void Awake()
    {
        TryGetComponent(out m_player);
        m_player.OnTakeDamage.AddListener(this.onTakeDamage);

        TryGetComponent(out m_meleeComponent);
        m_meleeComponent.OnMeleeSwingBegin.AddListener(this.onMeleeSwingBegin);
        m_meleeComponent.OnMeleeSwingHit.AddListener(this.onMeleeSwingHit);
    }

    private void onTakeDamage()
    {
        var _voiceAudioManager = PlayerAudioManager.Instance;
        if (_voiceAudioManager != null)
            _voiceAudioManager.PlayTakeDamageSound();
    }

    private void onMeleeSwingBegin()
    {
        var _voiceAudioManager = PlayerAudioManager.Instance;
        if (_voiceAudioManager != null)
            _voiceAudioManager.PlayPickaxeSwingSound();
    }

    private void onMeleeSwingHit()
    {
        var _voiceAudioManager = PlayerAudioManager.Instance;
        if (_voiceAudioManager != null)
            _voiceAudioManager.PlayPickaxeHitSound();
    }
}