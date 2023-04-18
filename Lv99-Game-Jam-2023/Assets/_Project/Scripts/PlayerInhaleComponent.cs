using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInhaleComponent : SingletonBehaviour<PlayerInhaleComponent>
{
    [Header("Inhale Settings")]
    [SerializeField, Min(0f)] private float m_collisionStartTime = 0.6f;
    public float InhaleDistance = 30f;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_inhaleActionRef = null;

    [NonSerialized] public bool IsInhaling = false;
    [NonSerialized] public float InhaleTime = 0f;

    private PlayerCharacter m_player = null;
    private PlayerMoveComponent m_moveComponent = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_player);
        TryGetComponent(out m_moveComponent);

        if (m_inhaleActionRef != null)
        {
            m_inhaleActionRef.action.started += this.onInhaleInput;
            m_inhaleActionRef.action.canceled += this.onInhaleInput;
            m_inhaleActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_inhaleActionRef != null)
        {
            m_inhaleActionRef.action.started -= this.onInhaleInput;
            m_inhaleActionRef.action.canceled -= this.onInhaleInput;
            m_inhaleActionRef.action.Disable();
        }
    }

    private void onInhaleInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (m_player.AllowInhale == false)
                return;

            IsInhaling = true;
            InhaleTime = 0f;

            m_player.UseAbility(AbilityTypes.Inhale);
        }

        if (context.canceled)
        {
            IsInhaling = false;
        }
    }

    private void Update()
    {
        if (IsInhaling == false)
            return;

        if (m_moveComponent.IsGrounded == false)
        {
            IsInhaling = false;
            return;
        }

        InhaleTime += Time.deltaTime;

        if (InhaleTime < m_collisionStartTime)
            return;

        Vector3 _raycastOrigin = transform.position + new Vector3(0f, 1f, 0f);

        int _hitCount = Physics.SphereCastNonAlloc(
            origin: _raycastOrigin,
            radius: 1.5f,
            direction: transform.forward,
            results: PhysicsUtility.CachedRaycastHits,
            maxDistance: InhaleDistance,
            layerMask: PhysicsUtility.GroundLayerMask,
            queryTriggerInteraction: QueryTriggerInteraction.Collide);

        if (_hitCount == 0)
            return;

        for (int i = 0; i < _hitCount; i++)
        {
            var _hit = PhysicsUtility.CachedRaycastHits[i];

            if (_hit.point == Vector3.zero)
                continue;

            if (Physics.Linecast(_raycastOrigin, _hit.point + 0.1f * _hit.normal))
                continue;

            Debug.DrawLine(_raycastOrigin, _hit.point, Color.cyan);

            if (_hit.transform.root.TryGetComponent(out IInhaleTarget _inhaleTarget))
                _inhaleTarget.OnInhaleHit(playerPosition: transform.position);
        }
    }
}