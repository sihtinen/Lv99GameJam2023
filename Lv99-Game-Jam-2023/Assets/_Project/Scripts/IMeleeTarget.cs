using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IMeleeTarget
{
    public MeleeHitParticlesType HitParticlesType { get; }
    public Transform RootTransform { get; }
    public void OnHit(Vector3 playerPosition);
}

public enum MeleeHitParticlesType
{
    None = 0,
    Metal = 1,
}