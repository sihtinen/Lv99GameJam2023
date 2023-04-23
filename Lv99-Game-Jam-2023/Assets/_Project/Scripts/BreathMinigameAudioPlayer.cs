using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BreathMinigameAudioPlayer : AudioPlayer
{
    [SerializeField] private List<AudioClip> m_breathInClips = new();
    [SerializeField] private List<AudioClip> m_breathOutClipts = new();

    public void PlayBreathIn()
    {
        m_audioSource.clip = m_breathInClips.GetRandomElement();
        m_audioSource.Play();
    }

    public void PlayBreathOut()
    {
        m_audioSource.clip = m_breathOutClipts.GetRandomElement();
        m_audioSource.Play();
    }
}
