using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAudioManager : AudioSourceManagerBase<PlayerAudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_takeDamageClips = new();
    [SerializeField] private List<AudioClip> m_pickaxeSwingClips = new();
    [SerializeField] private List<AudioClip> m_pickaxeHitClips = new();
    [SerializeField] private List<AudioClip> m_jumpStartClips = new();
    [SerializeField] private List<AudioClip> m_jumpLandingClips = new();

    public void PlayTakeDamageSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_takeDamageClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
        _audioSource.volume = 1f;
    }

    public void PlayPickaxeSwingSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_pickaxeSwingClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.PlayDelayed(delayTime: 0.15f);
        _audioSource.volume = 1f;
    }

    public void PlayPickaxeHitSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_pickaxeHitClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
        _audioSource.volume = 1f;
    }

    public void PlayJumpStartSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_jumpStartClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
        _audioSource.volume = 0.5f;
    }

    public void PlayJumpLandingSound()
    {
        var _audioSource = tryGetNewAudioPlayer();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_jumpLandingClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
        _audioSource.volume = 1f;
    }
}
