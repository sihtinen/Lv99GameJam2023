using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudioManager : AudioSourceManagerBase<PlayerAudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_takeDamageClips = new();
    [SerializeField] private List<AudioClip> m_pickaxeSwingClips = new();
    [SerializeField] private List<AudioClip> m_pickaxeHitClips = new();

    public void PlayTakeDamageSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_takeDamageClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
    }

    public void PlayPickaxeSwingSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_pickaxeSwingClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.PlayDelayed(delayTime: 0.15f);
    }

    public void PlayPickaxeHitSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_pickaxeHitClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
    }
}
