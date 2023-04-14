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

    [Header("Object References")]
    [SerializeField] private InputActionReference m_meleeActionRef = null;

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
        bool _hitDealt = false;
        float _timer = 0f;

        while (_timer < m_meleeDuration)
        {
            yield return null;
            _timer += Time.deltaTime;

            if (_hitDealt == false)
            {
                float _timePos = _timer / m_meleeDuration;

                if (_timePos >= m_meleeHitTimeNormalized)
                {
                    calculateHit();
                    _hitDealt = true;
                }
            }
        }

        m_meleeCoroutine = null;
    }

    private void calculateHit()
    {
        int _hitCount = Physics.BoxCastNonAlloc(
            center: transform.position,
            halfExtents: m_hitBoxHalfExtents,
            direction: transform.forward,
            results: PhysicsUtility.CachedRaycastHits,
            orientation: transform.rotation,
            maxDistance: m_hitCheckDistance,
            layerMask: m_hitLayers,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        if (_hitCount == 0)
            return;

        for (int i = 0; i < _hitCount; i++)
        {
            var _coll = PhysicsUtility.CachedRaycastHits[i];

            if (_coll.transform.TryGetComponent(out IMeleeTarget _meleeTarget))
                _meleeTarget.OnHit(playerPosition: transform.position);
        }
    }

    public bool IsMeleeAttacking() => m_meleeCoroutine != null;
}
