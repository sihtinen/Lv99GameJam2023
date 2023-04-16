using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Breath Minigame/New Breath Minigame Collection")]
public class BreathMinigameCollection : ScriptableObject
{
    public List<BreathMinigameSettings> AvailableMinigames = new List<BreathMinigameSettings>();
}