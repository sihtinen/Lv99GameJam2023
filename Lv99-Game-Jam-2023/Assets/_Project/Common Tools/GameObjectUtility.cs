using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class GameObjectUtility
{
    /// <summary>
    /// Calls 'GameObject.SetActive(isActive)' only if GameObject.activeSelf is not the desired active state
    /// </summary>
    /// <param name="go"></param>
    /// <param name="isActive"></param>
    public static void SetActiveOptimized(this GameObject go, bool isActive)
    {
        if (go.activeSelf != isActive)
            go.SetActive(isActive);
    }
}
