using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class AvailableAbilityElement : MonoBehaviour
{
    [SerializeField] private float m_spawnAnimationDuration = 1f;
    [SerializeField] private float m_spawnAnimationMaxScale = 1.2f;
    [SerializeField] private AnimationCurve m_spawnAnimationScaleCurve = new AnimationCurve();

    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;

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
        m_nameText.SetText(abilityType.ToString());
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
