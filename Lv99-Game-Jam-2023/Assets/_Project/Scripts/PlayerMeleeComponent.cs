using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
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
            m_playerCharacter.UseAbility(AbilityTypes.Melee);
            m_meleeCoroutine = StartCoroutine(coroutine_meleeAttack());
        }
    }

    private IEnumerator coroutine_meleeAttack()
    {
        IsMeleeAttacking = true;
        MeleeTime = 0f;

        bool _hitDealt = false;
        float _timer = 0f;

        while (_timer < m_meleeDuration)
        {
            yield return null;
            _timer += Time.deltaTime;

            MeleeTime = _timer / m_meleeDuration;

            if (_hitDealt == false && MeleeTime >= m_meleeHitTimeNormalized)
            {
                calculateHit();
                _hitDealt = true;
            }
        }

        IsMeleeAttacking = false;
        m_meleeCoroutine = null;
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
                scale: m_hitBoxHalfExtents,
                c: Color.red,
                time: 5f);
        }

        if (_hitCount == 0)
            return;

        for (int i = 0; i < _hitCount; i++)
        {
            var _coll = PhysicsUtility.CachedRaycastHits[i];

            if (_coll.transform.TryGetComponent(out IMeleeTarget _meleeTarget))
                _meleeTarget.OnHit(playerPosition: transform.position);
        }

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
