using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class StaticEnvironmentAudioPlayer : AudioPlayer
{
    protected override void Update()
    {
        var _audioListener = AudioListenerPositionUpdater.Instance;

        if (_audioListener != null)
        {
            bool _isAudible = Vector3.Distance(_audioListener.transform.position, transform.position) <= m_audioSource.maxDistance;

            if (_isAudible && m_audioSource.isPlaying == false)
            {
                m_audioSource.time = Random.Range(0f, m_audioSource.clip.length);
                m_audioSource.Play();
            }
            else if (_isAudible == false && m_audioSource.isPlaying)
                m_audioSource.Stop();
        }

        base.Update();
    }
}