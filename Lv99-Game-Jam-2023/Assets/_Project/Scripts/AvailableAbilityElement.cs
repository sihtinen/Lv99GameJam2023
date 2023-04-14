using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class AvailableAbilityElement : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TMP_Text m_nameText = null;

    public void Clear()
    {
        gameObject.SetActiveOptimized(false);
        AbilityCanvas.Instance.ReturnToPool(this);
    }
}
