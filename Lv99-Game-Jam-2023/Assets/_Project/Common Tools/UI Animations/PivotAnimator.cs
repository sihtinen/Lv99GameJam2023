using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tensori.UI.Animations
{
    public class PivotAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform m_targetRectTransform = null;
        [SerializeField] private Vector2 m_pivotStart = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 m_pivotEnd = new Vector2(0.5f, 0.5f);
        [SerializeField] private AnimationCurve m_animationCurve = new AnimationCurve();

        public void OnAnimationUpdated(float animationPos)
        {
            if (m_targetRectTransform == null)
                return;

            float _curvePos = m_animationCurve.Evaluate(animationPos);
            m_targetRectTransform.pivot = Vector2.LerpUnclamped(m_pivotStart, m_pivotEnd, _curvePos);
            m_targetRectTransform.anchoredPosition = Vector2.zero;
        }
    }
}