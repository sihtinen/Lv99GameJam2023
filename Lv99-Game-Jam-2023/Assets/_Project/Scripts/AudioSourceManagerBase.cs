using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AudioSourceManagerBase<T> : SingletonBehaviour<T> where T : MonoBehaviour
{
    [Header("Audio Source Manager Base Settings")]
    [SerializeField] private int m_audioSourcePoolSize = 16;
    [SerializeField] private AudioPlayer m_audioPlayerPrefab = null;

    protected Stack<AudioSource> m_audioSourcePool = new();
    protected List<AudioSource> m_activeAudioSources = new();

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

            _audioSourceObj.TryGetComponent(out AudioSource _audioSource);
            m_audioSourcePool.Push(_audioSource);
        }
    }

    protected virtual void LateUpdate()
    {
        for (int i = m_activeAudioSources.Count; i-- > 0;)
        {
            var _source = m_activeAudioSources[i];

            if (_source.isPlaying == false)
            {
                _source.gameObject.SetActiveOptimized(false);
                m_activeAudioSources.RemoveAt(i);
                m_audioSourcePool.Push(_source);
            }
        }
    }

    protected AudioSource tryGetNewAudioSource()
    {
        if (m_audioSourcePool.Count == 0)
        {
            Debug.Log($"{GetType().Name}.tryGetNewAudioSource(): audio source pool is empty - returning null");
            return null;
        }

        var _audioSource = m_audioSourcePool.Pop();
        m_activeAudioSources.Add(_audioSource);
        return _audioSource;
    }
}
