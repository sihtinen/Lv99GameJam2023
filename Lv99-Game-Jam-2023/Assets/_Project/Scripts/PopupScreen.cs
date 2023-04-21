using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

public class PopupScreen : MonoBehaviour
{
    [SerializeField] private float m_fadeInDuration = 1.5f;
    [SerializeField] private AnimationCurve m_fadeInAlphaCurve = new();
    [Space]
    [SerializeField] private float m_textWaitTime = 4.0f;
    [SerializeField] private float m_textFadeInDuration = 1.5f;
    [SerializeField] private AnimationCurve m_textFadeInAlphaCurve = new();
    [Space]
    [SerializeField] private float m_fadeOutDuration = 1.5f;
    [SerializeField] private AnimationCurve m_fadeOutAlphaCurve = new();

    [Header("Object References")]
    [SerializeField] private InputActionReference m_confirmInputRef = null;
    [SerializeField] private CanvasGroup m_rootCanvasGroup = null;
    [SerializeField] private CanvasGroup m_textCanvasGroup = null;
    [SerializeField] private TMP_Text m_inputPromptText = null;
    [SerializeField] private List<CanvasGroup> m_subScreens = new();

    [NonSerialized] public bool IsActive = false;

    private Canvas m_canvas = null;
    private Coroutine m_activeCoroutine = null;

    private int m_subScreenIndex = 0;

    private void Awake()
    {
        TryGetComponent(out m_canvas);
        m_canvas.enabled = false;

        m_rootCanvasGroup.alpha = 0;
        m_textCanvasGroup.alpha = 0;

        if (m_confirmInputRef != null)
        {
            m_confirmInputRef.action.performed += this.onInput;
            m_confirmInputRef.action.Enable();
        }
    }

    private void OnDestroy()
    {
        if (m_confirmInputRef != null)
        {
            m_confirmInputRef.action.performed -= this.onInput;
            m_confirmInputRef.action.Disable();
        }
    }

    private void onInput(InputAction.CallbackContext context)
    {
        if (m_canvas.enabled == false)
            return;

        if (m_activeCoroutine != null)
            return;

        if (m_subScreens.Count == 0 || m_subScreenIndex >= m_subScreens.Count - 1)
        {
            m_activeCoroutine = StartCoroutine(coroutine_fadeOut());
        }
        else
        {
            m_activeCoroutine = StartCoroutine(coroutine_toNextSubScreen());
        }
    }

    public void Activate()
    {
        IsActive = true;

        if (m_subScreens.Count > 0)
        {
            m_subScreenIndex = 0;
            m_subScreens[m_subScreenIndex].alpha = 1;
            m_subScreens[m_subScreenIndex].gameObject.SetActiveOptimized(true);
        }

        m_canvas.enabled = true;
        m_activeCoroutine = StartCoroutine(coroutine_lifetime());
    }

    private IEnumerator coroutine_lifetime()
    {
        float _timer = 0f;

        while (_timer < m_fadeInDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_fadeInDuration;
            m_rootCanvasGroup.alpha = m_fadeInAlphaCurve.Evaluate(_fadeInPos);
        }

        _timer = 0f;

        while (_timer < m_textWaitTime)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);
        }

        _timer = 0f;

        while (_timer < m_textFadeInDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_textFadeInDuration;
            m_textCanvasGroup.alpha = m_textFadeInAlphaCurve.Evaluate(_fadeInPos);
        }

        m_activeCoroutine = null;
    }

    private IEnumerator coroutine_fadeOut()
    {
        float _timer = 0f;

        while (_timer < m_fadeOutDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_fadeOutDuration;
            m_rootCanvasGroup.alpha = m_fadeOutAlphaCurve.Evaluate(_fadeInPos);
        }

        m_activeCoroutine = null;

        m_canvas.enabled = false;

        IsActive = false;
    }

    private IEnumerator coroutine_toNextSubScreen()
    {
        float _timer = 0f;

        while (_timer < m_fadeOutDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_fadeOutDuration;
            m_subScreens[m_subScreenIndex].alpha = m_fadeOutAlphaCurve.Evaluate(_fadeInPos);
            m_textCanvasGroup.alpha = m_subScreens[m_subScreenIndex].alpha;
        }

        m_subScreens[m_subScreenIndex].gameObject.SetActiveOptimized(false);
        m_subScreenIndex++;
        m_subScreens[m_subScreenIndex].alpha = 0;
        m_subScreens[m_subScreenIndex].gameObject.SetActiveOptimized(true);

        _timer = 0f;

        while (_timer < m_fadeInDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_fadeInDuration;
            m_subScreens[m_subScreenIndex].alpha = m_fadeInAlphaCurve.Evaluate(_fadeInPos);
        }

        _timer = 0f;

        while (_timer < m_textWaitTime)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);
        }

        _timer = 0f;

        while (_timer < m_textFadeInDuration)
        {
            yield return null;
            _timer += GameTime.DeltaTime(TimeChannel.Player);

            float _fadeInPos = _timer / m_textFadeInDuration;
            m_textCanvasGroup.alpha = m_textFadeInAlphaCurve.Evaluate(_fadeInPos);
        }

        m_activeCoroutine = null;
    }
}
