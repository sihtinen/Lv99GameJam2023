using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerCameraFollowTarget : MonoBehaviour
{
    [SerializeField] private PlayerMoveComponent m_moveComponent = null;

    private void Awake()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        Vector3 _movePos = m_moveComponent.transform.position;

        if (m_moveComponent.IsGrounded == false && _movePos.y > transform.position.y)
            _movePos.y = transform.position.y;

        transform.position = _movePos;
    }
}
