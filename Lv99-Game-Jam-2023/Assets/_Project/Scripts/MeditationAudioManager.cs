using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationAudioManager : AudioSourceManagerBase<MeditationAudioManager>
{
    [Header("Audio Settings")]
    [SerializeField, Range(0f, 1f)] private float m_primaryLoopMaxVolume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float m_secondaryLoopMaxVolume = 0.7f;
    [Space]
    [SerializeField] private List<AudioClip> m_minigameSuccessClips = new();
    [SerializeField] private List<AudioClip> m_minigameFailClips = new();

    [Header("Object References")]
    [SerializeField] private AudioSource m_audioLoopPrimary = null;
    [SerializeField] private AudioSource m_audioLoopSecondary = null;

    protected override void Awake()
    {
        base.Awake();

        m_audioLoopPrimary.volume = 0f;
        m_audioLoopSecondary.volume = 0f;
    }

    private void Update()
    {
        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem == null)
            return;

        float _targetPrimaryVolume = _meditationSystem.IsPlayerMeditating ? m_primaryLoopMaxVolume : 0.0f;

        float _primaryVolumeSeekSpeed = _targetPrimaryVolume > m_audioLoopPrimary.volume ? 0.9f : 1.5f;
        m_audioLoopPrimary.volume = Mathf.Lerp(m_audioLoopPrimary.volume, _targetPrimaryVolume, Time.deltaTime * _primaryVolumeSeekSpeed);

        if (m_audioLoopPrimary.volume > 0 && m_audioLoopPrimary.isPlaying == false)
            m_audioLoopPrimary.Play();
        else if (m_audioLoopPrimary.volume <= 0f && m_audioLoopPrimary.isPlaying)
            m_audioLoopPrimary.Stop();



        float _targetSecondaryVolume = _meditationSystem.IsPlayerMeditating ? m_secondaryLoopMaxVolume : 0.0f;
        _targetSecondaryVolume *= _meditationSystem.GetMeditationProgress();

        float _secondaryVolumeSeekSpeed = _targetSecondaryVolume > m_audioLoopSecondary.volume ? 0.8f : 1.5f;
        m_audioLoopSecondary.volume = Mathf.Lerp(m_audioLoopSecondary.volume, _targetSecondaryVolume, Time.deltaTime * _secondaryVolumeSeekSpeed);

        if (m_audioLoopSecondary.volume > 0 && m_audioLoopSecondary.isPlaying == false)
            m_audioLoopSecondary.Play();
        else if (m_audioLoopSecondary.volume <= 0f && m_audioLoopSecondary.isPlaying)
            m_audioLoopSecondary.Stop();
    }

    public void OnMinigameEnded(bool success)
    {
        if (success == false)
        {
            m_audioLoopPrimary.volume = 0f;
            m_audioLoopSecondary.volume = 0f;
        }

        var _audioPlayer = tryGetNewAudioPlayer();

        if (_audioPlayer == null)
            return;

        _audioPlayer.clip = success ? m_minigameSuccessClips.GetRandomElement() : m_minigameFailClips.GetRandomElement();
        _audioPlayer.gameObject.SetActiveOptimized(true);
        _audioPlayer.Play();
    }
}
