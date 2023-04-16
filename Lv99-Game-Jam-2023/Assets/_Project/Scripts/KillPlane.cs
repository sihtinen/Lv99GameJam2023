using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class KillPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player) == false)
            return;

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem == null)
            return;

        if (_meditationSystem.PreviousMeditationPoint != null)
            _meditationSystem.ResetCurrentPuzzle();
        else
            SceneLoader.Instance?.ReloadScene();
    }
}
