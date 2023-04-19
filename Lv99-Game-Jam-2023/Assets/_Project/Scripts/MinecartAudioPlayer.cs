using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MinecartAudioPlayer : AudioPlayer
{
    [SerializeField] private Minecart m_minecart = null;

    private void Awake()
    {
        m_volume = 0f;
        m_minecart.OnResetPuzzleState.AddListener(this.onResetPuzzleState);
    }

    private void onResetPuzzleState()
    {
        m_volume = 0.0f;
    }

    protected override void Update()
    {
        float _targetVolume = m_minecart.IsOnRailroad && m_minecart.IsMoving ? 1.0f : 0.0f;
        _targetVolume *= (m_minecart.AccelerationTime / m_minecart.AccelerationDuration);

        m_volume = Mathf.Lerp(m_volume, _targetVolume, GameTime.DeltaTime(TimeChannel.Environment) * 3f);

        if (m_audioSource.isPlaying == false)
            m_audioSource.Play();

        base.Update();
    }
}
