using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IMinecartObstacle
{
    public bool IsActive();
    public bool IsStationary();
    public Vector3 Position { get; }
    public void Collision(Minecart minecart);
}