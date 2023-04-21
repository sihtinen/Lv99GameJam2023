using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [Header("Animation State Names")]
    [SerializeField] private string m_idleState = "Idle";
    [SerializeField] private string m_runState = "Run";
    [SerializeField] private string m_jumpState = "Jump";
    [SerializeField] private string m_fallState = "Fall";
    [SerializeField] private string m_meleeState = "Hit";
    [SerializeField] private string m_meditateState = "Meditate";
    [SerializeField] private string m_inhaleState = "Inhale";

    [Header("Input Direction Tilt Rotate Settings")]
    [SerializeField] private float m_inputLean_Forward = 10f;
    [SerializeField] private float m_inputLean_Sideways = 25f;
    [SerializeField] private float m_inputLean_Sharpness = 3f;

    private string m_currentAnimState = string.Empty;
    private Quaternion m_inputRotation = Quaternion.identity;

    private Animator m_animator = null;
    private PlayerCharacter m_player = null;
    private PlayerMoveComponent m_moveComponent = null;
    private PlayerMeleeComponent m_meleeComponent = null;
    private PlayerInhaleComponent m_inhaleComponent = null;

    private void Start()
    {
        TryGetComponent(out m_animator);
        transform.root.TryGetComponent(out m_player);
        transform.root.TryGetComponent(out m_moveComponent);
        transform.root.TryGetComponent(out m_meleeComponent);
        transform.root.TryGetComponent(out m_inhaleComponent);
    }

    private void LateUpdate()
    {
        if (m_player == null || m_moveComponent == null || m_meleeComponent == null)
            return;

        string _state = m_idleState;

        if (m_moveComponent.IsGrounded)
        {
            if (m_moveComponent.IsRunning)
                _state = m_runState;
        }
        else
            _state = m_moveComponent.CurrentVerticalVelocity > 0f ? m_jumpState : m_fallState;

        if (m_meleeComponent.IsMeleeAttacking)
        {
            _state = m_meleeState;
            setAnimatorTimeParameter(m_meleeComponent.MeleeTime);
        }

        if (m_inhaleComponent.IsInhaling)
            _state = m_inhaleState;

        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            _state = m_meditateState;

        setAnimationState(_state);

        updateInputDirectionRotate();
    }

    private void updateInputDirectionRotate()
    {
        transform.localRotation = Quaternion.identity;

        var _targetRotation = Quaternion.identity;

        if (allowInputRotate())
        {
            Vector3 _localInputVector = transform.InverseTransformVector(m_moveComponent.InputWorldDirection);
            _targetRotation = Quaternion.Euler(_localInputVector.z * m_inputLean_Forward, 0f, -_localInputVector.x * m_inputLean_Sideways);
        }

        m_inputRotation = Quaternion.Lerp(m_inputRotation, _targetRotation, GameTime.DeltaTime(TimeChannel.Player) * m_inputLean_Sharpness);

        transform.rotation = transform.rotation * m_inputRotation;
    }

    private void setAnimationState(string newAnimState)
    {
        if (newAnimState == m_currentAnimState)
            return;

        m_currentAnimState = newAnimState;

        if (m_animator.HasState(m_currentAnimState) == false)
        {
            Debug.LogWarning("Player Animator Handler: animator doesn't have state " + m_currentAnimState);
            return;
        }

        m_animator.CrossFadeInFixedTime(m_currentAnimState, 0.1f);
    }

    private void setAnimatorTimeParameter(float time)
    {
        m_animator.SetFloat("MotionTime", time);
    }

    private bool allowInputRotate()
    {
        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            return false;

        if (m_moveComponent.IsRunning == false)
            return false;

        return true;
    }
}
