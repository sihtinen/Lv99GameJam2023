using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MinecartSpawner : PuzzleBehaviour
{
    [SerializeField] private float m_spawnFrequencySeconds = 3.5f;
    [SerializeField] private float m_initialSpawnWaitTime = 3.5f;
    [SerializeField, Range(0f, 1f)] private float m_startSpeedNormalized = 1f;

    [Header("Object References")]
    [SerializeField] private Minecart m_minecartPrefab = null;
    [SerializeField] private Railroad m_initialRailroad = null;

    private float m_spawnWaitTime = 0f;

    private List<Minecart> m_activeMinecarts = new();
    private Stack<Minecart> m_minecartPool = new();

    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        { 
            var _minecartObj = Instantiate(m_minecartPrefab.gameObject, parent: null);
            _minecartObj.SetActiveOptimized(false);
            _minecartObj.TryGetComponent(out Minecart _minecart);
            _minecart.SourceSpawner = this;
            m_minecartPool.Push(_minecart);
        }
    }

    private void Start()
    {
        ResetPuzzleState();
    }

    public override void ResetPuzzleState()
    {
        m_spawnWaitTime = m_initialSpawnWaitTime;

        for (int i = 0; i < m_activeMinecarts.Count; i++)
        {
            var _minecart = m_activeMinecarts[i];
            _minecart.gameObject.SetActiveOptimized(false);
            m_minecartPool.Push(_minecart);
        }

        m_activeMinecarts.Clear();
    }

    private void Update()
    {
        m_spawnWaitTime -= Time.deltaTime;

        if (m_spawnWaitTime > 0f)
            return;

        m_spawnWaitTime += m_spawnFrequencySeconds;

        var _newCart = getNewMinecart();
        _newCart.transform.SetPositionAndRotation(transform.position, transform.rotation);
        _newCart.AccelerationTime = m_startSpeedNormalized * _newCart.AccelerationDuration;
        _newCart.VerticalVelocity = 0f;
        _newCart.IsMoving = true;
        _newCart.IsOnRailroad = false;
        _newCart.CurrentPath = null;
        _newCart.CurrentRailroad = null;

        if (m_initialRailroad != null)
            _newCart.OnCollidedWithRailroad(m_initialRailroad);

        _newCart.gameObject.SetActiveOptimized(true);
    }

    private Minecart getNewMinecart()
    {
        if (m_minecartPool.Count > 0)
            return m_minecartPool.Pop();

        var _minecartObj = Instantiate(m_minecartPrefab.gameObject, parent: null);
        _minecartObj.SetActiveOptimized(false);
        _minecartObj.TryGetComponent(out Minecart _minecart);
        _minecart.SourceSpawner = this;
        return _minecart;
    }

    public void ReturnToPool(Minecart minecart)
    {
        m_activeMinecarts.Remove(minecart);
        m_minecartPool.Push(minecart);
    }
}
