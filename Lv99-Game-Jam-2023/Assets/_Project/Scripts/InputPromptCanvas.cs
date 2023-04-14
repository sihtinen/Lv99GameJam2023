using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPromptCanvas : MonoBehaviour
{
    [Header("Meditate Prompt")]
    [SerializeField] private RectTransform m_meditatePromptRoot = null;
    [SerializeField] private float m_meditatePromptAppearSpeed = 5f;
    [SerializeField] private float m_meditatePromptDisappearSpeed = 9f;

    private Canvas m_canvas = null;
    private TMPro.TMP_Text m_meditatePromptText = null;

    private float m_maxMeditateFontSize;

    private void Awake()
    {
        TryGetComponent(out m_canvas);
        m_canvas.enabled = true;

        m_meditatePromptRoot.TryGetComponent(out m_meditatePromptText);
        m_maxMeditateFontSize = m_meditatePromptText.fontSize;
        m_meditatePromptText.fontSize = 0f;
    }

    private void Update()
    {
        bool _isMeditatePromptActive = false;

        var _meditateSystem = MeditationSystem.Instance;
        if (_meditateSystem != null && _meditateSystem.IsPlayerMeditating == false && _meditateSystem.OverlappingMeditationPoint != null)
            _isMeditatePromptActive = true;

        var _playerCamera = PlayerCharacterCamera.Instance;
        if (_playerCamera != null && _playerCamera.IsAnimating())
            _isMeditatePromptActive = false;

        float _targetFontSize = _isMeditatePromptActive ? m_maxMeditateFontSize : 0f;
        float _speed = _isMeditatePromptActive ? m_meditatePromptAppearSpeed : m_meditatePromptDisappearSpeed;

        m_meditatePromptText.fontSize = Mathf.Lerp(m_meditatePromptText.fontSize, _targetFontSize, Time.deltaTime * _speed);
    }
}
