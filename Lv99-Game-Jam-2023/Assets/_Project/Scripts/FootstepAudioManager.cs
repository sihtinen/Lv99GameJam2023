using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FootstepAudioManager : AudioSourceManagerBase<FootstepAudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_stoneFootstepClips = new();

    public void PlayAudio(Vector3 position)
    {
        var _audioSource = tryGetNewAudioSource();

        if (_audioSource == null)
            return;

        _audioSource.clip = m_stoneFootstepClips.GetRandomElement();
        _audioSource.transform.position = position;
        _audioSource.gameObject.SetActiveOptimized(true);
        _audioSource.Play();
    }
}