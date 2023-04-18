using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerVoiceAudioManager : AudioSourceManagerBase<PlayerVoiceAudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_takeDamageClips = new();

    public void PlayTakeDamageSound()
    {
        var _audioSource = tryGetNewAudioSource();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_takeDamageClips.GetRandomElement();
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
    }
}
