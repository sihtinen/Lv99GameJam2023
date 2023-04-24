using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BreakableObject : PuzzleBehaviour, IMeleeTarget
{
    [SerializeField] private MeleeHitParticlesType m_hitParticlesType = MeleeHitParticlesType.None;
    [SerializeField] private GameObject m_visualObject = null;
    [SerializeField] private Collider m_collider = null;
    [SerializeField] private BreakableObjectAudioPlayer m_audioPlayer = null;
    [SerializeField] private ParticleSystem m_breakParticles = null;

    public Transform RootTransform => transform;

    public MeleeHitParticlesType HitParticlesType => m_hitParticlesType;

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

        if (m_breakParticles != null)
            m_breakParticles.Play(withChildren: true);
    }

    public override void ResetPuzzleState()
    {
        if (m_visualObject != null)
            m_visualObject.gameObject.SetActiveOptimized(true);

        if (m_collider != null)
            m_collider.enabled = true;
    }
}