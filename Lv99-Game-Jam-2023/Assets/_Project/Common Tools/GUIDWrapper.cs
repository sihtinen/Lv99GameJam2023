using System;
using System.Numerics;
using System.Text;

using UnityEngine;

[Serializable]
public class GUIDWrapper
{
    [SerializeField, NonEditable]
    private string m_guid = Guid.NewGuid().ToString("N");

    public override string ToString() => m_guid;
}
