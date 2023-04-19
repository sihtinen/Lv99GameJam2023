using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTimestop : SingletonBehaviour<PlayerTimestop>
{
    [Header("Settings")]
    [SerializeField] private float m_timeStopDuration = 4.0f;
    [SerializeField] private float m_timeStopTimeScale = 0.05f;

    [Header("Object References")]
    [SerializeField] private InputActionReference m_timeStopInputRef = null;

    private PlayerCharacter m_player = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_player);

        if (m_timeStopInputRef != null)
        {
            m_timeStopInputRef.action.performed += this.onTimeStopInput;
            m_timeStopInputRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_timeStopInputRef != null)
        {
            m_timeStopInputRef.action.performed -= this.onTimeStopInput;
            m_timeStopInputRef.action.Disable();
        }
    }

    private void onTimeStopInput(InputAction.CallbackContext context)
    {
        if (m_player.AllowTimestop == false)
            return;

        GameTimeManager.Instance?.TriggerTimeStop(duration: m_timeStopDuration, timeScale: m_timeStopTimeScale);

        m_player.UseAbility(AbilityTypes.Timestop);
    }
}
