using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Radial Menu/New Button Settings")]
public class RadialMenuButtonSettings : ScriptableObject
{
    public Color NormalColor = Color.white;
    public Color HighlightColor = Color.white;
    public Color SelectedColor = Color.white;
    public Color NotSelectedColor = Color.white;

    [Min(0)] public float TextDistanceFromCenter = 0.5f;

    [Header("Animation Settings")]
    public float SelectionAnimationLength = 0.2f;
    public AnimationCurve SelectedColorCurve = new AnimationCurve();
    public AnimationCurve NotSelectedColorCurve = new AnimationCurve();
}
