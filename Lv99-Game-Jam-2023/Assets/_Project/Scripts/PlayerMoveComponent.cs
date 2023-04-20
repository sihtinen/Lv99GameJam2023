using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Cinemachine;

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
    [Space]
    [SerializeField] private float m_rotateSpeed = 3f;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_moveActionRef = null;
    [SerializeField] private InputActionReference m_jumpActionRef = null;
    [SerializeField] private CinemachineImpulseSource m_onLandingImpulseSource = null;

    [Header("Unity Events")]
    public UnityEvent OnJumped = new UnityEvent();
    public UnityEvent OnLanded = new UnityEvent();

    [NonSerialized] public Vector3 InputWorldDirection = Vector3.zero;
    [NonSerialized] public float CurrentVerticalVelocity = 0f;

    private bool m_wasGroundedPreviousFrame = false;
    private float m_jumpStartTime = 0f;
    private Vector3 m_currentHorizontalVelocity = Vector3.zero;
    private CharacterController m_characterController = null;
    private PlayerCharacter m_playerCharacter = null;
    private List<Collider> m_myColliders = new List<Collider>();

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_characterController);
        TryGetComponent(out m_playerCharacter);

        m_myColliders.Add(m_characterController);

        if (TryGetComponent(out CapsuleCollider _capsuleColl))
            m_myColliders.Add(_capsuleColl);

        if (m_moveActionRef != null)
            m_moveActionRef.action.Enable();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.started += this.onJumpInput;
            m_jumpActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_moveActionRef != null)
            m_moveActionRef.action.Disable();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.started -= this.onJumpInput;
            m_jumpActionRef.action.Disable();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.started == false)
            return;

        if (m_playerCharacter.AllowJump == false)
            return;

        if (m_characterController.isGrounded == false)
            return;

        float _timePassed = Time.time - m_jumpStartTime;

        if (_timePassed < m_jumpCooldownTime)
            return;

        CurrentVerticalVelocity = m_jumpVelocity;
        m_jumpStartTime = Time.time;
        m_playerCharacter.UseAbility(AbilityTypes.Jump);

        OnJumped?.Invoke();
    }

    private void Update()
    {
        if (m_moveActionRef == null)
            return;

        if (m_wasGroundedPreviousFrame == false && m_characterController.isGrounded)
        {
            OnLanded?.Invoke();

            if (CurrentVerticalVelocity < -0.5f && m_onLandingImpulseSource != null)
            {
                var _impulseVelocity = 0.01f * new Vector3(
                    0f,
                    CurrentVerticalVelocity,
                    0f);

                m_onLandingImpulseSource.GenerateImpulseWithVelocity(_impulseVelocity);
            }
        }

        var _moveInput = m_moveActionRef.action.ReadValue<Vector2>();

        var _mainCameraForward = MainCameraComponent.Instance.HorizontalForwardDirection;
        var _mainCameraRight = MainCameraComponent.Instance.HorizontalRightDirection;

        InputWorldDirection = _moveInput.x * _mainCameraRight + _moveInput.y * _mainCameraForward;

        if (m_playerCharacter.AllowMovement == false)
            _moveInput = Vector2.zero;

        float _deltaTime = GameTime.DeltaTime(TimeChannel.Player);

        float _targetAcceleration = m_characterController.isGrounded ? m_moveAcceleration_Ground : m_moveAcceleration_Air;
        float _targetDeceleration = m_characterController.isGrounded ? m_moveDeceleration_Ground : m_moveDeceleration_Air;
        float _finalAcceleration = _moveInput.magnitude > m_moveInputThreshold ? _targetAcceleration : _targetDeceleration;

        var _targetVelocity = m_moveSpeed * (_moveInput.x * _mainCameraRight + _moveInput.y * _mainCameraForward);

        m_currentHorizontalVelocity = Vector3.MoveTowards(m_currentHorizontalVelocity, _targetVelocity, _deltaTime * _finalAcceleration);

        bool _isJumping = Time.time - m_jumpStartTime < m_jumpCooldownTime;

        if (_isJumping == false && (m_wasGroundedPreviousFrame && m_characterController.isGrounded == false))
            CurrentVerticalVelocity = 0f;
        else
            CurrentVerticalVelocity = Mathf.MoveTowards(CurrentVerticalVelocity, -m_maxFallVelocity, _deltaTime * m_fallAcceleration);

        Vector3 _moveVelocity = m_currentHorizontalVelocity + new Vector3(0f, CurrentVerticalVelocity, 0f);

        if (m_characterController.isGrounded && m_standingOnMinecart != null)
        {
            var _minecartVelocity = m_standingOnMinecart.GetVelocity();
            _moveVelocity += GameTimeManager.Instance.EnvironmentTimeScale * _minecartVelocity;
        }

        m_wasGroundedPreviousFrame = m_characterController.isGrounded;
        m_characterController.Move(_deltaTime * _moveVelocity);

        if (m_characterController.isGrounded == false)
        {
            m_previousGroundCheckObjectID = -1;
            m_standingOnMinecart = null;
        }

        updateCharacterForwardDirection(_moveInput);
    }

    private Vector2 m_previousRotationInput = Vector2.zero;

    private void updateCharacterForwardDirection(Vector2 moveInput)
    {
        if (m_playerCharacter.AllowMovement == false)
            return;

        if (moveInput.magnitude >= 0.01f)
            m_previousRotationInput = moveInput;

        if (m_previousRotationInput == Vector2.zero)
            return;

        var _mainCamera = MainCameraComponent.Instance;

        if (_mainCamera == null)
            return;

        Vector3 _newForward = (
            m_previousRotationInput.x * _mainCamera.HorizontalRightDirection + 
            m_previousRotationInput.y * _mainCamera.HorizontalForwardDirection).normalized;

        var _targetRotation = Quaternion.LookRotation(_newForward, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, _targetRotation, GameTime.DeltaTime(TimeChannel.Player) * m_rotateSpeed);
    }

    private int m_previousGroundCheckObjectID = -1;
    private Minecart m_standingOnMinecart = null;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null)
        {
            m_previousGroundCheckObjectID = -1;
            m_standingOnMinecart = null;
            return;
        }

        int _id = hit.transform.root.GetInstanceID();

        if (_id == m_previousGroundCheckObjectID)
            return;

        m_previousGroundCheckObjectID = _id;

        hit.transform.root.TryGetComponent(out m_standingOnMinecart);
    }

    public bool IsGrounded => m_characterController.isGrounded;
    public bool IsRunning
    {
        get
        {
            if (IsGrounded == false)
                return false;

            Vector3 _vel = m_characterController.velocity;

            if (m_standingOnMinecart != null)
                _vel -= m_standingOnMinecart.GetVelocity();

            return _vel.sqrMagnitude > 0.7f;
        }
    }

    public void MoveTowards(Transform moveTarget)
    {
        float _deltaTime = GameTime.DeltaTime(TimeChannel.Player);

        Vector3 _newTargetPos = Vector3.Lerp(transform.position, moveTarget.position, _deltaTime * 5f);

        m_characterController.Move(_newTargetPos - transform.position);

        transform.rotation = Quaternion.Lerp(transform.rotation, moveTarget.rotation, _deltaTime * 20f);
    }

    public void SetPositionAndRotation(Transform target)
    {
        m_characterController.enabled = false;
        transform.SetPositionAndRotation(target.position, target.rotation);
        m_characterController.enabled = true;
    }

    public void ResetVerticalVelocity()
    {
        CurrentVerticalVelocity = 0f;
    }
}