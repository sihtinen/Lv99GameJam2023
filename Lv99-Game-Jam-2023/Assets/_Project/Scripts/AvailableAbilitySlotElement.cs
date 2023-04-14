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

    public void BindToAbilityElement(AvailableAbilityElement abilityElement)
    {
        AbilityElementBinding = abilityElement;
        AbilityElementBinding.transform.SetParent(this.transform);
        AbilityElementBinding.gameObject.SetActiveOptimized(true);

        RectTransform _rectTransform = AbilityElementBinding.transform as RectTransform;
        _rectTransform.SetLeft(20);
        _rectTransform.SetTop(20);
        _rectTransform.SetRight(20);
        _rectTransform.SetBottom(20);
    }
}
