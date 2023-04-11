using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Tensori.UI.Animations
{
    public class SizeDeltaAnimator : MonoBehaviour
    {
        [SerializeField] private bool m_animateWidth = false;
        [SerializeField] private bool m_animateHeight = false;

        [SerializeField] private Vector2 m_sizeStart = Vector2.one;
        [SerializeField] private Vector2 m_sizeEnd = Vector2.one;
        [SerializeField] private AnimationCurve m_animationCurve = new AnimationCurve();
        [SerializeField] private RectTransform m_targetRectTransform = null;

        public void OnAnimationUpdated(float animationPos)
        {
            if (m_targetRectTransform == null)
                return;

            float _curvePos = m_animationCurve.Evaluate(animationPos);
            Vector2 _currentSize = Vector2.LerpUnclamped(m_sizeStart, m_sizeEnd, _curvePos);

            if (m_animateWidth)
                m_targetRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _currentSize.x);

            if (m_animateHeight)
                m_targetRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _currentSize.y);
        }
    }
}
