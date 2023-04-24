using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMeleeComponent : SingletonBehaviour<PlayerMeleeComponent>
{
    [Header("Melee Settings")]
    [SerializeField] private float m_meleeDuration = 1.0f;
    [SerializeField, Range(0f, 1f)] private float m_meleeHitTimeNormalized = 0.4f;
    [SerializeField] private Vector3 m_hitBoxHalfExtents = new Vector3(1f, 1f, 1f);
    [SerializeField] private float m_hitCheckDistance = 1.0f;
    [SerializeField] private LayerMask m_hitLayers;
    [Space]
    [SerializeField] private float m_meleeHitStopDuration = 0.3f;
    [SerializeField] private float m_meleeHitStopTimeScale = 0.3f;
    [Space]
    [SerializeField] private float m_meleeHitImpulseMinStrength = 0f;
    [SerializeField] private float m_meleeHitImpulseMaxStrength = 0f;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_meleeActionRef = null;
    [SerializeField] private CinemachineImpulseSource m_meleeHitImpulseSource = null;
    [SerializeField] private ParticleSystem m_metalHitParticles = null;

    [Header("Events")]
    public UnityEvent OnMeleeSwingBegin = new();
    public UnityEvent OnMeleeSwingHit = new();

    [NonSerialized] public bool IsMeleeAttacking = false;
    [NonSerialized] public float MeleeTime = 0f;

    private PlayerCharacter m_playerCharacter = null;
    private PlayerMoveComponent m_moveComponent = null;
    private Coroutine m_meleeCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_playerCharacter);
        TryGetComponent(out m_moveComponent);

        m_metalHitParticles.transform.SetParent(null);

        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.started += this.onMeleeInput;
            m_meleeActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.started -= this.onMeleeInput;
            m_meleeActionRef.action.Disable();
        }
    }

    private void onMeleeInput(InputAction.CallbackContext context)
    {
        if (m_playerCharacter.AllowMelee == false)
            return;

        if (m_moveComponent.IsGrounded == false)
            return;

        if (context.started)
        {
            m_playerCharacter.UseAbility(AbilityTypes.Pickaxe);
            m_meleeCoroutine = StartCoroutine(coroutine_meleeAttack());
        }
    }

    private static Collider[] m_tempColliders = new Collider[32];

    private IEnumerator coroutine_meleeAttack()
    {
        IsMeleeAttacking = true;
        MeleeTime = 0f;

        OnMeleeSwingBegin?.Invoke();

        bool _hitDealt = false;
        float _timer = 0f;

        var _hitTarget = getMeleeTargetTransform();

        while (_timer < m_meleeDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            MeleeTime = _timer / m_meleeDuration;

            if (_hitDealt == false && MeleeTime >= m_meleeHitTimeNormalized)
            {
                calculateHit();

                _hitDealt = true;
            }
            else if (MeleeTime < m_meleeHitTimeNormalized && _hitTarget != null)
            {
                Vector3 _toTarget = _hitTarget.RootTransform.position - transform.position;

                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    Quaternion.LookRotation(_toTarget.normalized, Vector3.up),
                    GameTime.DeltaTime(TimeChannel.Player) * 400);

                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }

        IsMeleeAttacking = false;
        m_meleeCoroutine = null;
    }

    private List<IMeleeTarget> m_possibleTransforms = new List<IMeleeTarget>();

    private IMeleeTarget getMeleeTargetTransform()
    {
        int _hitCount = Physics.OverlapSphereNonAlloc(
            position: transform.position + new Vector3(0f, 1f, 0f),
            radius: 4.0f,
            results: m_tempColliders,
            layerMask: Physics.AllLayers);

        for (int i = 0; i < _hitCount; i++)
        {
            var _coll = m_tempColliders[i];

            if (_coll.TryGetComponent(out IMeleeTarget _meleeTarget))
            {
                if (m_possibleTransforms.Contains(_meleeTarget) == false)
                    m_possibleTransforms.Add(_meleeTarget);
            }
        }

        float _bestScore = float.MinValue;
        IMeleeTarget _bestTarget = null;

        for (int i = 0; i < m_possibleTransforms.Count; i++)
        {
            var _targetTransform = m_possibleTransforms[i].RootTransform;

            Vector3 _toTransform = _targetTransform.position - transform.position;
            float _distance = _toTransform.magnitude;
            float _dot = Vector3.Dot(_toTransform.normalized, transform.forward);

            float _score = -_distance + _dot * 2f;

            if (_score > _bestScore || _bestTarget == null)
            {
                _bestScore = _score;
                _bestTarget = m_possibleTransforms[i];
            }
        }

        m_possibleTransforms.Clear();

        return _bestTarget;
    }

    private Vector3 getClosestHit(Transform target)
    {
        Vector3 _hitOrigin = transform.position + new Vector3(0f, 1f, 0f);

        if (target.TryGetComponent(out Collider coll))
            return coll.ClosestPoint(_hitOrigin);

        return target.transform.position + new Vector3(0f, 1f, 0f);
    }

    private void calculateHit()
    {
        Vector3 _hitOrigin = transform.position + new Vector3(0f, 1f, 0f);

        int _hitCount = Physics.BoxCastNonAlloc(
            center: _hitOrigin,
            halfExtents: m_hitBoxHalfExtents,
            direction: transform.forward,
            results: PhysicsUtility.CachedRaycastHits,
            orientation: transform.rotation,
            maxDistance: m_hitCheckDistance,
            layerMask: m_hitLayers,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        if (Application.isEditor)
        {
            drawBox(
                pos: _hitOrigin + m_hitCheckDistance * transform.forward,
                rot: transform.rotation,
                scale: 2f * m_hitBoxHalfExtents,
                c: Color.red,
                time: 5f);
        }

        if (_hitCount == 0)
            return;

        bool _hitValidTarget = false;

        for (int i = 0; i < _hitCount; i++)
        {
            var _coll = PhysicsUtility.CachedRaycastHits[i];

            if (_coll.transform.TryGetComponent(out IMeleeTarget _meleeTarget))
            {
                _meleeTarget.OnHit(playerPosition: transform.position);
                _hitValidTarget = true;

                if (_meleeTarget != null)
                {
                    switch (_meleeTarget.HitParticlesType)
                    {
                        case MeleeHitParticlesType.Metal:

                            Vector3 _particlesPos = getClosestHit(_meleeTarget.RootTransform);
                            m_metalHitParticles.transform.position = _particlesPos;
                            m_metalHitParticles.transform.forward = (transform.position - _particlesPos).normalized;
                            m_metalHitParticles.Play();

                            break;
                    }
                }
            }
        }

        if (_hitValidTarget == false)
            return;

        OnMeleeSwingHit?.Invoke();

        if (m_meleeHitImpulseSource != null)
        {
            var _impulseVelocity = m_meleeHitImpulseMaxStrength * UnityEngine.Random.insideUnitSphere;
            if (_impulseVelocity.magnitude < m_meleeHitImpulseMinStrength)
                _impulseVelocity = m_meleeHitImpulseMinStrength * _impulseVelocity.normalized;

            m_meleeHitImpulseSource.GenerateImpulseWithVelocity(_impulseVelocity);
        }

        HitStop.Play(timeScale: m_meleeHitStopTimeScale, duration: m_meleeHitStopDuration);
    }

    private void drawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c, float time)
    {
        // create matrix
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));

        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));

        Debug.DrawLine(point1, point2, c, time);
        Debug.DrawLine(point2, point3, c, time);
        Debug.DrawLine(point3, point4, c, time);
        Debug.DrawLine(point4, point1, c, time);

        Debug.DrawLine(point5, point6, c, time);
        Debug.DrawLine(point6, point7, c, time);
        Debug.DrawLine(point7, point8, c, time);
        Debug.DrawLine(point8, point5, c, time);

        Debug.DrawLine(point1, point5, c, time);
        Debug.DrawLine(point2, point6, c, time);
        Debug.DrawLine(point3, point7, c, time);
        Debug.DrawLine(point4, point8, c, time);

        // optional axis display
        //Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
        //Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
        //Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
    }
}
