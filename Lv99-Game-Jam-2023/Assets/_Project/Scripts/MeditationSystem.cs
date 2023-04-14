using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class MeditationSystem : SingletonBehaviour<MeditationSystem>
{
    [SerializeField] private bool m_isJumpAvailable = false;
    [SerializeField] private bool m_isMeleeAvailable = false;
    [SerializeField, Min(1)] private int m_abilityCount = 3;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_breathActionRef = null;

    [NonSerialized] public bool IsPlayerMeditating = false;
    [NonSerialized] public MeditationPoint OverlappingMeditationPoint = null;
    [NonSerialized] public MeditationPoint SelectedMeditationPoint = null;

    protected override void Awake()
    {
        base.Awake();

        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.performed += this.onBreathInput;
            m_breathActionRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_breathActionRef != null)
        {
            m_breathActionRef.action.performed -= this.onBreathInput;
            m_breathActionRef.action.Disable();
        }
    }

    private void onBreathInput(InputAction.CallbackContext context)
    {
        if (context.performed == false)
            return;

        if (IsPlayerMeditating)
        {
            IsPlayerMeditating = false;
            SelectedMeditationPoint = null;

            PlayerCharacterCamera.Instance?.DisableMeditationCamera();
        }
        else
        {
            if (OverlappingMeditationPoint == null)
                return;

            IsPlayerMeditating = true;
            SelectedMeditationPoint = OverlappingMeditationPoint;

            PlayerCharacterCamera.Instance?.EnableMeditationCamera(SelectedMeditationPoint);
        }
    }
}
