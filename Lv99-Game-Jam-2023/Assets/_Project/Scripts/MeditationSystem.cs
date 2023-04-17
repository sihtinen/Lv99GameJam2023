using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MeditationSystem : SingletonBehaviour<MeditationSystem>
{
    [Header("Meditation Settings")]
    [SerializeField] private float m_minigameSuccessWindowHalf = 0.2f;

    [Header("Debug Settings")]
    [SerializeField] private bool m_debugSkipMinigames = false;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_jumpActionRef = null;
    [SerializeField] private InputActionReference m_meleeActionRef = null;
    [SerializeField] private InputActionReference m_breathActionRef = null;
    [Space]
    [SerializeField] private BreathMinigameCollection m_jumpMinigameCollection = null;
    [SerializeField] private BreathMinigameCollection m_meleeMinigameCollection = null;

    [NonSerialized] public bool IsPlayerMeditating = false;
    [NonSerialized] public bool IsBreathMinigameActive = false;
    [NonSerialized] public bool IsMinigameSuccessWindowActive = false;
    [NonSerialized] public AbilityTypes CurrentBreathAbility;
    [NonSerialized] public MeditationPoint OverlappingMeditationPoint = null;
    [NonSerialized] public MeditationPoint ActiveMeditationPoint = null;
    [NonSerialized] public float CurrentMinigameTime = 0f;
    [NonSerialized] public BreathMinigameSettings CurrentMinigameSettings = null;

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

    private void Update()
    {
        if (IsBreathMinigameActive)
        {
            CurrentMinigameTime += Time.unscaledDeltaTime;

            IsMinigameSuccessWindowActive = Mathf.Abs(CurrentMinigameSettings.Duration - CurrentMinigameTime) < m_minigameSuccessWindowHalf;

            if (CurrentMinigameTime > CurrentMinigameSettings.Duration + m_minigameSuccessWindowHalf)
                endBreathMinigame();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (ActiveMeditationPoint.IsJumpAvailable == false)
            return;

        if (context.started)
            startBreathMinigame(AbilityTypes.Jump);

        if (context.canceled)
        {
            if (IsBreathMinigameActive && CurrentBreathAbility == AbilityTypes.Jump)
                endBreathMinigame();
        }
    }

    private void onMeleeInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (ActiveMeditationPoint.IsMeleeAvailable == false)
            return;

        if (context.started)
            startBreathMinigame(AbilityTypes.Melee);

        if (context.canceled)
        {
            if (IsBreathMinigameActive && CurrentBreathAbility == AbilityTypes.Melee)
                endBreathMinigame();
        }
    }

    private void onBreathInput(InputAction.CallbackContext context)
    {
        if (context.started == false)
            return;

        var _player = PlayerCharacter.Instance;
        if (_player == null)
            return;

        if (ActiveMeditationPoint != null)
            ResetCurrentPuzzle();
        else
        {
            if (OverlappingMeditationPoint == null)
                return;

            ActiveMeditationPoint = OverlappingMeditationPoint;
            startMeditation();
        }
    }

    private void exitMeditation()
    {
        IsPlayerMeditating = false;
        IsBreathMinigameActive = false;

        ActiveMeditationPoint.DeactivateMeditation();

        PlayerMoveComponent.Instance?.ResetVerticalVelocity();

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
        _playerMoveComponent.SetPositionAndRotation(ActiveMeditationPoint.PlayerMoveTarget);

        ActiveMeditationPoint.ResetLinkedPuzzleBehaviors();

        startMeditation();
    }

    private void startMeditation()
    {
        IsPlayerMeditating = true;
        m_abilitiesSelected = 0;

        ActiveMeditationPoint.ActivateMeditation();

        var _player = PlayerCharacter.Instance;
        _player.JumpUses = 0;
        _player.MeleeUses = 0;

        AbilityCanvas.Instance?.Initialize(ActiveMeditationPoint.AbilityCount);

        PlayerCharacterCamera.Instance?.EnableMeditationCamera(ActiveMeditationPoint);

        var _meditationScreen = MeditationScreen.Instance;
        _meditationScreen.Clear();

        if (ActiveMeditationPoint.IsJumpAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Jump);

        if (ActiveMeditationPoint.IsMeleeAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Melee);
    }

    private void startBreathMinigame(AbilityTypes abilityType)
    {
        IsBreathMinigameActive = true;
        CurrentMinigameTime = 0f;
        CurrentBreathAbility = abilityType;

        switch (CurrentBreathAbility)
        {
            case AbilityTypes.Jump:
                CurrentMinigameSettings = m_jumpMinigameCollection.AvailableMinigames.GetRandomElement();
                break;
            case AbilityTypes.Melee:
                CurrentMinigameSettings = m_meleeMinigameCollection.AvailableMinigames.GetRandomElement();
                break;
            case AbilityTypes.Ability3:
                break;
            case AbilityTypes.Ability4:
                break;
        }

        MeditationScreen.Instance?.OnBreathMinigameStarted(CurrentBreathAbility);
    }

    private void endBreathMinigame()
    {
        IsBreathMinigameActive = false;

        if (IsMinigameSuccessWindowActive || (Application.isEditor && m_debugSkipMinigames))
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

            AbilityCanvas.Instance?.AddAbilityElement(CurrentBreathAbility);
        }

        MeditationScreen.Instance?.OnBreathEnded(IsMinigameSuccessWindowActive);

        if (m_abilitiesSelected >= ActiveMeditationPoint.AbilityCount)
            exitMeditation();

        CurrentMinigameSettings = null;
    }
}
