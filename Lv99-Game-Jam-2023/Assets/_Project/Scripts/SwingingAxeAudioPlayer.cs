using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class SwingingAxeAudioPlayer : AudioPlayer
{
    [Header("Axe Audio Settings")]
    [SerializeField] private float m_minPitch = 0.8f;
    [SerializeField] private float m_maxPitch = 0.85f;

    [SerializeField] private List<AudioClip> m_clips = new();

    public void SelectClipAndPlay()
    {
        m_audioSource.clip = m_clips.GetRandomElement();
        m_audioSource.pitch = Random.Range(m_minPitch, m_maxPitch);
        gameObject.SetActiveOptimized(true);
        Play();
    }
}