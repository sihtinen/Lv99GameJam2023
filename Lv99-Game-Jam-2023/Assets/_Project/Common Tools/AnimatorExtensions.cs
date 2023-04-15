using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class AnimatorExtensions
{
    public static bool HasState(this Animator anim, string stateName, int layer = 0)
    {
        int stateID = Animator.StringToHash(stateName);
        return anim.HasState(layer, stateID);
    }
}
