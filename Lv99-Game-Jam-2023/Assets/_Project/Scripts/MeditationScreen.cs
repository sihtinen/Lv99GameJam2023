using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationScreen : MonoBehaviour
{
    [SerializeField] private Vector2 m_abilitySelectionHidePosition = Vector2.zero;
    [SerializeField] private float m_abilitySelectionPanelAppearSpeed = 5.0f;
    [SerializeField] private float m_abilitySelectionPanelHideSpeed = 5.0f;

    [Header("Object References")]
    [SerializeField] private MeditationAbilitySelectionUIElement m_abilitySelectionElementPrefab = null;
    [SerializeField] private RectTransform m_abilitySelectionPanelRoot = null;


    private void Update()
    {
        bool _isAbilityPanelVisible = false;

        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            _isAbilityPanelVisible = true;

        var _targetAnchorMin = _isAbilityPanelVisible ? Vector2.zero : m_abilitySelectionHidePosition + Vector2.zero;
        var _targetAnchorMax = _isAbilityPanelVisible ? Vector2.one : m_abilitySelectionHidePosition + Vector2.one;
        float _targetSpeed = _isAbilityPanelVisible ? Time.deltaTime * m_abilitySelectionPanelAppearSpeed : Time.deltaTime * m_abilitySelectionPanelHideSpeed;

        m_abilitySelectionPanelRoot.anchorMin = Vector2.Lerp(m_abilitySelectionPanelRoot.anchorMin, _targetAnchorMin, _targetSpeed);
        m_abilitySelectionPanelRoot.anchorMax = Vector2.Lerp(m_abilitySelectionPanelRoot.anchorMax, _targetAnchorMax, _targetSpeed);
        m_abilitySelectionPanelRoot.anchoredPosition = Vector2.zero;
    }
}
