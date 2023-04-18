using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] protected float m_volume = 1.0f;
    [SerializeField] protected AudioChannelLevelManager.AudioChannel m_channel = AudioChannelLevelManager.AudioChannel.None;

    [Header("Object References")]
    [SerializeField] protected AudioSource m_audioSource = null;

    protected virtual void OnDestroy()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
            m_audioSource.Stop();
    }

    protected virtual void Update()
    {
        if (m_audioSource.isPlaying == false)
            return;

        var _channelManager = AudioChannelLevelManager.Instance;

        if (_channelManager == null)
            return;

        m_audioSource.volume = m_volume * _channelManager.GetChannelVolume(m_channel);
    }
}