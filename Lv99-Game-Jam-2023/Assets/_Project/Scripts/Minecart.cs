using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

    [NonSerialized] public bool IsOnRailroad = false;
    [NonSerialized] public bool IsMoving = false;
    [NonSerialized] public float RailPathPosition;
    [NonSerialized] public float AccelerationTime = 0f;
    [NonSerialized] public float VerticalVelocity = 0f;
    [NonSerialized] public Railroad CurrentRailroad = null;
    [NonSerialized] public CinemachineSmoothPath CurrentPath = null;
    [NonSerialized] public MinecartSpawner SourceSpawner = null;
    [NonSerialized] IMinecartObstacle CurrentObstacle = null;
     
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

        CurrentObstacle = null;

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
        if (AccelerationTime < AccelerationDuration)
            AccelerationTime += Time.deltaTime;
        else
            AccelerationTime = AccelerationDuration;

        VerticalVelocity = 0f;

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;

        bool _pathUpdated = false;
        float _distanceOverflow = 0f;

        if (m_isPathReversed)
        {
            RailPathPosition -= _speed * Time.deltaTime;

            if (RailPathPosition <= 0f)
            {
                _pathUpdated = true;
                _distanceOverflow = Mathf.Abs(RailPathPosition);
                getNextRailPath();
            }
        }
        else
        {
            RailPathPosition += _speed * Time.deltaTime;

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

        Vector3 _finalPos = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 40f);
        Quaternion _finalRot = Quaternion.Lerp(transform.rotation, _targetRot, Time.deltaTime * 20f);

        transform.SetPositionAndRotation(_finalPos, _finalRot);

        //if (m_isPathReversed)
        //    transform.forward = -transform.forward;
    }

    private void updateNonRailroadMovement()
    {
        if (checkBelow(checkDistance: Mathf.Max(VerticalVelocity * Time.deltaTime, 0.1f)))
        {
            VerticalVelocity = 0f;
        }
        else
        {
            if (VerticalVelocity < MaxFallVelocity)
                VerticalVelocity += Time.deltaTime * FallVelocityAcceleration;
            else
                VerticalVelocity = MaxFallVelocity;
        }

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;

        transform.position += _speed * Time.deltaTime * transform.forward;
        transform.position -= VerticalVelocity * Time.deltaTime * Vector3.up;

        if (transform.position.y < -40)
        {
            gameObject.SetActiveOptimized(false);

            if (SourceSpawner != null)
                SourceSpawner.ReturnToPool(this);
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

    public void OnCollidedWithObstacle(IMinecartObstacle obstacle)
    {
        if (CurrentObstacle != null)
            return;

        float _accelerationCurveVal = AccelerationCurve.Evaluate(AccelerationTime / AccelerationDuration);
        float _speed = MaxSpeed * _accelerationCurveVal;
        m_collisionImpulseSource.GenerateImpulseWithVelocity(_speed * transform.forward);

        IsMoving = false;
        AccelerationTime = 0;
        VerticalVelocity = 0;
        CurrentObstacle = obstacle;
        CurrentObstacle.OnCollision(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out IMinecartObstacle _obstacle))
        {
            if (_obstacle.IsActive())
                OnCollidedWithObstacle(_obstacle);
        }
    }

    bool IMinecartObstacle.IsActive()
    {
        return IsMoving == false;
    }

    IMinecartObstacle.CollisionResults IMinecartObstacle.OnCollision(Minecart minecart)
    {
        var _results = new IMinecartObstacle.CollisionResults();
        _results.IsPathBlocked = true;

        if (CurrentObstacle != null)
        {
            var _frontCollision = CurrentObstacle.OnCollision(this);

            if (_frontCollision.IsPathBlocked == false)
                CurrentObstacle = null;
        }
        else
        {
            _results.IsPathBlocked = false;

            IsMoving = true;
            AccelerationTime = 0f;
        }

        return _results;
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