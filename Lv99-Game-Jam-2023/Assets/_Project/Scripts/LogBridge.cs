using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;

public class LogBridge : PuzzleBehaviour, IMeleeTarget
{
    [SerializeField] private float m_animationSpeed = 1.0f;
    [SerializeField] private Collider m_collider = null;
    [SerializeField] private PlayableDirector m_director = null;

    public void OnHit(Vector3 playerPosition)
    {
        if (m_collider != null)
            m_collider.enabled = false;

        if (m_director.playableGraph.IsValid() == false)
            m_director.RebuildGraph();

        m_director.playableGraph.GetRootPlayable(0).SetSpeed(m_animationSpeed);
        m_director.Play();
    }

    public override void ResetPuzzleState()
    {
        if (m_collider != null)
            m_collider.enabled = true;

        m_director.Stop();
        m_director.time = 0;
        m_director.Evaluate();
    }

    private IEnumerator coroutine_playAnimation()
    {
        while (m_director.time < m_director.duration)
        {
            yield return null;
            m_director.time += Time.deltaTime * m_animationSpeed;
            m_director.Evaluate();
        }
    }
}
