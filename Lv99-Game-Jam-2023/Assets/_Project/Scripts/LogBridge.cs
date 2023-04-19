using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;

public class LogBridge : PuzzleBehaviour, IMeleeTarget, IMinecartObstacle, IInhaleTarget
{
    [SerializeField] private bool m_isCollapsedByDefault = false;
    [SerializeField] private int m_initialCollapsedHealth = 3;
    [SerializeField] private float m_animationSpeed = 1.0f;
    [SerializeField] private Collider m_collider = null;
    [SerializeField] private PlayableDirector m_director = null;
    [SerializeField] private LogBridgeAudioPlayer m_audioPlayer = null;

    private int m_currentHealth;

    private void Start()
    {
        ResetPuzzleState();
    }

    public void OnHit(Vector3 playerPosition)
    {
        if (m_isCollapsedByDefault)
            return;

        if (m_collider != null)
            m_collider.enabled = false;

        if (m_director.playableGraph.IsValid() == false)
            m_director.RebuildGraph();

        m_director.playableGraph.GetRootPlayable(0).SetSpeed(m_animationSpeed);
        m_director.Play();

        m_audioPlayer.gameObject.SetActiveOptimized(true);
        m_audioPlayer.Play();
    }

    public override void ResetPuzzleState()
    {
        if (m_collider != null)
            m_collider.enabled = m_isCollapsedByDefault == false;

        m_currentHealth = m_initialCollapsedHealth;

        gameObject.SetActiveOptimized(true);

        m_director.Stop();
        m_director.time = m_isCollapsedByDefault ? m_director.duration : 0f;
        m_director.Evaluate();
    }

    bool IMinecartObstacle.IsActive()
    {
        return gameObject.activeInHierarchy && m_director.time >= m_director.duration;
    }

    IMinecartObstacle.CollisionResults IMinecartObstacle.OnCollision(Minecart minecart)
    {
        var _results = new IMinecartObstacle.CollisionResults();
        _results.IsPathBlocked = true;

        m_currentHealth--;

        if (m_currentHealth <= 0)
        {
            gameObject.SetActiveOptimized(false);
            _results.IsPathBlocked = false;
        }

        return _results;
    }

    public void OnInhaleHit(Vector3 playerPosition)
    {
        if (m_director.time > 0)
            return;

        Vector3 _toPlayer = playerPosition - transform.position;
        float _forwardDot = Vector3.Dot(_toPlayer.normalized, transform.forward);

        if (_forwardDot < 0.5f)
            return;

        OnHit(playerPosition);

        m_audioPlayer.gameObject.SetActiveOptimized(true);
        m_audioPlayer.Play();
    }
}
