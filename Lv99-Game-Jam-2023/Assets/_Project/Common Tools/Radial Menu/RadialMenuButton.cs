using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class RadialMenuButton : PoolableBehaviour<RadialMenuButton>
{
    public IRadialMenuOption OptionBinding => m_optionBinding;
    public bool IsAnimating => m_activeCoroutine != null;

    [NonSerialized] public RectTransform RectTransformComponent = null;
    [NonSerialized] public Image ImageComponent = null;

    [Header("Object References")]
    [SerializeField] private RadialMenuButtonSettings m_settings = null;

    [SerializeField] private RectTransform m_textRotationPivot = null;
    [SerializeField] private RectTransform m_textDistanceTransform = null;
    [SerializeField] private TMP_Text m_buttonLabelText = null;

    private IRadialMenuOption m_optionBinding = null;
    private Coroutine m_activeCoroutine = null;

    private void Awake()
    {
        RectTransformComponent = transform as RectTransform;
        TryGetComponent(out ImageComponent);
    }

    public void Initialize(IRadialMenuOption option, int index)
    {
        m_optionBinding = option;

        m_buttonLabelText.SetText((index+1).ToStringMinimalAlloc());
        m_buttonLabelText.enabled = true;

        m_textDistanceTransform.anchorMax = new Vector2(0.5f, m_settings.TextDistanceFromCenter);
        m_textDistanceTransform.anchoredPosition = Vector2.zero;

        m_textRotationPivot.localEulerAngles = new Vector3(0, 0, -180 * ImageComponent.fillAmount);
        m_buttonLabelText.rectTransform.eulerAngles = Vector3.zero;

        OnHighlightEnd();

        transform.localScale = Vector3.one;

        gameObject.name = $"Radial Menu Button - {option.UILabel}";
        gameObject.SetActiveOptimized(true);
    }

    public void OnHighlightBegin()
    {
        ImageComponent.color = m_settings.HighlightColor;
    }

    public void OnHighlightEnd()
    {
        ImageComponent.color = m_settings.NormalColor;
    }

    public void StartSelectionAnimation(bool thisButtonSelected)
    {
        if (m_activeCoroutine != null)
            return;

        m_activeCoroutine = StartCoroutine(animation_selectionRegistered(thisButtonSelected));
    }

    protected override void resetAndClearBindings()
    {
        StopAllCoroutines();
        m_activeCoroutine = null;

        OnHighlightEnd();
        m_optionBinding = null;
    }

    private IEnumerator animation_selectionRegistered(bool thisButtonSelected)
    {
        if (thisButtonSelected == false)
            m_buttonLabelText.enabled = false;

        float _timer = 0f;

        while (_timer < m_settings.SelectionAnimationLength)
        {
            _timer += Time.unscaledDeltaTime;
            float _animPos = _timer / m_settings.SelectionAnimationLength;

            var _startColor = thisButtonSelected ? m_settings.HighlightColor : m_settings.NormalColor;
            var _endColor = thisButtonSelected ? m_settings.SelectedColor : m_settings.NotSelectedColor;
            var _colorCurve = thisButtonSelected ? m_settings.SelectedColorCurve : m_settings.NotSelectedColorCurve;

            float _colorCurveValue = _colorCurve.Evaluate(_animPos);
            ImageComponent.color = Color.Lerp(_startColor, _endColor, _colorCurveValue);

            yield return null;
        }

        m_activeCoroutine = null;

        if (thisButtonSelected && m_optionBinding != null)
            m_optionBinding.OnSelected();
    }
}