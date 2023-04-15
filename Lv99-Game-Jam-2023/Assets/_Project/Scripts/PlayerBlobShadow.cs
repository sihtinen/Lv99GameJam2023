using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(1)]
public class PlayerBlobShadow : MonoBehaviour
{
    [SerializeField] private float m_maxDistanceSize = 2f;
    [SerializeField] private float m_sizeClose = 2f;
    [SerializeField] private float m_sizeFar = 0.7f;

    [SerializeField] private float m_maxDistanceOpacity = 1.0f;
    [SerializeField] private float m_opacityClose = 0f;
    [SerializeField] private float m_opacityFar = 1f;

    [Header("Object References")]
    [SerializeField] private PlayerMoveComponent m_moveComponent = null;

    private DecalProjector m_decalProjector = null;

    private void Awake()
    {
        TryGetComponent(out m_decalProjector);

        transform.SetParent(null);
    }

    private void Update()
    {
        if (m_moveComponent.IsGrounded)
        {
            transform.position = m_moveComponent.transform.position + new Vector3(0f, 0.2f, 0f);
            m_decalProjector.enabled = false;
            return;
        }

        Vector3 _movePos = m_moveComponent.transform.position + new Vector3(0f, 0.2f, 0f);

        if (m_moveComponent.IsGrounded == false && _movePos.y > transform.position.y)
            _movePos.y = transform.position.y;

        transform.position = _movePos;

        float _distance = m_moveComponent.transform.position.y - transform.position.y;
        updateProjectorSize(_distance);
        updateProjectorOpacity(_distance);

        m_decalProjector.enabled = true;
    }

    private void updateProjectorSize(float distance)
    {
        float _distanceNormalized = distance / m_maxDistanceSize;
        float _size = Mathf.Lerp(m_sizeClose, m_sizeFar, _distanceNormalized);
        m_decalProjector.size = new Vector3(_size, _size, 30f);
    }

    private void updateProjectorOpacity(float distance)
    {
        float _distanceNormalized = distance / m_maxDistanceOpacity;
        m_decalProjector.fadeFactor = Mathf.Lerp(m_opacityClose, m_opacityFar, _distanceNormalized);
    }
}
