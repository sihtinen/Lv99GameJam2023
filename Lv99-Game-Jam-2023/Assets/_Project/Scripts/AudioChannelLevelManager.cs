using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AudioChannelLevelManager : SingletonBehaviour<AudioChannelLevelManager>
{
    [NonSerialized] public float EnvironmentVolume = 1.0f;
    [NonSerialized] public float AmbientVolume = 1.0f;
    [NonSerialized] public float MeditationVolume = 1.0f;

    public float GetChannelVolume(AudioChannel channel)
    {
        switch (channel)
        {
            default:
            case AudioChannel.None:
                return 1f;

            case AudioChannel.Environment:
                return EnvironmentVolume;

            case AudioChannel.Ambient:
                return AmbientVolume;

            case AudioChannel.Meditation:
                return MeditationVolume;
        }
    }

    private void LateUpdate()
    {
        float _globalMaxVolume = 1.0f;

        var _transitionScreen = TransitionScreen.Instance;
        if (_transitionScreen != null && _transitionScreen.IsTransitionActive)
            _globalMaxVolume = 1.0f - _transitionScreen.TransitionFillAmount;

        float _targetEnvironmentVolume = 1.0f * _globalMaxVolume;
        float _targetAmbientVolume = 1.0f * _globalMaxVolume;
        float _targetMeditationVolume = _globalMaxVolume;

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
        {
            _targetEnvironmentVolume = 0f;
            _targetAmbientVolume = 0f;
        }

        var _gameTime = GameTimeManager.Instance;
        if (_gameTime != null && _gameTime.IsTimestopActive)
        {
            _targetAmbientVolume = 0f;
            _targetEnvironmentVolume = 0f;
        }

        EnvironmentVolume = Mathf.MoveTowards(EnvironmentVolume, _targetEnvironmentVolume, EnvironmentVolume < _targetEnvironmentVolume ? GameTime.DeltaTime(TimeChannel.Player) : 2f * GameTime.DeltaTime(TimeChannel.Player));
        AmbientVolume = Mathf.MoveTowards(AmbientVolume, _targetAmbientVolume, GameTime.DeltaTime(TimeChannel.Player));
        MeditationVolume = Mathf.MoveTowards(MeditationVolume, _targetMeditationVolume, GameTime.DeltaTime(TimeChannel.Player));
    }

    public enum AudioChannel
    {
        None = 0,
        Environment = 1,
        Ambient = 2,
        Meditation = 3,
    }
}
