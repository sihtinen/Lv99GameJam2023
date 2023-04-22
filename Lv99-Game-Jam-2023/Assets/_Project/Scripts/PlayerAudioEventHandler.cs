using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudioEventHandler : MonoBehaviour
{
    private PlayerCharacter m_player = null;
    private PlayerMeleeComponent m_meleeComponent = null;
    private PlayerMoveComponent m_moveComponent = null;

    private void Awake()
    {
        TryGetComponent(out m_player);
        m_player.OnTakeDamage.AddListener(this.onTakeDamage);

        TryGetComponent(out m_meleeComponent);
        m_meleeComponent.OnMeleeSwingBegin.AddListener(this.onMeleeSwingBegin);
        m_meleeComponent.OnMeleeSwingHit.AddListener(this.onMeleeSwingHit);

        TryGetComponent(out m_moveComponent);
        m_moveComponent.OnJumped.AddListener(this.onJumpStarted);
        m_moveComponent.OnLanded.AddListener(this.onJumpLanded);
    }

    private void onTakeDamage()
    {
        PlayerAudioManager.Instance?.PlayTakeDamageSound();
    }

    private void onMeleeSwingBegin()
    {
        PlayerAudioManager.Instance?.PlayPickaxeSwingSound();
    }

    private void onMeleeSwingHit()
    {
        PlayerAudioManager.Instance?.PlayPickaxeHitSound();
    }

    private void onJumpStarted()
    {
        PlayerAudioManager.Instance?.PlayJumpStartSound();
    }

    private void onJumpLanded()
    {
        PlayerAudioManager.Instance?.PlayJumpLandingSound();
    }
}