using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Cinemachine;


[DefaultExecutionOrder(-1)]
public class Minecart : PuzzleBehaviour, IMeleeTarget, IMinecartObstacle
{
    public MinecartState InitialState = new MinecartState();

    [Header("Movement Settings")]
    public float MaxSpeed = 2.0f;
    public float AccelerationDuration = 3.0f;
    public AnimationCurve AccelerationCurve = new AnimationCurve();
    [Space]
    public float MaxFallVelocity = 8f;
    public float FallVelocityAcceleration = 3f;

    [Header("Object References")]
    [SerializeField] private CinemachineImpulseSource m_collisionImpulseSource = null;
    [SerializeField] private MinecartCrashAudioPlayer m_crashAudioPlayer = null;

    [Header("Unity Events")]
    public UnityEvent OnResetPuzzleState = new();

    [NonSerialized] public bool IsOnRailroad = false;
    [NonSerialized] public bool IsMoving = false;
    [NonSerialized] public float RailPathPosition;
    [NonSerialized] public float AccelerationTime = 0f;
    [NonSerialized] public float VerticalVelocity = 0f;
    [NonSerialized] public Railroad CurrentRailroad = null;
    [NonSerialized] public CinemachineSmoothPath CurrentPath = null;
    [NonSerialized] public MinecartSpawner SourceSpawner = null;

    [NonSerialized] IMinecartObstacle InFrontObstacle = null;

    private bool m_isPathReversed = false;
    private BoxCollider m_boxCollider = null;
    private List<Railroad> m_collidedRailroads = new List<Railroad>();

    private void Awake()
    {
        TryGetComponent(out m_boxCollider);

        InitialState.Position = transform.position;
        InitialState.Rotation = transform.rotation;

        ResetPuzzleState();
    }

    public override void ResetPuzzleState()
    {
        transform.position = InitialState.Position;
        transform.rotation = InitialState.Rotation;

        InFrontObstacle = null;

        IsMoving = InitialState.IsMoving;
        AccelerationTime = InitialState.AccelerationCurvePos * AccelerationDuration;
        CurrentRailroad = InitialState.FirstRailroadPiece;
        IsOnRailroad = CurrentRailroad != null;

        if (CurrentRailroad != null)
        {
            CurrentPath = CurrentRailroad.GetOpenPath();
            prepareRailroadPath();
        }

        gameObject.SetActiveOptimized(true);

        OnResetPuzzleState?.Invoke();
    }

    private void prepareRailroadPath()
    {
        RailPathPosition = CurrentPath.FromPathNativeUnits(CurrentPath.FindClosestPoint(transform.position, 0, -1, 32), CinemachinePathBase.PositionUnits.Distance);

        var _defaultOrientation = CurrentPath.EvaluateOrientationAtUnit(RailPathPosition, CinemachinePathBase.PositionUnits.Distance);
        var _defaultForwardDir = _defaultOrientation * Vector3.forward;

        m_isPathReversed = Vector3.Dot(_defaultForwardDir, transform.forward) < 0;
    }

    private void Update()
    {
        if (IsMoving == false)
            return;

        if (IsOnRailroad)
            updateRailPathMovement();

        if (IsOnRailroad == false)
            updateNonRailroadMovement();

        if (m_collidedRailroads.Count > 0)
            resolveRailroadCollisions();
    }

    private void updateRailPathMovement()
    {
        float _deltaTime = GameTime.DeltaTime(TimeChannel.Environment);

        if (AccelerationTime < AccelerationDuration)
            AccelerationTime += _deltaTime;
        else
            AccelerationTime = AccelerationDuration;

        VerticalVelocity = 0f;

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;

        bool _pathUpdated = false;
        float _distanceOverflow = 0f;

        if (m_isPathReversed)
        {
            RailPathPosition -= _speed * _deltaTime;

            if (RailPathPosition <= 0f)
            {
                _pathUpdated = true;
                _distanceOverflow = Mathf.Abs(RailPathPosition);
                getNextRailPath();
            }
        }
        else
        {
            RailPathPosition += _speed * _deltaTime;

            if (RailPathPosition >= CurrentPath.PathLength)
            {
                _pathUpdated = true;
                _distanceOverflow = RailPathPosition - CurrentPath.PathLength;
                getNextRailPath();
            }
        }

        if (IsOnRailroad == false)
            return;

        if (_pathUpdated)
        {
            if (m_isPathReversed)
                RailPathPosition -= _distanceOverflow;
            else
                RailPathPosition += _distanceOverflow;
        }

        Vector3 _targetPos = CurrentPath.EvaluatePositionAtUnit(RailPathPosition, CinemachinePathBase.PositionUnits.Distance);
        Quaternion _targetRot = CurrentPath.EvaluateOrientationAtUnit(RailPathPosition, CinemachinePathBase.PositionUnits.Distance);

        if (m_isPathReversed)
            _targetRot *= Quaternion.Euler(0f, 180f, 0f);

        Vector3 _finalPos = Vector3.Lerp(transform.position, _targetPos, _deltaTime * 40f);
        Quaternion _finalRot = Quaternion.Lerp(transform.rotation, _targetRot, _deltaTime * 20f);

        transform.SetPositionAndRotation(_finalPos, _finalRot);

        //if (m_isPathReversed)
        //    transform.forward = -transform.forward;
    }

