using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MeditationScreen : SingletonBehaviour<MeditationScreen>
{
    [Header("Visual Settings")]
    [SerializeField] private Vector2 m_abilitySelectionHidePosition = Vector2.zero;
    [SerializeField] private float m_abilitySelectionPanelAppearTime = 5.0f;
    [SerializeField] private float m_abilitySelectionPanelHideTime = 5.0f;

    [Header("Object References")]
    [SerializeField] private MeditationAbilitySelectionUIElement m_abilitySelectionElementPrefab = null;
    [SerializeField] private RectTransform m_abilitySelectionPanelRoot = null;
    [SerializeField] private RectTransform m_abilitySelectionVerticalGroup = null;

    private Vector2 m_anchorMinVelocity = Vector2.zero;
    private Vector2 m_anchorMaxVelocity = Vector2.zero;

    private List<MeditationAbilitySelectionUIElement> m_activeAbilitySelections = new();
    private Stack<MeditationAbilitySelectionUIElement> m_abilitySelectionPool = new();

    private void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            var _newSelectionPrefab = Instantiate(m_abilitySelectionElementPrefab.gameObject, parent: m_abilitySelectionVerticalGroup);
            _newSelectionPrefab.gameObject.SetActiveOptimized(false);
            _newSelectionPrefab.TryGetComponent(out MeditationAbilitySelectionUIElement _element);
            m_abilitySelectionPool.Push(_element);
        }
    }

    private void Update()
    {
        bool _isAbilityPanelVisible = false;

        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating)
            _isAbilityPanelVisible = true;

        var _targetAnchorMin = _isAbilityPanelVisible ? Vector2.zero : m_abilitySelectionHidePosition + Vector2.zero;
        var _targetAnchorMax = _isAbilityPanelVisible ? Vector2.one : m_abilitySelectionHidePosition + Vector2.one;
        float _targetSpeed = _isAbilityPanelVisible ? m_abilitySelectionPanelAppearTime : m_abilitySelectionPanelHideTime;

        m_abilitySelectionPanelRoot.anchorMin = Vector2.SmoothDamp(m_abilitySelectionPanelRoot.anchorMin, _targetAnchorMin, ref m_anchorMinVelocity, _targetSpeed, maxSpeed: float.MaxValue, deltaTime: Time.unscaledDeltaTime);
        m_abilitySelectionPanelRoot.anchorMax = Vector2.SmoothDamp(m_abilitySelectionPanelRoot.anchorMax, _targetAnchorMax, ref m_anchorMaxVelocity, _targetSpeed, maxSpeed: float.MaxValue, deltaTime: Time.unscaledDeltaTime);
        m_abilitySelectionPanelRoot.anchoredPosition = Vector2.zero;
    }

    public void Clear()
    {
        for (int i = 0; i < m_activeAbilitySelections.Count; i++)
        {
            var _element = m_activeAbilitySelections[i];
            _element.gameObject.SetActiveOptimized(false);
            m_abilitySelectionPool.Push(_element);
        }

        m_activeAbilitySelections.Clear();
    }

    public void EnableAbilitySelection(AbilityTypes abilityType)
    {
        var _newElement = m_abilitySelectionPool.Pop();
        _newElement.BindToAbility(abilityType);
        m_activeAbilitySelections.Add(_newElement);
    }
}
