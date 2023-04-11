using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IRadialMenuOption
{
    public string UILabel { get; }
    public GUIDWrapper GUID { get; }
    public void OnSelected();
}
