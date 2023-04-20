using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class MeditationAbilitySelectionUIElement : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Image m_iconImage = null;
    [SerializeField] private TMP_Text m_ablityNameText = null;
    [SerializeField] private TMP_Text m_inputText = null;
    [Space]
    [SerializeField] private Sprite m_jumpIconSprite = null;
    [SerializeField] private Sprite m_meleeIconSprite = null;
    [SerializeField] private Sprite m_suctionIconSprite = null;
    [SerializeField] private Sprite m_timestopIconSprite = null;

    [NonSerialized] public AbilityTypes AbilityType;
    [NonSerialized] public RectTransform RectTransform = null;

    private void Awake()
    {
        RectTransform = transform as RectTransform;
    }

    public void BindToAbility(AbilityTypes abilityType)
    {
        AbilityType = abilityType;
        m_ablityNameText.SetText(AbilityType.ToString());

        switch (AbilityType)
        {
            default:
            case AbilityTypes.Jump:
                m_iconImage.overrideSprite = m_jumpIconSprite;
                m_inputText.SetText("Press \"<b>Space</b>\"");
                break;
            case AbilityTypes.Pickaxe:
                m_iconImage.overrideSprite = m_meleeIconSprite;
                m_inputText.SetText("Press \"<b>X</b>\"");
                break;
            case AbilityTypes.Inhale:
                m_iconImage.overrideSprite = m_suctionIconSprite;
                m_inputText.SetText("Press \"<b>C</b>\"");
                break;
            case AbilityTypes.Timestop:
                m_iconImage.overrideSprite = m_timestopIconSprite;
                m_inputText.SetText("Press \"<b>V</b>\"");
                break;
        }

        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }
}