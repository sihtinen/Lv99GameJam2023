using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AmbienceAudioSource : AudioPlayer
{
    private void Start()
    {
        m_audioSource.Play();
    }
}