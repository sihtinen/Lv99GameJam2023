using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class CutsceneSignalListener : MonoBehaviour
{
    public void MovePath(int pathIndex)
    {
        CutsceneManager.Instance?.MovePath(pathIndex);
    }

    public void ActivatePopup(int popupIndex)
    {
        CutsceneManager.Instance?.ActivatePopup(popupIndex);
    }
}
