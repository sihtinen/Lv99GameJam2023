using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BreakableObject : PuzzleBehaviour, IMeleeTarget
{
    [SerializeField] private GameObject m_visualObject = null;
    [SerializeField] private Collider m_collider = null;
    [SerializeField] private BreakableObjectAudioPlayer m_audioPlayer = null;

    public void OnHit(Vector3 playerPosition)
    {
        if (m_visualObject != null)
            m_visualObject.gameObject.SetActiveOptimized(false);

        if (m_collider != null)
            m_collider.enabled = false;

        if (m_audioPlayer != null)
        {
            m_audioPlayer.gameObject.SetActiveOptimized(true);
            m_audioPlayer.Play();
        }
    }

    public override void ResetPuzzleState()
    {
        if (m_visualObject != null)
            m_visualObject.gameObject.SetActiveOptimized(true);

        if (m_collider != null)
            m_collider.enabled = true;
    }
}