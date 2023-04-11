using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Tensori.UI.Animations
{
    public class ImageColorAnimator : MonoBehaviour
    {
        [SerializeField] private Color m_colorMin = Color.white;
        [SerializeField] private Color m_colorMax = Color.white;
        [SerializeField] private AnimationCurve m_animationCurve = new AnimationCurve();
        [SerializeField] private Image m_imageComponent = null;

        public void UpdateAnimation(float animationPos)
        {
            m_imageComponent.color = Color.Lerp(
                m_colorMin,
                m_colorMax,
                m_animationCurve.Evaluate(animationPos));
        }
    }
}