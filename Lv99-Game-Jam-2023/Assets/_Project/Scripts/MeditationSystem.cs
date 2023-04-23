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
    [SerializeField] private InputActionReference m_inhaleActionRef = null;
    [SerializeField] private InputActionReference m_timestopActionRef = null;
    [Space]
    [SerializeField] private BreathMinigameCollection m_jumpMinigameCollection = null;
    [SerializeField] private BreathMinigameCollection m_meleeMinigameCollection = null;
    [Space]
    [SerializeField] private BreathMinigameAudioPlayer m_audioPlayerBreathIn = null;
    [SerializeField] private BreathMinigameAudioPlayer m_audioPlayerBreathOut = null;

    [NonSerialized] public bool IsPlayerMeditating = false;
    [NonSerialized] public bool IsBreathMinigameActive = false;
    [NonSerialized] public bool IsMinigameSuccessWindowActive = false;
    [NonSerialized] public AbilityTypes CurrentBreathAbility;
    [NonSerialized] public MeditationPoint OverlappingMeditationPoint = null;
    [NonSerialized] public MeditationPoint ActiveMeditationPoint = null;
    [NonSerialized] public float CurrentMinigameTime = 0f;
    [NonSerialized] public BreathMinigameSettings CurrentMinigameSettings = null;

    private int m_abilitiesSelected = 0;

    public float MinigameSuccessWindowHalf => m_minigameSuccessWindowHalf;

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

        if (m_inhaleActionRef != null)
        {
            m_inhaleActionRef.action.started += this.onInhaleInput;
            m_inhaleActionRef.action.canceled += this.onInhaleInput;
            m_inhaleActionRef.action.Enable();
        }

        if (m_timestopActionRef != null)
        {
            m_timestopActionRef.action.started += this.onTimestopInput;
            m_timestopActionRef.action.canceled += this.onTimestopInput;
            m_timestopActionRef.action.Enable();
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

        if (m_inhaleActionRef != null)
        {
            m_inhaleActionRef.action.started -= this.onInhaleInput;
            m_inhaleActionRef.action.canceled -= this.onInhaleInput;
            m_inhaleActionRef.action.Disable();
        }

        if (m_timestopActionRef != null)
        {
            m_timestopActionRef.action.started -= this.onTimestopInput;
            m_timestopActionRef.action.canceled -= this.onTimestopInput;
            m_timestopActionRef.action.Disable();
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

            if (CurrentMinigameTime > CurrentMinigameSettings.Duration + m_minigameSuccessWindowHalf * 2f)
                endBreathMinigame();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (ActiveMeditationPoint.IsJumpAvailable == false)
            return;

        if (context.started && IsBreathMinigameActive == false)
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

        if (context.started && IsBreathMinigameActive == false)
            startBreathMinigame(AbilityTypes.Pickaxe);

        if (context.canceled)
        {
            if (IsBreathMinigameActive && CurrentBreathAbility == AbilityTypes.Pickaxe)
                endBreathMinigame();
        }
    }

    private void onInhaleInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (ActiveMeditationPoint.IsInhaleAvailable == false)
            return;

        if (context.started && IsBreathMinigameActive == false)
            startBreathMinigame(AbilityTypes.Inhale);

        if (context.canceled)
        {
            if (IsBreathMinigameActive && CurrentBreathAbility == AbilityTypes.Inhale)
                endBreathMinigame();
        }
    }

    private void onTimestopInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (ActiveMeditationPoint.IsTimestopAvailable == false)
            return;

        if (context.started && IsBreathMinigameActive == false)
            startBreathMinigame(AbilityTypes.Timestop);

        if (context.canceled)
        {
            if (IsBreathMinigameActive && CurrentBreathAbility == AbilityTypes.Timestop)
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

        if (IsPlayerMeditating)
        {
            ResetCurrentPuzzle();
        }
        else if (ActiveMeditationPoint != null && OverlappingMeditationPoint == null)
        {
            ResetCurrentPuzzle();
        }
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

        GameTimeManager.Instance?.OnResetPuzzle();

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

        if (ActiveMeditationPoint.IsMeleeAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Pickaxe);

        if (ActiveMeditationPoint.IsJumpAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Jump);

        if (ActiveMeditationPoint.IsInhaleAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Inhale);

        if (ActiveMeditationPoint.IsTimestopAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Timestop);
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
            case AbilityTypes.Pickaxe:
                CurrentMinigameSettings = m_meleeMinigameCollection.AvailableMinigames.GetRandomElement();
                break;
            case AbilityTypes.Inhale:
                CurrentMinigameSettings = m_jumpMinigameCollection.AvailableMinigames.GetRandomElement();
                break;
            case AbilityTypes.Timestop:
                CurrentMinigameSettings = m_jumpMinigameCollection.AvailableMinigames.GetRandomElement();
                break;
        }

        MeditationScreen.Instance?.OnBreathMinigameStarted(CurrentBreathAbility);

        m_audioPlayerBreathIn.PlayBreathIn();
    }

    private void endBreathMinigame()
    {
        IsBreathMinigameActive = false;

        if (Application.isEditor && m_debugSkipMinigames)
            IsMinigameSuccessWindowActive = true;

        if (IsMinigameSuccessWindowActive)
        {
            switch (CurrentBreathAbility)
            {
                case AbilityTypes.Jump:
                    PlayerCharacter.Instance.JumpUses++;
                    break;
                case AbilityTypes.Pickaxe:
                    PlayerCharacter.Instance.MeleeUses++;
                    break;
                case AbilityTypes.Inhale:
                    PlayerCharacter.Instance.InhaleUses++;
                    break;
                case AbilityTypes.Timestop:
                    PlayerCharacter.Instance.TimestopUses++;
                    break;
            }

            m_abilitiesSelected++;

            AbilityCanvas.Instance?.AddAbilityElement(CurrentBreathAbility);
        }

        MeditationScreen.Instance?.OnMinigameEnded(IsMinigameSuccessWindowActive);

        MeditationAudioManager.Instance?.OnMinigameEnded(IsMinigameSuccessWindowActive);

        m_audioPlayerBreathIn.PlayBreathOut();

        if (m_abilitiesSelected >= ActiveMeditationPoint.AbilityCount)
            exitMeditation();

        CurrentMinigameSettings = null;
    }

    public float GetMeditationProgress()
    {
        if (ActiveMeditationPoint == null)
            return 0f;

        int _abilityCount = 0;

        var _player = PlayerCharacter.Instance;
        if (_player != null)
        {
            _abilityCount += _player.JumpUses;
            _abilityCount += _player.MeleeUses;
            _abilityCount += _player.InhaleUses;
            _abilityCount += _player.TimestopUses;
        }

        return (float)_abilityCount / (float)ActiveMeditationPoint.AbilityCount;
    }
}
