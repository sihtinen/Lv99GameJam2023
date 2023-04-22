using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MusicAudioPlayer : AudioPlayer
{
    [SerializeField] private bool m_playOnStart = false;

    private void Start()
    {
        if (m_playOnStart)
            Play();
    }
}