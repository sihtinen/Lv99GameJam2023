using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MinecartSpawner : PuzzleBehaviour
{
    [SerializeField] private float m_spawnFrequencySeconds = 3.5f;
    [SerializeField] private float m_initialSpawnWaitTime = 3.5f;
    [SerializeField, Range(0f, 1f)] private float m_startSpeedNormalized = 1f;
    [SerializeField] private float m_spawnerFreeSpaceRequired = 5f;

    [Header("Object References")]
    [SerializeField] private Minecart m_minecartPrefab = null;
    [SerializeField] private Railroad m_initialRailroad = null;

    private float m_spawnWaitTime = 0f;

    private List<Minecart> m_activeMinecarts = new();
    private List<Minecart> m_minecartPool = new();

    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        { 
            var _minecartObj = Instantiate(m_minecartPrefab.gameObject, parent: null);
            _minecartObj.name = $"{gameObject.name}_Minecart-{i.ToStringMinimalAlloc()}";
            _minecartObj.SetActiveOptimized(false);
            _minecartObj.TryGetComponent(out Minecart _minecart);
            _minecart.SourceSpawner = this;
            m_minecartPool.Add(_minecart);
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
            _minecart.ResetPuzzleState();
            _minecart.gameObject.SetActiveOptimized(false);

            if (m_minecartPool.Contains(_minecart) == false)
                m_minecartPool.Add(_minecart);
        }

        m_activeMinecarts.Clear();
    }

    private void Update()
    {
        m_spawnWaitTime -= GameTime.DeltaTime(TimeChannel.Environment);

        if (m_spawnWaitTime > 0f)
            return;

        m_spawnWaitTime += m_spawnFrequencySeconds;

        for (int i = 0; i < m_activeMinecarts.Count; i++)
        {
            var _activeCart = m_activeMinecarts[i];

            if (Vector3.Distance(transform.position, _activeCart.transform.position) < m_spawnerFreeSpaceRequired)
                return;
        }

        var _newCart = getNewMinecart();
        m_activeMinecarts.Add(_newCart);

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
        {
            var _result = m_minecartPool[m_minecartPool.Count - 1];
            m_minecartPool.Remove(_result);
            return _result;
        }

        var _minecartObj = Instantiate(m_minecartPrefab.gameObject, parent: null);
        _minecartObj.SetActiveOptimized(false);
        _minecartObj.TryGetComponent(out Minecart _minecart);
        _minecart.SourceSpawner = this;
        return _minecart;
    }

    public void ReturnToPool(Minecart minecart)
    {
        if (m_activeMinecarts.Contains(minecart))
            m_activeMinecarts.Remove(minecart);

        if (m_minecartPool.Contains(minecart) == false)
            m_minecartPool.Add(minecart);
    }
}
