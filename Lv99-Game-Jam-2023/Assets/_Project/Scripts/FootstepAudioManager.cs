using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FootstepAudioManager : AudioSourceManagerBase<FootstepAudioManager>
{
    [Header("Audio Clips")]
    [SerializeField] private List<AudioClip> m_stoneFootstepClips = new();

    public void PlayAudio(Vector3 position)
    {
        var _audioPlayer = tryGetNewAudioPlayer();

        if (_audioPlayer == null)
            return;

        _audioPlayer.clip = m_stoneFootstepClips.GetRandomElement();
        _audioPlayer.transform.position = position;
        _audioPlayer.gameObject.SetActiveOptimized(true);
        _audioPlayer.Play();
    }
}