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
    [NonSerialized] public RectTransform RectTransform = null;

    private void Awake()
    {
        RectTransform = transform as RectTransform;
    }

    public void BindToAbility(AbilityTypes abilityType)
    {
        AbilityType = abilityType;
        m_labelText.SetText(AbilityType.ToString());
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
}