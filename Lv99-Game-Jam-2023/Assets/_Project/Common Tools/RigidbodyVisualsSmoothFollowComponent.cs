using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RigidbodyVisualsSmoothFollowComponent : MonoBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody = null;

    private Vector3 m_oldPosition, m_newPosition;
    private Vector3 m_localPositionOffset;
    private Quaternion m_oldRotation, m_newRotation;

    private float m_betweenFixedUpdateTimer = 0f;

    private Transform m_transform = null;
    private Transform m_rigidTransform = null;

    private void Start()
    {
        m_transform = transform;
        m_rigidTransform = m_rigidbody.transform;

        m_oldPosition = m_newPosition = m_rigidbody.position;
        m_oldRotation = m_newRotation = m_rigidbody.rotation;
        m_localPositionOffset = m_rigidTransform.InverseTransformPoint(m_transform.position);
        m_transform.SetParent(null);
    }

    private void FixedUpdate()
    {
        m_oldPosition = m_newPosition;
        m_oldRotation = m_newRotation;
        m_newPosition = m_rigidbody.position;
        m_newRotation = m_rigidbody.rotation;
        m_betweenFixedUpdateTimer = 0f;
    }

    private void LateUpdate()
    {
        if (m_rigidbody == null)
        {
            Destroy(gameObject);
            return;
        }

        if (m_rigidbody.isKinematic == false)
        {
            m_transform.SetPositionAndRotation(
                m_rigidTransform.position + m_rigidTransform.TransformVector(m_localPositionOffset),
                m_rigidTransform.rotation);

            return;
        }

        m_betweenFixedUpdateTimer += Time.smoothDeltaTime;
        float _lerpPos = m_betweenFixedUpdateTimer / Time.fixedDeltaTime;

        Vector3 _targetPos = Vector3.LerpUnclamped(m_oldPosition, m_newPosition, _lerpPos) + m_rigidTransform.TransformVector(m_localPositionOffset);
        Quaternion _targetRot = Quaternion.LerpUnclamped(m_oldRotation, m_newRotation, _lerpPos);

        _targetPos = Vector3.Lerp(m_transform.position, _targetPos, 0.5f);
        _targetRot = Quaternion.Lerp(m_transform.rotation, _targetRot, 0.5f);

        m_transform.SetPositionAndRotation(_targetPos, _targetRot);
    }
}
