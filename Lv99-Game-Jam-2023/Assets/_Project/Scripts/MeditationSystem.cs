using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MeditationSystem : SingletonBehaviour<MeditationSystem>
{
    [Header("Meditation Settings")]
    [SerializeField] private bool m_isJumpAvailable = false;
    [SerializeField] private bool m_isMeleeAvailable = false;
    [SerializeField, Min(1)] private int m_abilityCount = 3;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_jumpActionRef = null;
    [SerializeField] private InputActionReference m_meleeActionRef = null;
    [SerializeField] private InputActionReference m_breathActionRef = null;

    [NonSerialized] public bool IsPlayerMeditating = false;
    [NonSerialized] public bool IsPlayerBreathingForAbility = false;
    [NonSerialized] public AbilityTypes CurrentBreathAbility;
    [NonSerialized] public MeditationPoint OverlappingMeditationPoint = null;
    [NonSerialized] public MeditationPoint PreviousMeditationPoint = null;

    private int m_abilitiesSelected = 0;

    protected override void Awake()
    {
        base.Awake();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.started += this.onJumpInput;
            m_jumpActionRef.action.canceled += this.onJumpInput;
            m_jumpActionRef.action.Enable();
        }

        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.started += this.onMeleeInput;
            m_meleeActionRef.action.canceled += this.onMeleeInput;
            m_meleeActionRef.action.Enable();
        }

        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.started += this.onBreathInput;
            m_breathActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.started -= this.onJumpInput;
            m_jumpActionRef.action.canceled -= this.onJumpInput;
            m_jumpActionRef.action.Disable();
        }

        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.started -= this.onMeleeInput;
            m_meleeActionRef.action.canceled -= this.onMeleeInput;
            m_meleeActionRef.action.Disable();
        }

        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.started -= this.onBreathInput;
            m_breathActionRef.action.Disable();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (m_isJumpAvailable == false)
            return;

        if (context.started)
            startBreathForAbility(AbilityTypes.Jump);

        if (context.canceled)
        {
            if (IsPlayerBreathingForAbility && CurrentBreathAbility == AbilityTypes.Jump)
                endBreathForAbility();
        }
    }

    private void onMeleeInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (m_isMeleeAvailable == false)
            return;

        if (context.started)
            startBreathForAbility(AbilityTypes.Melee);

        if (context.canceled)
        {
            if (IsPlayerBreathingForAbility && CurrentBreathAbility == AbilityTypes.Melee)
                endBreathForAbility();
        }
    }

    private void onBreathInput(InputAction.CallbackContext context)
    {
        if (context.started == false)
            return;

        var _player = PlayerCharacter.Instance;
        if (_player == null)
            return;

        if (PreviousMeditationPoint != null)
            ResetCurrentPuzzle();
        else
        {
            if (OverlappingMeditationPoint == null)
                return;

            PreviousMeditationPoint = OverlappingMeditationPoint;
            startMeditation();
        }
    }

    private void exitMeditation()
    {
        IsPlayerMeditating = false;
        IsPlayerBreathingForAbility = false;

        PreviousMeditationPoint.DeactivateMeditation();

        PlayerCharacterCamera.Instance?.DisableMeditationCamera();
    }

    public void ResetCurrentPuzzle()
    {
        var _transitionScreen = TransitionScreen.Instance;

        if (_transitionScreen == null || _transitionScreen.IsTransitionActive)
            return;

        _transitionScreen.OnScreenObscured += this.onTransitionScreenObscured_resetPuzzle;
        _transitionScreen.StartTransition();
    }

    private void onTransitionScreenObscured_resetPuzzle()
    {
        var _transitionScreen = TransitionScreen.Instance;
        _transitionScreen.OnScreenObscured -= this.onTransitionScreenObscured_resetPuzzle;

        var _playerMoveComponent = PlayerMoveComponent.Instance;
        _playerMoveComponent.SetPositionAndRotation(PreviousMeditationPoint.PlayerMoveTarget);

        startMeditation();
    }

    private void startMeditation()
    {
        IsPlayerMeditating = true;
        m_abilitiesSelected = 0;

        PreviousMeditationPoint.ActivateMeditation();

        var _player = PlayerCharacter.Instance;
        _player.JumpUses = 0;
        _player.MeleeUses = 0;

        AbilityCanvas.Instance?.Initialize(m_abilityCount);

        PlayerCharacterCamera.Instance?.EnableMeditationCamera(PreviousMeditationPoint);

        var _meditationScreen = MeditationScreen.Instance;
        _meditationScreen.Clear();

        if (m_isJumpAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Jump);

        if (m_isMeleeAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Melee);
    }

    private void startBreathForAbility(AbilityTypes abilityType)
    {
        IsPlayerBreathingForAbility = true;
        CurrentBreathAbility = abilityType;
    }

    private void endBreathForAbility()
    {
        IsPlayerBreathingForAbility = false;

        bool _isSuccess = true;

        if (_isSuccess)
        {
            switch (CurrentBreathAbility)
            {
                case AbilityTypes.Jump:
                    PlayerCharacter.Instance.JumpUses++;
                    break;
                case AbilityTypes.Melee:
                    PlayerCharacter.Instance.MeleeUses++;
                    break;
                case AbilityTypes.Ability3:
                    break;
                case AbilityTypes.Ability4:
                    break;
            }

            m_abilitiesSelected++;

            AbilityCanvas.Instance.AddAbilityElement(CurrentBreathAbility);
        }

        if (m_abilitiesSelected >= m_abilityCount)
            exitMeditation();
    }
}
