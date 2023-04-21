using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class InputPromptCanvas : MonoBehaviour
{
    [Header("Meditate Prompt")]
    [SerializeField] private RectTransform m_meditatePromptRoot = null;
    [SerializeField] private float m_meditatePromptAppearSpeed = 5f;
    [SerializeField] private float m_meditatePromptDisappearSpeed = 9f;

    [Header("Retry Prompt")]
    [SerializeField] private RectTransform m_retryPromptRoot = null;
    [SerializeField] private float m_retryPromptAppearSpeed = 5f;
    [SerializeField] private float m_retryPromptDisappearSpeed = 9f;

    [Header("Object References")]
    [SerializeField] private SceneReference m_mainMenuSceneRef = null;

    private Canvas m_canvas = null;
    private TMPro.TMP_Text m_meditatePromptText = null;
    private TMPro.TMP_Text m_retryPromptText = null;

    private float m_maxMeditateFontSize;
    private float m_maxRetryFontSize;

    private void Awake()
    {
        TryGetComponent(out m_canvas);
        m_canvas.enabled = true;

        m_meditatePromptRoot.TryGetComponent(out m_meditatePromptText);
        m_maxMeditateFontSize = m_meditatePromptText.fontSize;
        m_meditatePromptText.fontSize = 0f;

        m_retryPromptRoot.TryGetComponent(out m_retryPromptText);
        m_maxRetryFontSize = m_retryPromptText.fontSize;
        m_retryPromptText.fontSize = 0f;
    }

    private void Update()
    {
        updateMeditatePrompt();
        updateRetryPrompt();
    }

    private void updateMeditatePrompt()
    {
        bool _isMeditatePromptActive = isMeditatePromptActive();

        float _targetFontSize = _isMeditatePromptActive ? m_maxMeditateFontSize : 0f;
        float _speed = _isMeditatePromptActive ? m_meditatePromptAppearSpeed : m_meditatePromptDisappearSpeed;

        m_meditatePromptText.fontSize = Mathf.Lerp(m_meditatePromptText.fontSize, _targetFontSize, GameTime.DeltaTime(TimeChannel.Player) * _speed);
    }

    private void updateRetryPrompt()
    {
        bool _isRetryPromptActive = isRetryPromptActive();

        float _targetFontSize = _isRetryPromptActive ? m_maxRetryFontSize : 0f;
        float _speed = _isRetryPromptActive ? m_retryPromptAppearSpeed : m_retryPromptDisappearSpeed;

        m_retryPromptText.fontSize = Mathf.Lerp(m_retryPromptText.fontSize, _targetFontSize, GameTime.DeltaTime(TimeChannel.Player) * _speed);
    }

    private static bool isMeditatePromptActive()
    {
        var _player = PlayerCharacter.Instance;
        if (_player != null && _player.HasAnyUsesLeft())
            return false;

        var _playerCamera = PlayerCharacterCamera.Instance;
        if (_playerCamera != null && _playerCamera.IsAnimating())
            return false;

        bool _isMeditatePromptActive = false;

        var _meditateSystem = MeditationSystem.Instance;
        if (_meditateSystem != null && _meditateSystem.IsPlayerMeditating == false && _meditateSystem.OverlappingMeditationPoint != null)
            _isMeditatePromptActive = true;

        return _isMeditatePromptActive;
    }

    private static bool isRetryPromptActive()
    {
        var _meditateSystem = MeditationSystem.Instance;
        if (_meditateSystem != null && _meditateSystem.ActiveMeditationPoint != null)
            return true;

        return false;
    }

    public void Button_LoadMainMenu()
    {
        SceneLoader.Instance?.LoadScene(m_mainMenuSceneRef);
    }
}
