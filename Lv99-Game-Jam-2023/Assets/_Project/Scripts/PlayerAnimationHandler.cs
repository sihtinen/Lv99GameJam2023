using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    [Header("Animation State Names")]
    [SerializeField] private string m_idleState = "Idle";
    [SerializeField] private string m_runState = "Run";
    [SerializeField] private string m_jumpState = "Jump";
    [SerializeField] private string m_meleeState = "Hit";
    [SerializeField] private string m_meditateState = "Meditate";

    private string m_currentAnimState = string.Empty;

    private Animator m_animator = null;
    private PlayerCharacter m_player = null;
    private PlayerMoveComponent m_moveComponent = null;
    private PlayerMeleeComponent m_meleeComponent = null;

    private void Start()
    {
        TryGetComponent(out m_animator);
        transform.root.TryGetComponent(out m_player);
        transform.root.TryGetComponent(out m_moveComponent);
        transform.root.TryGetComponent(out m_meleeComponent);
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
            _state = m_jumpState;

        if (m_meleeComponent.IsMeleeAttacking)
        {
            _state = m_meleeState;
            setAnimatorTimeParameter(m_meleeComponent.MeleeTime);
        }

        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            _state = m_meditateState;

        setAnimationState(_state);
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
}
