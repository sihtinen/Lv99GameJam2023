using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class AvailableAbilityElement : MonoBehaviour
{
    [SerializeField] private float m_spawnAnimationDuration = 1f;
    [SerializeField] private float m_spawnAnimationMaxScale = 1.2f;
    [SerializeField] private AnimationCurve m_spawnAnimationScaleCurve = new AnimationCurve();

    [Header("Object References")]
    [SerializeField] private TMP_Text m_inputText = null;
    [SerializeField] private Image m_iconImage = null;
    [Space]
    [SerializeField] private Sprite m_jumpIconSprite = null;
    [SerializeField] private Sprite m_meleeIconSprite = null;
    [SerializeField] private Sprite m_inhaleIconSprite = null;
    [SerializeField] private Sprite m_timestopIconSprite = null;

    [NonSerialized] public AbilityTypes AbilityType;

    public void Clear()
    {
        StopAllCoroutines();
        gameObject.SetActiveOptimized(false);
        transform.localScale = Vector3.one;
        AbilityCanvas.Instance.ReturnToPool(this);
    }

    public void BindToAbilityAndActivate(AbilityTypes abilityType)
    {
        AbilityType = abilityType;

        switch (AbilityType)
        {
            default:
            case AbilityTypes.Jump:
                m_iconImage.overrideSprite = m_jumpIconSprite;
                m_inputText.SetText("Space");
                break;
            case AbilityTypes.Pickaxe:
                m_iconImage.overrideSprite = m_meleeIconSprite;
                m_inputText.SetText("X");
                break;
            case AbilityTypes.Inhale:
                m_iconImage.overrideSprite = m_inhaleIconSprite;
                m_inputText.SetText("C");
                break;
            case AbilityTypes.Timestop:
                m_iconImage.overrideSprite = m_timestopIconSprite;
                m_inputText.SetText("V");
                break;
        }

        gameObject.SetActiveOptimized(true);
        StartCoroutine(coroutine_onSpawn());
    }

    private IEnumerator coroutine_onSpawn()
    {
        float _timer = 0f;

        transform.localScale = Vector3.zero;

        while (_timer < m_spawnAnimationDuration)
        {
            yield return null;
            _timer += Time.unscaledDeltaTime;
            float _animPos = _timer / m_spawnAnimationDuration;
            float _scaleCurvePos = m_spawnAnimationScaleCurve.Evaluate(_animPos);
            float _scale = _scaleCurvePos * m_spawnAnimationMaxScale;
            transform.localScale = new Vector3(_scale, _scale, 1f);
        }

        transform.localScale = Vector3.one;
    }
}
