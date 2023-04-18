using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AudioSourceManagerBase<T> : SingletonBehaviour<T> where T : MonoBehaviour
{
    [Header("Audio Source Manager Base Settings")]
    [SerializeField] private int m_audioSourcePoolSize = 16;
    [SerializeField] private AudioPlayer m_audioPlayerPrefab = null;

    protected Stack<AudioPlayer> m_audioPlayerPool = new();
    protected List<AudioPlayer> m_activeAudioPlayers = new();

    protected override void Awake()
    {
        base.Awake();

        if (this != Instance)
            return;

        for (int i = 0; i < m_audioSourcePoolSize; i++)
        {
            var _audioSourceObj = Instantiate(m_audioPlayerPrefab.gameObject, parent: null);
            DontDestroyOnLoad(_audioSourceObj);
            _audioSourceObj.SetActiveOptimized(false);

            _audioSourceObj.TryGetComponent(out AudioPlayer _audioPlayer);
            m_audioPlayerPool.Push(_audioPlayer);
        }
    }

    protected virtual void LateUpdate()
    {
        for (int i = m_activeAudioPlayers.Count; i-- > 0;)
        {
            var _source = m_activeAudioPlayers[i];

            if (_source.IsPlaying() == false)
            {
                _source.gameObject.SetActiveOptimized(false);
                m_activeAudioPlayers.RemoveAt(i);
                m_audioPlayerPool.Push(_source);
            }
        }
    }

    protected AudioPlayer tryGetNewAudioPlayer()
    {
        if (m_audioPlayerPool.Count == 0)
        {
            Debug.Log($"{GetType().Name}.tryGetNewAudioSource(): audio source pool is empty - returning null");
            return null;
        }

        var _audioSource = m_audioPlayerPool.Pop();
        m_activeAudioPlayers.Add(_audioSource);
        return _audioSource;
    }
}
