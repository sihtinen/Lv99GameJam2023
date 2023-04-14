using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerMoveComponent : SingletonBehaviour<PlayerMoveComponent>
{
    [Header("Move Settings")]
    [SerializeField, Min(0f)] private float m_moveSpeed = 1f;
    [SerializeField, Min(0f)] private float m_moveAcceleration_Ground = 1f;
    [SerializeField, Min(0f)] private float m_moveDeceleration_Ground = 1f;
    [SerializeField, Min(0f)] private float m_moveAcceleration_Air = 1f;
    [SerializeField, Min(0f)] private float m_moveDeceleration_Air = 1f;
    [SerializeField, Range(0f, 1f)] private float m_moveInputThreshold = 0.1f;
    [Space]
    [SerializeField] private float m_fallAcceleration = 8f;
    [SerializeField] private float m_maxFallVelocity = 5f;
    [Space]
    [SerializeField, Min(0f)] private float m_jumpVelocity = 2f;
    [SerializeField] private float m_jumpCooldownTime = 1f;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_moveActionRef = null;
    [SerializeField] private InputActionReference m_jumpActionRef = null;

    [Header("Unity Events")]
    public UnityEvent OnJumped = new UnityEvent();
    public UnityEvent OnLanded = new UnityEvent();

    private bool m_wasGroundedPreviousFrame = false;
    private float m_currentVerticalVelocity = 0f;
    private float m_jumpStartTime = 0f;
    private Vector3 m_currentHorizontalVelocity = Vector3.zero;
    private CharacterController m_characterController = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_characterController);

        if (m_moveActionRef != null)
            m_moveActionRef.action.Enable();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.performed += this.onJumpInput;
            m_jumpActionRef.action.Enable();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed == false)
            return;

        if (m_characterController.isGrounded == false)
            return;

        float _timePassed = Time.time - m_jumpStartTime;

        if (_timePassed < m_jumpCooldownTime)
            return;

        m_currentVerticalVelocity = m_jumpVelocity;
        m_jumpStartTime = Time.time;
        OnJumped?.Invoke();
    }

    private void OnDestroy()
    {
        if (m_moveActionRef != null)
            m_moveActionRef.action.Disable();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.performed -= this.onJumpInput;
            m_jumpActionRef.action.Disable();
        }
    }

    private void Update()
    {
        if (m_moveActionRef == null)
            return;

        if (m_wasGroundedPreviousFrame == false && m_characterController.isGrounded)
            OnLanded?.Invoke();

        var _moveInput = m_moveActionRef.action.ReadValue<Vector2>();

        float _targetAcceleration = m_characterController.isGrounded ? m_moveAcceleration_Ground : m_moveAcceleration_Air;
        float _targetDeceleration = m_characterController.isGrounded ? m_moveAcceleration_Air : m_moveDeceleration_Air;
        float _finalAcceleration = _moveInput.magnitude > m_moveInputThreshold ? _targetAcceleration : _targetDeceleration;

        var _mainCameraForward = MainCameraComponent.Instance.HorizontalForwardDirection;
        var _mainCameraRight = MainCameraComponent.Instance.HorizontalRightDirection;

        var _targetVelocity = m_moveSpeed * (_moveInput.x * _mainCameraRight + _moveInput.y * _mainCameraForward);

        m_currentHorizontalVelocity = Vector3.MoveTowards(m_currentHorizontalVelocity, _targetVelocity, Time.deltaTime * _finalAcceleration);

        bool _isJumping = Time.time - m_jumpStartTime < m_jumpCooldownTime;

        if (_isJumping == false && (m_wasGroundedPreviousFrame && m_characterController.isGrounded == false))
            m_currentVerticalVelocity = 0f;
        else
            m_currentVerticalVelocity = Mathf.MoveTowards(m_currentVerticalVelocity, -m_maxFallVelocity, Time.deltaTime * m_fallAcceleration);

        Vector3 _moveVelocity = m_currentHorizontalVelocity + new Vector3(0f, m_currentVerticalVelocity, 0f);

        m_wasGroundedPreviousFrame = m_characterController.isGrounded;
        m_characterController.Move(Time.deltaTime * _moveVelocity);
    }

    public bool IsGrounded => m_characterController.isGrounded;
}