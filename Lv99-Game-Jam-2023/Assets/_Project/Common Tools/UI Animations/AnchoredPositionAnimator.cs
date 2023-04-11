using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tensori.UI.Animations
{
    public class AnchoredPositionAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform m_targetRectTransform = null;
        [SerializeField] private Vector2 m_positionStart = Vector2.zero;
        [SerializeField] private Vector2 m_positionEnd = Vector2.zero;
        [SerializeField] private AnimationCurve m_animationCurve = new AnimationCurve();

        public void OnAnimationUpdated(float animationPos)
        {
            if (m_targetRectTransform == null)
                return;

            float _curvePos = m_animationCurve.Evaluate(animationPos);
            m_targetRectTransform.anchoredPosition = Vector2.LerpUnclamped(m_positionStart, m_positionEnd, _curvePos);
        }
    }
}