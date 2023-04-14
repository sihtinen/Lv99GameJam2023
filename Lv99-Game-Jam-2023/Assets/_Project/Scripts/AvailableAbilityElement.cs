using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class AvailableAbilityElement : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;

    [NonSerialized] public AbilityTypes AbilityType;

    public void Clear()
    {
        gameObject.SetActiveOptimized(false);
        AbilityCanvas.Instance.ReturnToPool(this);
    }

    public void BindToAbility(AbilityTypes abilityType)
    {
        AbilityType = abilityType;
        m_nameText.SetText(abilityType.ToString());
    }
}
