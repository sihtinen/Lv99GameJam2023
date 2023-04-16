using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class MeditationScreen : SingletonBehaviour<MeditationScreen>
{
    [Header("Visual Settings")]
    [SerializeField] private Vector2 m_abilitySelectionHidePosition = Vector2.zero;
    [SerializeField] private float m_abilitySelectionPanelAppearTime = 5.0f;
    [SerializeField] private float m_abilitySelectionPanelHideTime = 5.0f;
    [Space]
    [SerializeField] private Vector2 m_breathAbilityCenterOffset = new Vector2(0f, 0f);
    [SerializeField] private float m_breathAbilityMoveToCenterTime = 0.5f;
    [SerializeField] private AnimationCurve m_breathAbilityMoveToCenterCurve = new AnimationCurve();
    [SerializeField] private float m_breathAbilityMaxScale = 1.1f;
    [SerializeField] private AnimationCurve m_breathAbilityScaleCurve = new AnimationCurve();

    [Header("Object References")]
    [SerializeField] private MeditationAbilitySelectionUIElement m_abilitySelectionElementPrefab = null;
    [SerializeField] private RectTransform m_abilitySelectionPanelRoot = null;
    [SerializeField] private RectTransform m_abilitySelectionVerticalGroup = null;
    [SerializeField] private RectTransform m_minigameCenter = null;
    [SerializeField] private RectTransform m_minigameTarget = null;

    private float m_centeredAbilityMoveTime = 0f;
    private Vector2 m_anchorMinVelocity = Vector2.zero;
    private Vector2 m_anchorMaxVelocity = Vector2.zero;
    private Vector2 m_centeredElementStartPosition = Vector2.zero;

    private MeditationAbilitySelectionUIElement m_centeredAbilityElement = null;
    private Image m_minigameCenterImage = null;
    private Image m_minigameTargetImage = null;
    private List<MeditationAbilitySelectionUIElement> m_activeAbilitySelections = new();
    private Stack<MeditationAbilitySelectionUIElement> m_abilitySelectionPool = new();

    private void Start()
    {
        m_minigameCenter.TryGetComponent(out m_minigameCenterImage);
        m_minigameTarget.TryGetComponent(out m_minigameTargetImage);

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

        if (_meditationSystem != null && _meditationSystem.IsPlayerMeditating && _meditationSystem.IsBreathMinigameActive == false)
            _isAbilityPanelVisible = true;

        var _targetAnchorMin = _isAbilityPanelVisible ? Vector2.zero : m_abilitySelectionHidePosition + Vector2.zero;
        var _targetAnchorMax = _isAbilityPanelVisible ? Vector2.one : m_abilitySelectionHidePosition + Vector2.one;
        float _targetSpeed = _isAbilityPanelVisible ? m_abilitySelectionPanelAppearTime : m_abilitySelectionPanelHideTime;

        m_abilitySelectionPanelRoot.anchorMin = Vector2.SmoothDamp(m_abilitySelectionPanelRoot.anchorMin, _targetAnchorMin, ref m_anchorMinVelocity, _targetSpeed, maxSpeed: float.MaxValue, deltaTime: Time.unscaledDeltaTime);
        m_abilitySelectionPanelRoot.anchorMax = Vector2.SmoothDamp(m_abilitySelectionPanelRoot.anchorMax, _targetAnchorMax, ref m_anchorMaxVelocity, _targetSpeed, maxSpeed: float.MaxValue, deltaTime: Time.unscaledDeltaTime);
        m_abilitySelectionPanelRoot.anchoredPosition = Vector2.zero;

        if (_meditationSystem.IsBreathMinigameActive && m_centeredAbilityElement != null)
        {
            m_centeredAbilityMoveTime += Time.unscaledDeltaTime / m_breathAbilityMoveToCenterTime;

            m_centeredAbilityElement.RectTransform.anchoredPosition = Vector2.Lerp(
                m_centeredElementStartPosition, 
                m_breathAbilityCenterOffset, 
                m_breathAbilityMoveToCenterCurve.Evaluate(m_centeredAbilityMoveTime));

            float _scalePos = m_breathAbilityScaleCurve.Evaluate(m_centeredAbilityMoveTime);
            float _scale = Mathf.LerpUnclamped(1.0f, m_breathAbilityMaxScale, _scalePos);
            m_centeredAbilityElement.RectTransform.localScale = new Vector3(_scale, _scale, _scale);
        }

        updateMinigame();
    }

    private void updateMinigame()
    {
        var _meditationSystem = MeditationSystem.Instance;

        if (_meditationSystem == null || _meditationSystem.IsBreathMinigameActive == false)
        {
            m_minigameCenterImage.enabled = false;
            m_minigameTargetImage.enabled = false;
            return;
        }

        var _minigameSettings = _meditationSystem.CurrentMinigameSettings;

        float _minigameTimeNormalized = _meditationSystem.CurrentMinigameTime / _minigameSettings.Duration;
        float _posX = _minigameSettings.PositionXCurve.Evaluate(_minigameTimeNormalized);
        float _posY = _minigameSettings.PositionYCurve.Evaluate(_minigameTimeNormalized);

        m_minigameTarget.anchoredPosition = new Vector2(
            Mathf.LerpUnclamped(_minigameSettings.CenterOffset.x, 0f, _posX),
            Mathf.LerpUnclamped(_minigameSettings.CenterOffset.y, 0f, _posY));

        var _targetColor = _meditationSystem.IsMinigameSuccessWindowActive ? Color.green : Color.white;
        _targetColor.a = _meditationSystem.CurrentMinigameTime < 0.5f ? _meditationSystem.CurrentMinigameTime * 2f : 1.0f;

        m_minigameCenterImage.color = _targetColor;
        m_minigameTargetImage.color = _targetColor;

        m_minigameCenterImage.enabled = true;
        m_minigameTargetImage.enabled = true;
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
        _newElement.RectTransform.SetParent(m_abilitySelectionVerticalGroup);
        _newElement.RectTransform.localScale = Vector3.one;
        _newElement.BindToAbility(abilityType);
        m_activeAbilitySelections.Add(_newElement);
    }

    public void OnBreathMinigameStarted(AbilityTypes ability)
    {
        m_centeredAbilityElement = m_abilitySelectionPool.Pop();
        m_centeredAbilityElement.transform.SetParent(transform);
        m_centeredAbilityElement.BindToAbility(ability);

        var _matchingElement = getMatchingElement(ability);
        m_centeredAbilityElement.RectTransform.sizeDelta = _matchingElement.RectTransform.sizeDelta;
        m_centeredAbilityElement.RectTransform.position = _matchingElement.RectTransform.position;
        m_centeredElementStartPosition = m_centeredAbilityElement.RectTransform.anchoredPosition;

        m_centeredAbilityMoveTime = 0f;
    }

    public void OnBreathEnded(bool success)
    {
        if (m_centeredAbilityElement != null)
        {
            m_centeredAbilityElement.gameObject.SetActiveOptimized(false);
            m_abilitySelectionPool.Push(m_centeredAbilityElement);
            m_centeredAbilityElement = null;
        }
    }

    private MeditationAbilitySelectionUIElement getMatchingElement(AbilityTypes ability)
    {
        for (int i = 0; i < m_activeAbilitySelections.Count; i++)
        {
            var _element = m_activeAbilitySelections[i];

            if (_element.AbilityType == ability)
                return _element;
        }

        return null;
    }
}
