using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameTimeManager : SingletonBehaviour<GameTimeManager>
{
    [NonSerialized] public float EnvironmentTimeScale = 1.0f;
    [NonSerialized] public float PlayerTimeScale = 1.0f;

    [NonSerialized] public float TimestopTimeScale = 0f;
    [NonSerialized] public float TimestopDuration = 0f;

    private void Update()
    {
        if (TimestopDuration > 0f)
        {
            TimestopDuration -= Time.deltaTime;
            EnvironmentTimeScale = TimestopTimeScale;
        }
        else
            EnvironmentTimeScale = 1.0f;
    }

    public void OnResetPuzzle()
    {
        EnvironmentTimeScale = 1.0f;
        PlayerTimeScale = 1.0f;
        TimestopDuration = 0f;
    }

    public void TriggerTimeStop(float duration, float timeScale)
    {
        TimestopDuration = duration;
        TimestopTimeScale = timeScale;
    }
}

public enum TimeChannel
{
    Environment = 0,
    Player = 1,
}

public static class GameTime
{
    public static float DeltaTime(TimeChannel channel)
    {
        var _gameTimeManager = GameTimeManager.Instance;

        if (_gameTimeManager == null)
            return Time.deltaTime;

        switch (channel)
        {
            default:
            case TimeChannel.Environment:
                return Time.deltaTime * _gameTimeManager.EnvironmentTimeScale;

            case TimeChannel.Player:
                return Time.deltaTime * _gameTimeManager.PlayerTimeScale;
        }
    }
}