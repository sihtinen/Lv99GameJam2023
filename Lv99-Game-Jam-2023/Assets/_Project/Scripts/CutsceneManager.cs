using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : SingletonBehaviour<CutsceneManager>
{
    [Header("Object References")]
    [SerializeField] private CutsceneTriggerVolume m_startTriggerVolume = null;
    [SerializeField] private PlayableDirector m_playableDirector = null;
    [SerializeField] private List<CinemachineSmoothPath> m_movementPaths = new();
    [SerializeField] private List<PopupScreen> m_popupScreens = new();

    [NonSerialized] public bool IsCutsceneActive = false;

    private float m_currentPathPos;
    private CinemachineSmoothPath m_currentMovementPath = null;
    private PlayerMoveComponent m_playerMoveComponent = null;
    private PopupScreen m_currentPopupScreen = null;

    protected override void Awake()
    {
        base.Awake();

        if (m_startTriggerVolume != null)
            m_startTriggerVolume.OnPlayerEnteredVolume.AddListener(this.onPlayerEnteredCutsceneStart);

        if (m_playableDirector != null)
            m_playableDirector.gameObject.SetActiveOptimized(false);
    }

    private void onPlayerEnteredCutsceneStart()
    {
        IsCutsceneActive = true;
        m_playerMoveComponent = PlayerMoveComponent.Instance;

        m_playableDirector.gameObject.SetActiveOptimized(true);
        m_playableDirector.Play();
    }

    public void MovePath(int index)
    {
        m_currentMovementPath = m_movementPaths[index];
        m_currentPathPos = m_currentMovementPath.FindClosestPoint(m_playerMoveComponent.transform.position, startSegment: 0, searchRadius: -1, stepsPerSegment: 32);
        m_currentPathPos = m_currentMovementPath.FromPathNativeUnits(m_currentPathPos, CinemachinePathBase.PositionUnits.Distance);
    }

    public void ActivatePopup(int index)
    {
        m_playableDirector.Pause();

        m_currentPopupScreen = m_popupScreens[index];
        m_currentPopupScreen.Activate();
    }

    private void Update()
    {
        if (IsCutsceneActive == false)
            return;

        updateCurrentMovementPath();
        updatePopupScreen();

        if (m_playableDirector.time >= m_playableDirector.duration)
        {
            m_playableDirector.gameObject.SetActiveOptimized(false);
            m_startTriggerVolume.gameObject.SetActiveOptimized(false);
            IsCutsceneActive = false;
        }
    }

    private void updateCurrentMovementPath()
    {
        if (m_currentMovementPath == null)
            return;

        m_currentPathPos += GameTime.DeltaTime(TimeChannel.Player) * m_playerMoveComponent.MaxMovementSpeed * 0.8f;

        Vector3 _targetPos = m_currentMovementPath.EvaluatePositionAtUnit(m_currentPathPos, CinemachinePathBase.PositionUnits.Distance);
        Vector3 _toTargetPos = _targetPos - m_playerMoveComponent.transform.position;

        var _mainCam = MainCameraComponent.Instance;
        Vector3 _rightDirectionProjection = Vector3.Project(_toTargetPos, _mainCam.HorizontalRightDirection);
        Vector3 _forwardDirectionProjection = Vector3.Project(_toTargetPos, _mainCam.HorizontalForwardDirection);

        float _inputX = _rightDirectionProjection.magnitude;
        if (Vector3.Dot(_mainCam.HorizontalRightDirection, _toTargetPos.normalized) < 0f)
            _inputX *= -1;

        float _inputY = _forwardDirectionProjection.magnitude;
        if (Vector3.Dot(_mainCam.HorizontalForwardDirection, _toTargetPos.normalized) < 0f)
            _inputY *= -1;

        m_playerMoveComponent.CutsceneInput = new Vector2(_inputX, _inputY);

        float _remainingPathDistance = Vector3.Distance(
            m_playerMoveComponent.transform.position,
            m_currentMovementPath.EvaluatePositionAtUnit(m_currentMovementPath.PathLength, CinemachinePathBase.PositionUnits.Distance));

        if (_remainingPathDistance < 0.1f)
        {
            m_currentMovementPath = null;
            m_playerMoveComponent.CutsceneInput = Vector2.zero;
        }
    }

    private void updatePopupScreen()
    {
        if (m_currentPopupScreen == null)
            return;

        if (m_currentPopupScreen.IsActive)
            return;

        m_playableDirector.Resume();

        m_currentPopupScreen = null;
    }
}
