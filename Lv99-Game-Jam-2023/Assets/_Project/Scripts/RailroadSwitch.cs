using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

public class RailroadSwitch : PuzzleBehaviour, IMeleeTarget, IInhaleTarget
{
    [Header("Switch Settings")]
    [SerializeField] private SwitchDirection m_initialDirection = SwitchDirection.Left;
    [SerializeField] private float m_maxRotateAngle = 50f;

    [Header("Object References")]
    [SerializeField] private CinemachineSmoothPath m_leftPath = null;
    [SerializeField] private CinemachineSmoothPath m_rightPath = null;
    [SerializeField] private Transform m_rotatePivot = null;
    [SerializeField] private RailroadSwitchAudioPlayer m_audioPlayer = null;

    private SwitchDirection m_currentDirection = SwitchDirection.Left;

    private void Start()
    {
        ResetPuzzleState();
    }

    public override void ResetPuzzleState()
    {
        m_currentDirection = m_initialDirection;
        updatePathStates();
    }

    private void updatePathStates()
    {
        m_leftPath.enabled = m_currentDirection == SwitchDirection.Left;
        m_rightPath.enabled = m_currentDirection == SwitchDirection.Right;
    }

    public void OnHit(Vector3 playerPosition)
    {
        switch (m_currentDirection)
        {
            case SwitchDirection.Left:
                m_currentDirection = SwitchDirection.Right;
                break;
            case SwitchDirection.Right:
                m_currentDirection = SwitchDirection.Left;
                break;
        }

        updatePathStates();

        m_audioPlayer.gameObject.SetActiveOptimized(true);
        m_audioPlayer.Play();
    }

    private void Update()
    {
        float _targetRotation = m_currentDirection == SwitchDirection.Left ? m_maxRotateAngle : -m_maxRotateAngle;
        m_rotatePivot.localRotation = Quaternion.Lerp(m_rotatePivot.localRotation, Quaternion.Euler(new Vector3(0f, 0f, _targetRotation)), GameTime.DeltaTime(TimeChannel.Player) * 5f);
    }

    public void OnInhaleHit(Vector3 playerPosition)
    {
        Vector3 _toPlayer = playerPosition - transform.position;
        float _rightDot = Vector3.Dot(_toPlayer.normalized, transform.right);
        bool _playerIsOnRightSide = _rightDot > 0.5f;
        bool _playerIsOnLeftSide = _rightDot < -0.5f;

        switch (m_currentDirection)
        {
            case SwitchDirection.Left:

                if (_playerIsOnRightSide)
                {
                    m_currentDirection = SwitchDirection.Right;
                    updatePathStates();

                    m_audioPlayer.gameObject.SetActiveOptimized(true);
                    m_audioPlayer.Play();
                }

                break;
            case SwitchDirection.Right:

                if (_playerIsOnLeftSide)
                {
                    m_currentDirection = SwitchDirection.Left;
                    updatePathStates();

                    m_audioPlayer.gameObject.SetActiveOptimized(true);
                    m_audioPlayer.Play();
                }

                break;
        }
    }

    public enum SwitchDirection
    {
        Left = 0,
        Right = 1,
    }
}
