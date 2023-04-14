using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AvailableAbilitySlotElement : MonoBehaviour
{
    [NonSerialized] public AvailableAbilityElement AbilityElementBinding = null;

    public void Clear()
    {
        gameObject.SetActiveOptimized(false);

        if (AbilityElementBinding != null)
        {
            AbilityElementBinding.Clear();
            AbilityElementBinding = null;
        }

        AbilityCanvas.Instance.ReturnToPool(this);
    }
}