    private void updateNonRailroadMovement()
    {
        float _deltaTime = GameTime.DeltaTime(TimeChannel.Environment);

        bool _groundBelow = checkBelow(checkDistance: Mathf.Max(VerticalVelocity * _deltaTime, 0.1f));

        if (_groundBelow)
        {
            VerticalVelocity = 0f;
        }
        else
        {
            if (AccelerationTime > 0f)
                AccelerationTime -= _deltaTime * AccelerationDuration;
            else
                AccelerationTime = 0f;

            if (VerticalVelocity < MaxFallVelocity)
                VerticalVelocity += _deltaTime * FallVelocityAcceleration;
            else
                VerticalVelocity = MaxFallVelocity;
        }

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;

        transform.position += _speed * _deltaTime * transform.forward;
        transform.position -= VerticalVelocity * _deltaTime * Vector3.up;

        if (transform.position.y < -40)
        {
            InFrontObstacle = null;

            if (SourceSpawner != null)
                SourceSpawner.ReturnToPool(this);

            gameObject.SetActiveOptimized(false);
        }
    }

    private void getNextRailPath()
    {
        var _closestSocket = CurrentRailroad.GetClosestSocket(transform.position);

        for (int i = 0; i < _closestSocket.ConnectedSockets.Count; i++)
        {
            var _socket = _closestSocket.ConnectedSockets[i];

            if (_socket.Path.enabled)
            {
                CurrentRailroad = _socket.RailroadComponent;
                CurrentPath = _socket.Path;
                prepareRailroadPath();
                return;
            }
        }

        IsOnRailroad = false;
    }

    private bool checkBelow(float checkDistance)
    {
        int _hitCount = Physics.BoxCastNonAlloc(
            center: transform.position + m_boxCollider.size.y * 0.5f * Vector3.up,
            halfExtents: 0.5f * m_boxCollider.size,
            direction: Vector3.down,
            results: PhysicsUtility.CachedRaycastHits,
            orientation: transform.rotation,
            maxDistance: checkDistance,
            layerMask: PhysicsUtility.GroundLayerMask,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        if (_hitCount == 0)
            return false;

        for (int i = 0; i < _hitCount; i++)
        {
            var _hit = PhysicsUtility.CachedRaycastHits[i];

            if (_hit.transform.root != transform.root)
                return true;
        }

        return false;
    }

    public void OnCollidedWithRailroad(Railroad railroad)
    {
        m_collidedRailroads.Add(railroad);
    }

    private void resolveRailroadCollisions()
    {
        float _closestDistance = float.MaxValue;
        Railroad _closestRailroad = null;

        for (int i = 0; i < m_collidedRailroads.Count; i++)
        {
            var _railroad = m_collidedRailroads[i];

            if (_railroad == CurrentRailroad)
                continue;

            float _dist = Vector3.Distance(transform.position, _railroad.transform.position);

            if (_dist < _closestDistance)
            {
                _closestDistance = _dist;
                _closestRailroad = _railroad;
            }
        }

        m_collidedRailroads.Clear();

        if (_closestRailroad == null)
            return;

        AccelerationTime *= 0.3f;

        IsOnRailroad = true;
        CurrentRailroad = _closestRailroad;
        CurrentPath = CurrentRailroad.GetOpenPath();
        prepareRailroadPath();
    }

    public void OnHit(Vector3 playerPosition)
    {
        if (IsOnRailroad)
            return;

        AccelerationTime = AccelerationDuration * 0.33f;
        IsMoving = true;
    }

    public Vector3 GetVelocity()
    {
        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;

        Vector3 _result = Vector3.zero;
        _result += _speed * transform.forward;
        _result -= VerticalVelocity * Vector3.up;
        return _result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out IMinecartObstacle _obstacle) == false)
            return;

        if (_obstacle.IsActive() == false)
            return;

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;
        m_collisionImpulseSource.GenerateImpulseWithVelocity(_speed * transform.forward);

        AccelerationTime = 0;

        if (IsOnRailroad && InFrontObstacle == null)
        {
            Vector3 _toObstacle = (_obstacle.Position - transform.position).normalized;
            bool _isFront = Vector3.Dot(_toObstacle, transform.forward) >= 0f;

            if (_isFront)
            {
                IsMoving = false;
                InFrontObstacle = _obstacle;
                InFrontObstacle.Collision(this);

                if (m_crashAudioPlayer != null)
                {
                    m_crashAudioPlayer.gameObject.SetActiveOptimized(true);
                    m_crashAudioPlayer.Play();
                }
            }
        }
    }

    bool IMinecartObstacle.IsActive()
    {
        return true;
    }

    Vector3 IMinecartObstacle.Position => transform.position;

    void IMinecartObstacle.Collision(Minecart minecart)
    {
        if (InFrontObstacle == null)
        {
            IsMoving = true;
            AccelerationTime = 0f;
        }
        else
        {
            InFrontObstacle.Collision(this);

            if (InFrontObstacle.IsStationary() == false)
                InFrontObstacle = null;
        }
    }

    bool IMinecartObstacle.IsStationary()
    {
        return IsMoving == false;
    }

    [System.Serializable]
    public class MinecartState
    {
        public bool IsMoving = false;
        [Range(0f, 1f)] public float AccelerationCurvePos = 0f;
        public Railroad FirstRailroadPiece = null;
        [NonSerialized] public Vector3 Position;
        [NonSerialized] public Quaternion Rotation;
    }
}