using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerCharacter : SingletonBehaviour<PlayerCharacter>
{
    [Header("Object References")]
    [SerializeField] private GameObject m_renderMeshRootObj = null;

    [NonSerialized] public bool AllowMovement = false;
    [NonSerialized] public bool AllowJump = false;
    [NonSerialized] public bool AllowMelee = false;

    private bool m_meditationLayersActive = false;
    private PlayerMoveComponent m_moveComponent = null;
    private PlayerMeleeComponent m_meleeComponent = null;


    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_moveComponent);
        TryGetComponent(out m_meleeComponent);
    }

    private void Update()
    {
        AllowMovement = true;
        AllowJump = true;
        AllowMelee = true;

        if (m_moveComponent.IsGrounded == false)
        {
            AllowMelee = false;
        }

        if (m_meleeComponent.IsMeleeAttacking())
        {
            AllowMovement = false;
            AllowJump = false;
        }

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
        {
            AllowMovement = false;
            AllowJump = false;
            AllowMelee = false;

            if (m_meditationLayersActive == false)
                enableMeditationCameraLayer();
        }
        else
        {
            var _playerCamera = PlayerCharacterCamera.Instance;

            if (_playerCamera != null && _playerCamera.IsAnimating())
            {
                AllowMovement = false;
                AllowJump = false;
                AllowMelee = false;
            }

            if (m_meditationLayersActive && _playerCamera != null && _playerCamera.IsAnimating() == false)
                disableMeditationCameraLayer();
        }
    }

    private void enableMeditationCameraLayer()
    {
        m_renderMeshRootObj.layer = LayerMask.NameToLayer("PlayerCharacter");
        m_meditationLayersActive = true;
    }

    public void disableMeditationCameraLayer()
    {
        m_renderMeshRootObj.layer = LayerMask.NameToLayer("Default");
        m_meditationLayersActive = false;
    }
}
