using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IMinecartObstacle
{
    public bool IsActive();
    public CollisionResults OnCollision(Minecart minecart);

    public struct CollisionResults
    {
        public bool IsPathBlocked;
    }
}