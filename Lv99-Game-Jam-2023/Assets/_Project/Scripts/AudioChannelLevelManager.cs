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

    public enum AudioChannel
    {
        None = 0,
        Environment = 1,
        Ambient = 2,
        Meditation = 3,
    }
}
