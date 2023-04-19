using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

public class SwingingAxe : PuzzleBehaviour
{
    [SerializeField] private float m_swingSpeed = 2.0f;
    [SerializeField] private float m_maxRotationAngle = 70f;
    [SerializeField] private AnimationCurve m_swingCurve = new();
    [SerializeField, Range(0f, 2f)] private float m_initialSwingTime = 0f;
    [Space]
    [SerializeField, Range(0f, 1f)] private float m_audioPlayTime_Up = 0.3f;
    [SerializeField, Range(0f, 1f)] private float m_audioPlayTime_Down = 0.3f;

    [Header("Hit Stop Settings")]
    [SerializeField] private float m_hitStopTimeScale = 0.1f;
    [SerializeField] private float m_hitStopDuration = 0.5f;

    [Header("Object References")]
    [SerializeField] private Transform m_rotatePivot = null;
    [SerializeField] private CinemachineImpulseSource m_impulseSource = null;
    [SerializeField] private SwingingAxeAudioPlayer m_audioPlayer = null;

    private bool m_audioPlayedUp = false;
    private bool m_audioPlayedDown = false;
    private float m_currentSwingTime = 0f;

    private void Start()
    {
        ResetPuzzleState();
    }

    public override void ResetPuzzleState()
    {
        m_currentSwingTime = m_initialSwingTime;

        updateRotation();
    }

    private void Update()
    {
        m_currentSwingTime += GameTime.DeltaTime(TimeChannel.Environment) * m_swingSpeed;

        if (m_currentSwingTime > 2f)
        {
            m_currentSwingTime -= 2f;
            m_audioPlayedUp = false;
            m_audioPlayedDown = false;
        }

        updateRotation();
    }

    private void updateRotation()
    {
        float _evaluatePos = m_currentSwingTime > 1.0f ? 1.0f - (m_currentSwingTime - 1f) : m_currentSwingTime;

        float _curveVal = m_swingCurve.Evaluate(_evaluatePos);

        float _angle = Mathf.Lerp(-m_maxRotationAngle, m_maxRotationAngle, _curveVal);

        m_rotatePivot.localEulerAngles = new Vector3(0f, 0f, _angle);

        if (m_currentSwingTime < 1.0f)
        {
            if (m_audioPlayedUp == false && _curveVal >= m_audioPlayTime_Up)
            {
                m_audioPlayedUp = true;
                m_audioPlayer.SelectClipAndPlay();
            }
        }
        else
        {
            if (m_audioPlayedDown == false && (1.0f - _curveVal) >= m_audioPlayTime_Down)
            {
                m_audioPlayedDown = true;
                m_audioPlayer.SelectClipAndPlay();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter _player) == false)
            return;

        _player.TakeDamage();

        m_impulseSource.GenerateImpulseAt(_player.transform.position, UnityEngine.Random.insideUnitSphere.normalized);

        HitStop.Play(m_hitStopTimeScale, m_hitStopDuration);

        var _meditationSystem = MeditationSystem.Instance;
        if (_meditationSystem == null)
            return;

        if (_meditationSystem.ActiveMeditationPoint != null)
            _meditationSystem.ResetCurrentPuzzle();
        else
            SceneLoader.Instance?.ReloadScene();
    }
}
