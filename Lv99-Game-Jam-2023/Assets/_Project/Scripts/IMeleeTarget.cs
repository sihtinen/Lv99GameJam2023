using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IMeleeTarget
{
    public void OnHit(Vector3 playerPosition);
}