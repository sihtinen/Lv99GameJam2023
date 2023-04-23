using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AudioPlayer : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] protected float m_volume = 1.0f;
    [SerializeField] protected AudioChannelLevelManager.AudioChannel m_channel = AudioChannelLevelManager.AudioChannel.None;
    [SerializeField] protected TimeChannel m_timeChannel = TimeChannel.Environment;
    [SerializeField] private Vector2 m_pitchRange = Vector2.one;

    [Header("Object References")]
    [SerializeField] protected AudioSource m_audioSource = null;

    [NonSerialized] public bool IsDelayed = false;
    [NonSerialized] public float DelayTimeRemaining = 0f;

    protected Coroutine m_activeCoroutine = null;

    protected virtual void OnDestroy()
    {
        if (m_audioSource != null && m_audioSource.isPlaying)
            m_audioSource.Stop();
    }

    protected virtual void Update()
    {
        if (IsDelayed)
        {
            DelayTimeRemaining -= GameTime.DeltaTime(m_timeChannel);

            if (DelayTimeRemaining <= 0f)
            {
                IsDelayed = false;
                m_audioSource.pitch = UnityEngine.Random.Range(m_pitchRange.x, m_pitchRange.y);
                m_audioSource.Play();
            }

            return;
        }

        if (m_audioSource.isPlaying == false)
            return;

        var _channelManager = AudioChannelLevelManager.Instance;

        if (_channelManager == null)
            return;

        m_audioSource.volume = m_volume * _channelManager.GetChannelVolume(m_channel);
    }

    public bool IsPlaying()
    {
        if (IsDelayed)
            return true;

        return m_audioSource.isPlaying;
    }

    public AudioClip clip
    {
        get { return m_audioSource.clip; }
        set { m_audioSource.clip = value;}
    }

    public float volume
    {
        get { return m_volume; }
        set { m_volume = value; }
    }

    public void Play()
    {
        m_audioSource.pitch = UnityEngine.Random.Range(m_pitchRange.x, m_pitchRange.y);
        m_audioSource.Play();
    }

    public void PlayDelayed(float delayTime)
    {
        IsDelayed = true;
        DelayTimeRemaining = delayTime;
    }

    public void Stop()
    {
        m_audioSource.Stop();
    }

    public void FadeIn(float targetVolume, float speed)
    {
        if (m_activeCoroutine != null)
            StopCoroutine(m_activeCoroutine);

        m_activeCoroutine = StartCoroutine(coroutine_fadeIn(targetVolume, speed));
    }

    private IEnumerator coroutine_fadeIn(float targetVolume, float speed)
    {
        while (m_volume < targetVolume)
        {
            yield return null;
            m_volume += speed * GameTime.DeltaTime(m_timeChannel);
        }

        m_volume = targetVolume;
        m_activeCoroutine = null;
    }

    public void FadeOut(float speed)
    {
        if (m_activeCoroutine != null)
            StopCoroutine(m_activeCoroutine);

        m_activeCoroutine = StartCoroutine(coroutine_fadeOut(speed));
    }

    private IEnumerator coroutine_fadeOut(float speed)
    {
        while (m_volume > 0f)
        {
            yield return null;
            m_volume -= speed * GameTime.DeltaTime(m_timeChannel);
        }

        m_audioSource.Stop();
        m_activeCoroutine = null;
    }
}