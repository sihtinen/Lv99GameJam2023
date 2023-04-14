using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class MeditationAbilitySelectionUIElement : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_labelText = null;

    [NonSerialized] public AbilityTypes AbilityType;

    public void BindToAbility(AbilityTypes abilityType)
    {
        AbilityType = abilityType;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
}