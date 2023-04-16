using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Breath Minigame/New Breath Minigame Settings")]
public class BreathMinigameSettings : ScriptableObject
{
    public float Duration = 3.0f;
    public Vector2 CenterOffset = Vector2.zero;
    public AnimationCurve PositionXCurve = new AnimationCurve();
    public AnimationCurve PositionYCurve = new AnimationCurve();
}
