using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

[DefaultExecutionOrder(1)]
public class Minecart : PuzzleBehaviour, IMeleeTarget
{
    public MinecartState InitialState = new MinecartState();

    [Header("Movement Settings")]
    public float MaxSpeed = 2.0f;
    public float AccelerationDuration = 3.0f;
    public AnimationCurve AccelerationCurve = new AnimationCurve();
    [Space]
    public float MaxFallVelocity = 8f;
    public float FallVelocityAcceleration = 3f;

    [NonSerialized] public bool IsOnRailroad;
    [NonSerialized] public bool IsMoving;
    [NonSerialized] public float RailPathPosition;
    [NonSerialized] public float AccelerationTime = 0f;
    [NonSerialized] public float VerticalVelocity = 0f;
    [NonSerialized] public Railroad CurrentRailroad = null;
    [NonSerialized] public CinemachineSmoothPath CurrentPath = null;

    private BoxCollider m_boxCollider = null;

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

        IsMoving = InitialState.IsMoving;
        AccelerationTime = InitialState.AccelerationCurvePos * AccelerationDuration;
        CurrentRailroad = InitialState.FirstRailroadPiece;
        IsOnRailroad = CurrentRailroad != null;

        if (CurrentRailroad != null)
        {
            CurrentPath = CurrentRailroad.GetOpenPath();
            RailPathPosition = CurrentPath.FromPathNativeUnits(CurrentPath.FindClosestPoint(transform.position, 0, -1, 32), CinemachinePathBase.PositionUnits.Distance);
        }
    }

    private void Update()
    {
        if (IsMoving == false)
            return;

        if (IsOnRailroad)
            updateRailPathMovement();

        if (IsOnRailroad == false)
            updateNonRailroadMovement();
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
        RailPathPosition += _speed * Time.deltaTime;

        if (RailPathPosition >= CurrentPath.PathLength)
        {
            RailPathPosition -= CurrentPath.PathLength;
            getNextRailPath();
        }

        if (IsOnRailroad == false)
            return;

        Vector3 _pos = CurrentPath.EvaluatePositionAtUnit(RailPathPosition, CinemachinePathBase.PositionUnits.Distance);
        Quaternion _rot = CurrentPath.EvaluateOrientationAtUnit(RailPathPosition, CinemachinePathBase.PositionUnits.Distance);

        transform.SetPositionAndRotation(_pos, _rot);
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
        if (railroad == CurrentRailroad)
            return;

        CurrentRailroad = railroad;
        IsOnRailroad = true;
        CurrentPath = CurrentRailroad.GetOpenPath();
        RailPathPosition = CurrentPath.FromPathNativeUnits(CurrentPath.FindClosestPoint(transform.position, 0, -1, 32), CinemachinePathBase.PositionUnits.Distance);
    }

    public void OnHit(Vector3 playerPosition)
    {
        if (IsOnRailroad)
            return;

        AccelerationTime = AccelerationDuration * 0.33f;
        IsMoving = true;
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