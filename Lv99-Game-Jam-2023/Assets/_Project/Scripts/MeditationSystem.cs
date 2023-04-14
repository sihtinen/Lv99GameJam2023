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
    [NonSerialized] public MeditationPoint OverlappingMeditationPoint = null;
    [NonSerialized] public MeditationPoint SelectedMeditationPoint = null;

    private int m_abilitiesSelected = 0;

    protected override void Awake()
    {
        base.Awake();

        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.performed += this.onJumpInput;
            m_jumpActionRef.action.Enable();
        }

        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.performed += this.onMeleeInput;
            m_meleeActionRef.action.Enable();
        }

        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.performed += this.onBreathInput;
            m_breathActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_jumpActionRef != null)
        {
            m_jumpActionRef.action.performed -= this.onJumpInput;
            m_jumpActionRef.action.Disable();
        }

        if (m_meleeActionRef != null)
        {
            m_meleeActionRef.action.performed -= this.onMeleeInput;
            m_meleeActionRef.action.Disable();
        }

        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.performed -= this.onBreathInput;
            m_breathActionRef.action.Disable();
        }
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (context.started)
        {

        }

        if (context.canceled)
        {

        }
    }

    private void onMeleeInput(InputAction.CallbackContext context)
    {
        if (IsPlayerMeditating == false)
            return;

        if (context.started)
        {

        }

        if (context.canceled)
        {

        }
    }

    private void onBreathInput(InputAction.CallbackContext context)
    {
        if (context.performed == false)
            return;

        if (IsPlayerMeditating)
            exitMeditation();
        else
        {
            if (OverlappingMeditationPoint == null)
                return;

            startMeditation();
        }
    }

    private void exitMeditation()
    {
        IsPlayerMeditating = false;
        SelectedMeditationPoint = null;
        PlayerCharacterCamera.Instance?.DisableMeditationCamera();
    }

    private void startMeditation()
    {
        IsPlayerMeditating = true;
        SelectedMeditationPoint = OverlappingMeditationPoint;
        m_abilitiesSelected = 0;

        AbilityCanvas.Instance?.Initialize(m_abilityCount);

        PlayerCharacterCamera.Instance?.EnableMeditationCamera(SelectedMeditationPoint);

        var _meditationScreen = MeditationScreen.Instance;
        _meditationScreen.Clear();

        if (m_isJumpAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Jump);

        if (m_isMeleeAvailable)
            _meditationScreen.EnableAbilitySelection(AbilityTypes.Melee);
    }
}
