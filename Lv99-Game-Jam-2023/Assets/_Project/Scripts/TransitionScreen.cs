using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TransitionScreen : SingletonBehaviour<TransitionScreen>
{
    [Header("Transition Settings")]
    [SerializeField] private float m_fadeDuration = 1.0f;
    [SerializeField] private Vector2 m_anchorMin_StartTransition = Vector2.zero;
    [SerializeField] private Vector2 m_anchorMax_StartTransition = Vector2.zero;
    [SerializeField] private Vector2 m_anchorMin_EndTransition = Vector2.zero;
    [SerializeField] private Vector2 m_anchorMax_EndTransition = Vector2.zero;

    [Header("Object References")]
    [SerializeField] private RectTransform m_fillRect = null;

    public event Action OnScreenObscured = null;

    [NonSerialized] public bool AllowTransition = true;
    [NonSerialized] public bool IsTransitionActive = false;

    private Canvas m_canvas = null;
    private Coroutine m_transitionCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_canvas);
        m_canvas.enabled = false;
    }

    public void StartTransition()
    {
        if (m_transitionCoroutine != null)
            StopCoroutine(m_transitionCoroutine);

        m_transitionCoroutine = StartCoroutine(coroutine_performScreenFade());
    }

    private IEnumerator coroutine_performScreenFade()
    {
        IsTransitionActive = true;

        float _timer = 0f;

        m_fillRect.anchorMin = m_anchorMin_StartTransition;
        m_fillRect.anchorMax = m_anchorMax_StartTransition;
        m_fillRect.anchoredPosition = Vector2.zero;
        m_canvas.enabled = true;

        while (_timer < m_fadeDuration)
        {
            yield return null;

            _timer += Time.unscaledDeltaTime;
            float _animPos = _timer / m_fadeDuration;

            m_fillRect.anchorMin = Vector2.Lerp(m_anchorMin_StartTransition, Vector2.zero, _animPos);
            m_fillRect.anchorMax = Vector2.Lerp(m_anchorMax_StartTransition, Vector2.one, _animPos);
            m_fillRect.anchoredPosition = Vector2.zero;
        }

        m_fillRect.anchorMin = Vector2.zero;
        m_fillRect.anchorMax = Vector2.one;
        m_fillRect.anchoredPosition = Vector2.zero;

        AllowTransition = true;
        OnScreenObscured?.Invoke();

        while (AllowTransition == false)
            yield return null;

        _timer = 0f;

        while (_timer < m_fadeDuration)
        {
            yield return null;

            _timer += Time.unscaledDeltaTime;
            float _animPos = _timer / m_fadeDuration;

            m_fillRect.anchorMin = Vector2.Lerp(Vector2.zero, m_anchorMin_EndTransition, _animPos);
            m_fillRect.anchorMax = Vector2.Lerp(Vector2.one, m_anchorMax_EndTransition, _animPos);
            m_fillRect.anchoredPosition = Vector2.zero;
        }

        m_canvas.enabled = false;
        IsTransitionActive = false;
        m_transitionCoroutine = null;
    }
}
