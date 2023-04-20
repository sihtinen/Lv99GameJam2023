using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static LogBridge;

public class LogBridgeBone : MonoBehaviour
{
    [SerializeField] private List<LogBoneHealthState> m_healthStates = new();
    [SerializeField] private Transform m_rotateAroundPivot = null;

    private Vector3 m_defaultLocalPos;
    private Quaternion m_defaultLocalRot;
    private LogBoneHealthState m_healthState = null;

    private FractureDirection m_fractureDirection = FractureDirection.None;

    private void Awake()
    {
        m_defaultLocalPos = transform.localPosition;
        m_defaultLocalRot = transform.localRotation;
    }

    public void ResetToInitialState()
    {
        transform.localPosition = m_defaultLocalPos;
        transform.localRotation = m_defaultLocalRot;
        m_fractureDirection = FractureDirection.None;
    }

    public void OnHealthUpdated(bool isCollapsed, int health, FractureDirection fractureDirection)
    {
        m_fractureDirection = fractureDirection;
        m_healthState = getHealthState(isCollapsed, health);

        if (m_healthState == null)
            return;

        if (m_rotateAroundPivot != null)
        {
            transform.localPosition = m_defaultLocalPos;
            transform.localRotation = m_defaultLocalRot;

            int _eulerZMultiplier = m_fractureDirection == FractureDirection.Left ? -1 : 1;

            transform.RotateAround(m_rotateAroundPivot.position, m_rotateAroundPivot.right, m_healthState.LocalEulerOffset.x);
            transform.RotateAround(m_rotateAroundPivot.position, m_rotateAroundPivot.up, m_healthState.LocalEulerOffset.y);
            transform.RotateAround(m_rotateAroundPivot.position, m_rotateAroundPivot.forward, _eulerZMultiplier * m_healthState.LocalEulerOffset.z);
        }

        gameObject.SetActiveOptimized(m_healthState.IsObjectActive);
    }

    private LogBoneHealthState getHealthState(bool isCollapsed, int health)
    {
        for (int i = 0; i < m_healthStates.Count; ++i)
        {
            var _state = m_healthStates[i];

            if (isCollapsed != _state.EnabledWhenCollapsed)
                continue;

            if (health >= _state.MinHealth && health <= _state.MaxHealth)
                return _state;
        }

        return null;
    }

    [System.Serializable]
    public class LogBoneHealthState
    {
        public bool EnabledWhenCollapsed = false;
        [Min(0)] public int MinHealth = 0;
        [Min(0)] public int MaxHealth = 6;
        public bool IsObjectActive = false;
        public Vector3 LocalPositionOffset;
        public Vector3 LocalEulerOffset;
    }
}
