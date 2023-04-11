using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(100)]
public class HitStop : MonoBehaviour
{
    private static HitStop m_instance = null;
    private List<HitStopEvent> m_activeHitStopEvents = new();
    private Stack<HitStopEvent> m_hitStopEventPool = new();

    private void Update()
    {
        HitStopEvent _currentEvent = null;

        for (int i = m_activeHitStopEvents.Count; i --> 0;)
        {
            var _event = m_activeHitStopEvents[i];
            _event.RemainingDuration -= Time.unscaledDeltaTime;

            if (_event.RemainingDuration <= 0f)
            {
                m_activeHitStopEvents.RemoveAt(i);
                continue;
            }

            if (_currentEvent == null)
            {
                _currentEvent = _event;
                continue;
            }

            if (_event.Priority > _currentEvent.Priority)
            {
                _currentEvent = _event;
                continue;
            }

            if (_event.Priority == _currentEvent.Priority && _event.TimeScale < _currentEvent.TimeScale)
            {
                _currentEvent = _event;
                continue;
            }
        }

        Time.timeScale = _currentEvent != null ? _currentEvent.TimeScale : 1.0f;
    }

    public static void Play(float timeScale, float duration, int priority = 0)
    {
        var _instance = getInstance();
        var _newEvent = _instance.getNewHitStopEvent();

        _newEvent.TimeScale = Mathf.Max(timeScale, 0.001f);
        _newEvent.RemainingDuration = duration;
        _newEvent.Priority = priority;

        _instance.m_activeHitStopEvents.Add(_newEvent);
    }

    private static HitStop getInstance()
    {
        if (m_instance == null)
        {
            var _newInstanceObj = new GameObject("HitStop Instance");
            _newInstanceObj.transform.SetParent(null);
            DontDestroyOnLoad(_newInstanceObj);
            m_instance = _newInstanceObj.AddComponent<HitStop>();
        }

        return m_instance;
    }

    private HitStopEvent getNewHitStopEvent()
    {
        if (m_hitStopEventPool.Count > 0)
            return m_hitStopEventPool.Pop();

        return new HitStopEvent();
    }

    private class HitStopEvent
    {
        public float TimeScale;
        public float RemainingDuration;
        public int Priority;
    }
}