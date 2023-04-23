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
    [SerializeField] private InhaleAudioPlayer m_audioPlayer_Loop = null;
    [SerializeField] private InhaleAudioPlayer m_audioPlayer_End = null;

    [NonSerialized] public bool IsInhaling = false;
    [NonSerialized] public float InhaleTime = 0f;
    [NonSerialized] public bool InhaleCollisionFound = false;
    [NonSerialized] public RaycastHit InhaleCollisionHit;

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

            m_audioPlayer_Loop.gameObject.SetActiveOptimized(true);
            m_audioPlayer_Loop.volume = 0f;
            m_audioPlayer_Loop.FadeIn(targetVolume: 0.7f, speed: 0.3f);
            m_audioPlayer_Loop.Play();
        }

        if (IsInhaling && context.canceled)
            endInhale();
    }

    private void Update()
    {
        InhaleCollisionFound = false;

        if (IsInhaling == false)
            return;

        if (m_moveComponent.IsGrounded == false)
        {
            endInhale();
            return;
        }

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
        {
            endInhale();
            return;
        }

        InhaleTime += GameTime.DeltaTime(TimeChannel.Player);

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

        bool _inhaleTargetHit = false;

        for (int i = 0; i < _hitCount; i++)
        {
            var _hit = PhysicsUtility.CachedRaycastHits[i];

            if (_hit.point == Vector3.zero)
                continue;

            if (Physics.Linecast(_raycastOrigin, _hit.point + 0.1f * _hit.normal))
                continue;

            if (InhaleCollisionFound == false)
            {
                if (Vector3.Dot(_hit.normal, Vector3.up) < 0.5f)
                {
                    InhaleCollisionFound = true;
                    InhaleCollisionHit = _hit;
                }
            }

            if (_hit.transform.root.TryGetComponent(out IInhaleTarget _inhaleTarget))
            {
                _inhaleTarget.OnInhaleHit(playerPosition: transform.position);

                if (_inhaleTargetHit == false)
                {
                    _inhaleTargetHit = true;
                    InhaleCollisionFound = true;
                    InhaleCollisionHit = _hit;
                }
            }
        }
    }

    private void endInhale()
    {
        IsInhaling = false;

        m_audioPlayer_Loop.FadeOut(speed: 2f);
        m_audioPlayer_End.volume = m_audioPlayer_Loop.volume;
        m_audioPlayer_End.gameObject.SetActiveOptimized(true);
        m_audioPlayer_End.Play();
    }
}