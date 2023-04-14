using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerCharacter : SingletonBehaviour<PlayerCharacter>
{
    [NonSerialized] public bool AllowMovement = false;
    [NonSerialized] public bool AllowJump = false;
    [NonSerialized] public bool AllowMelee = false;

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
        }
    }
}
