using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;

public class LogBridge : PuzzleBehaviour, IMeleeTarget, IMinecartObstacle, IInhaleTarget
{
    [SerializeField] private bool m_isCollapsedByDefault = false;
    [SerializeField] private int m_initialCollapsedHealth = 3;
    [SerializeField] private float m_animationSpeed = 1.0f;

    [Header("Object References")]
    [SerializeField] private Collider m_collider = null;
    [SerializeField] private PlayableDirector m_director = null;
    [SerializeField] private LogBridgeAudioPlayer m_audioPlayer = null;
    [SerializeField] private List<LogBridgeBone> m_bonePieces = new();

    private int m_currentHealth;
    private FractureDirection m_fractureDirection;

    Vector3 IMinecartObstacle.Position => transform.position;

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

        m_fractureDirection = FractureDirection.None;

        for (int i = 0; i < m_bonePieces.Count; i++)
            m_bonePieces[i].OnHealthUpdated(isCollapsed: true, m_currentHealth, m_fractureDirection);

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

        for (int i = 0; i < m_bonePieces.Count; i++)
            m_bonePieces[i].OnHealthUpdated(m_isCollapsedByDefault, m_currentHealth, m_fractureDirection);

        gameObject.SetActiveOptimized(true);

        m_director.Stop();
        m_director.time = m_isCollapsedByDefault ? m_director.duration : 0f;
        m_director.Evaluate();

        for (int i = 0; i < m_bonePieces.Count; i++)
            m_bonePieces[i].ResetToInitialState();
    }

    bool IMinecartObstacle.IsActive()
    {
        return gameObject.activeInHierarchy && m_director.time >= m_director.duration;
    }

    bool IMinecartObstacle.IsStationary()
    {
        return m_currentHealth > 0;
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

    public void Collision(Minecart minecart)
    {
        m_currentHealth--;

        if (m_fractureDirection == FractureDirection.None)
        {
            Vector3 _toMinecart = (minecart.transform.position - transform.position).normalized;

            if (Vector3.Dot(transform.right, _toMinecart) >= 0f)
                m_fractureDirection = FractureDirection.Right;
            else
                m_fractureDirection = FractureDirection.Left;
        }

        for (int i = 0; i < m_bonePieces.Count; i++)
            m_bonePieces[i].OnHealthUpdated(isCollapsed: true, m_currentHealth, m_fractureDirection);
    }

    public enum FractureDirection
    {
        None = 0,
        Right = 1,
        Left = 2,
    }
}
