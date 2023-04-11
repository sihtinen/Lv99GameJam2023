using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{
    public event Action<IRadialMenuOption> OnHighlightOptionChanged = null;
    public event Action<IRadialMenuOption> OnOptionSelected = null;

    [NonSerialized] public IRadialMenuOption OverrideMenuOption = null;

    [Header("Settings")]
    [SerializeField] private bool m_allowMouseInteractionInsideCenter = false;
    [SerializeField] private bool m_allowMouseInteractionOutsideRadius = false;

    [Header("Object References")]
    [SerializeField] private RectTransform m_centerRootTransform = null;
    [SerializeField] private RectTransform m_directionIndicatorTransform = null;
    [SerializeField] private TMP_Text m_labelText = null;

    private RectTransform m_rectTransform = null;
    private RadialMenuButton m_highlightButton = null;
    private List<IRadialMenuOption> m_options = new List<IRadialMenuOption>();
    private List<RadialMenuButton> m_activeButtons = new List<RadialMenuButton>();

    private void Awake()
    {
        m_rectTransform = transform as RectTransform;

        if (TryGetComponent(out Image _image))
            _image.enabled = false;
    }

    public void PopulateMenu(List<IRadialMenuOption> options)
    {
        m_options.Clear();
        m_options.AddRange(options);
        RebuildLayout();
    }

    public void SetOverrideHighlightOption(IRadialMenuOption menuOption)
    {
        m_directionIndicatorTransform.gameObject.SetActiveOptimized(false);
        setHighlightButton(null);

        OverrideMenuOption = menuOption;

        if (OverrideMenuOption == null)
            return;

        for (int i = 0; i < m_options.Count; i++)
        {
            var _option = m_options[i];

            if (_option.GUID.ToString() == OverrideMenuOption.GUID.ToString())
            {
                setHighlightButton(m_activeButtons[i]);
                break;
            }
        }
    }

    public void RebuildLayout()
    {
        OverrideMenuOption = null;

        m_highlightButton = null;
        m_labelText.SetText(string.Empty);

        m_activeButtons.Clear();
        RadialMenuButtonPool.ResetUsedObjects();
        RadialMenuDividerPool.ResetUsedObjects();

        if (m_options == null || m_options.Count == 0)
            return;

        int _optionCount = m_options.Count;
        float _optionFillAngle = 360f / _optionCount;
        float _halfOptionAngle = _optionFillAngle / 2;

        for (int i = 0; i < _optionCount; i++)
        {
            var _newButton = RadialMenuButtonPool.Get();
            _newButton.ImageComponent.fillAmount = _optionFillAngle / 360f;
            _newButton.RectTransformComponent.localEulerAngles = new Vector3(0, 0, _halfOptionAngle - (i * _optionFillAngle));
            _newButton.Initialize(m_options[i], i);
            m_activeButtons.Add(_newButton);
        }

        for (int i = 0; i < _optionCount; i++)
        {
            var _newDivider = RadialMenuDividerPool.Get();
            _newDivider.RectTransformComponent.localEulerAngles = new Vector3(0, 0, _halfOptionAngle - (i * _optionFillAngle));
            _newDivider.RectTransformComponent.SetAsLastSibling();
            _newDivider.gameObject.SetActiveOptimized(true);
        }

        m_centerRootTransform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        if (m_options == null || m_options.Count == 0)
            return;

        updateMenu();
        pollUserInput();
    }

    private void updateMenu()
    {
        if (OverrideMenuOption != null)
            return;

        if (IsAnimating())
        {
            m_directionIndicatorTransform.gameObject.SetActiveOptimized(false);
            setHighlightButton(null);
            return;
        }

        var _mousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect: m_rectTransform,
            screenPoint: _mousePos,
            cam: null,
            out Vector2 _localMousePos);

        if (validateMouse(_localMousePos) == false)
        {
            m_directionIndicatorTransform.gameObject.SetActiveOptimized(false);
            setHighlightButton(null);
            return;
        }

        float _mouseAngle = Vector2.Angle(Vector2.up, _localMousePos);

        if (Vector3.Dot(Vector2.right, _localMousePos) < 0)
            _mouseAngle = 360f - _mouseAngle;

        int _optionCount = m_options.Count;
        float _optionFillAngle = 360f / _optionCount;
        float _halfOptionAngle = _optionFillAngle / 2;

        m_directionIndicatorTransform.localEulerAngles = new Vector3(0, 0, -_mouseAngle);
        m_directionIndicatorTransform.gameObject.SetActiveOptimized(true);

        for (int i = 0; i < _optionCount; i++)
        {
            float _buttonAngleStart = -_halfOptionAngle + (i * _optionFillAngle);
            float _buttonAngleEnd = -_halfOptionAngle + ((i+1) * _optionFillAngle);

            if (isMouseWithinAngle(_buttonAngleStart, _buttonAngleEnd, _mouseAngle))
            {
                setHighlightButton(m_activeButtons[i]);
                break;
            }
        }
    }

    private bool isMouseWithinAngle(float angleStart, float angleEnd, float mouseAngle)
    {
        if (angleStart < 0 && angleEnd < 0)
        {
            while (angleStart < 0)
                angleStart += 360;

            while (angleEnd < 0)
                angleEnd += 360;
        }

        if (angleStart < 0 && angleEnd > 0)
        {
            while (angleStart < 0)
                angleStart += 360;

            if (mouseAngle > angleStart && mouseAngle > angleEnd)
                return true;

            if (mouseAngle < angleEnd)
                return true;

            return false;
        }

        return mouseAngle > angleStart && mouseAngle < angleEnd;
    }

    private void pollUserInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            buttonKeyPressed(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            buttonKeyPressed(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            buttonKeyPressed(2);
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            buttonKeyPressed(3);
        if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            buttonKeyPressed(4);
        if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            buttonKeyPressed(5);
        if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            buttonKeyPressed(6);
        if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            buttonKeyPressed(7);
        if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            buttonKeyPressed(8);

        if (Input.GetMouseButtonDown(0))
            StartSelectAnimations();
    }

    public bool IsAnimating()
    {
        for (int i = 0; i < m_activeButtons.Count; i++)
        {
            if (m_activeButtons[i].IsAnimating)
                return true;
        }

        return false;
    }

    private bool validateMouse(Vector2 localMousePos)
    {
        float _mouseDistance = localMousePos.magnitude;

        if (m_allowMouseInteractionOutsideRadius == false && _mouseDistance > m_rectTransform.GetWidth() / 2)
            return false;

        if (m_allowMouseInteractionInsideCenter == false && _mouseDistance < m_centerRootTransform.GetWidth() / 2)
            return false;

        return true;
    }

    private void buttonKeyPressed(int buttonIndex)
    {
        if (buttonIndex < m_activeButtons.Count)
        {
            setHighlightButton(m_activeButtons[buttonIndex]);
            StartSelectAnimations();
        }
    }

    public void StartSelectAnimations()
    {
        if (m_highlightButton == null || m_activeButtons == null || m_activeButtons.Count == 0)
            return;

        for (int i = 0; i < m_activeButtons.Count; i++)
        {
            var _button = m_activeButtons[i];
            _button.StartSelectionAnimation(_button == m_highlightButton);
        }

        OnOptionSelected?.Invoke(m_highlightButton.OptionBinding);
    }

    private void setHighlightButton(RadialMenuButton button)
    {
        if (IsAnimating())
            return;

        if (m_highlightButton == button)
            return;

        if (m_highlightButton != null)
            m_highlightButton.OnHighlightEnd();

        m_highlightButton = button;

        if (m_highlightButton != null)
        {
            m_labelText.SetText(m_highlightButton.OptionBinding.UILabel);
            m_highlightButton.OnHighlightBegin();
        }
        else
            m_labelText.SetText(string.Empty);

        if (m_highlightButton != null)
            OnHighlightOptionChanged?.Invoke(m_highlightButton.OptionBinding);
        else
            OnHighlightOptionChanged?.Invoke(null);
    }
}