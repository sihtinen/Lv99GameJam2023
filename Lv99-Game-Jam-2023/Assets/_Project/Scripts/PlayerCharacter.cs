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
    [NonSerialized] public bool AllowInhale = false;

    [NonSerialized] public int JumpUses = 0;
    [NonSerialized] public int MeleeUses = 0;
    [NonSerialized] public int InhaleUses = 0;

    private bool m_meditationLayersActive = false;
    private PlayerMoveComponent m_moveComponent = null;
    private PlayerMeleeComponent m_meleeComponent = null;
    private PlayerInhaleComponent m_inhaleComponent = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_moveComponent);
        TryGetComponent(out m_meleeComponent);
        TryGetComponent(out m_inhaleComponent);
    }

    private void Update()
    {
        AllowMovement = true;
        AllowJump = JumpUses > 0;
        AllowMelee = MeleeUses > 0;
        AllowInhale = InhaleUses > 0;

        if (m_moveComponent.IsGrounded == false)
        {
            AllowMelee = false;
            AllowInhale = false;
        }

        if (m_inhaleComponent.IsInhaling)
        {
            AllowMovement = false;
            AllowJump = false;
            AllowMelee = false;
        }

        if (m_meleeComponent.IsMeleeAttacking)
        {
            AllowMovement = false;
            AllowJump = false;
            AllowMelee = false;
        }

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
        {
            AllowMovement = false;
            AllowJump = false;
            AllowMelee = false;
            AllowInhale = false;

            if (m_meditationLayersActive == false)
                enableMeditationCameraLayer();

            m_moveComponent.MoveTowards(_meditationSystem.ActiveMeditationPoint.PlayerMoveTarget);
        }
        else
        {
            var _playerCamera = PlayerCharacterCamera.Instance;

            if (_playerCamera != null && _playerCamera.IsAnimating())
            {
                AllowMovement = false;
                AllowJump = false;
                AllowMelee = false;
                AllowInhale = false;
            }

            if (m_meditationLayersActive && _playerCamera != null && _playerCamera.IsAnimating() == false)
                disableMeditationCameraLayer();
        }
    }

    public void UseAbility(AbilityTypes abilityType)
    {
        switch (abilityType)
        {
            case AbilityTypes.Jump:
                JumpUses--;
                break;
            case AbilityTypes.Melee:
                MeleeUses--;
                break;
            case AbilityTypes.Inhale:
                InhaleUses--;
                break;
            case AbilityTypes.Ability4:
                break;
        }

        AbilityCanvas.Instance?.OnAbilityUsed(abilityType);
    }

    private void enableMeditationCameraLayer()
    {
        setRenderMeshLayer(LayerMask.NameToLayer("PlayerCharacter"));
        m_meditationLayersActive = true;
    }

    public void disableMeditationCameraLayer()
    {
        setRenderMeshLayer(LayerMask.NameToLayer("Default"));
        m_meditationLayersActive = false;
    }

    private List<Transform> m_tempTransformList = new();

    private void setRenderMeshLayer(int layer)
    {
        m_renderMeshRootObj.GetComponentsInChildren<Transform>(includeInactive: true, m_tempTransformList);

        for (int i = 0; i < m_tempTransformList.Count; i++)
            m_tempTransformList[i].gameObject.layer = layer;

        m_tempTransformList.Clear();
    }

    public bool HasAnyUsesLeft()
    {
        return JumpUses > 0 || MeleeUses > 0;
    }
}
