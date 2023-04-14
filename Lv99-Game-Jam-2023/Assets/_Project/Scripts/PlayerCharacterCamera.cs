using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(101)]
public class PlayerCharacterCamera : SingletonBehaviour<PlayerCharacterCamera>
{
    [Header("Settings")]
    [SerializeField] private Camera m_mainCamera = null;
    [SerializeField] private AnimationCurve m_positionCurve = new AnimationCurve();
    [SerializeField] private AnimationCurve m_rotationCurve = new AnimationCurve();

    private float m_cameraBlend = 0f;

    private Camera m_camera = null;
    private MeditationPoint m_currentMeditationPoint = null;
    private Coroutine m_currentAnimCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        TryGetComponent(out m_camera);

        transform.SetParent(null);
    }

    private void OnDestroy()
    {
        m_camera = null;
        m_currentMeditationPoint = null;
    }

    private void LateUpdate()
    {
        if (m_currentMeditationPoint != null)
        {
            transform.SetPositionAndRotation(
                Vector3.Lerp(m_mainCamera.transform.position, m_currentMeditationPoint.GetCameraTargetPosition(), m_positionCurve.Evaluate(m_cameraBlend)),
                Quaternion.Lerp(m_mainCamera.transform.rotation, m_currentMeditationPoint.GetCameraTargetRotation(), m_rotationCurve.Evaluate(m_cameraBlend)));
        }
    }

    public void EnableMeditationCamera(MeditationPoint point)
    {
        m_currentMeditationPoint = point;
        transform.SetPositionAndRotation(m_mainCamera.transform.position, m_mainCamera.transform.rotation);
        m_camera.fieldOfView = m_mainCamera.fieldOfView;


        if (m_currentAnimCoroutine != null)
            StopCoroutine(m_currentAnimCoroutine);

        m_currentAnimCoroutine = StartCoroutine(coroutine_enableCamera());
    }

    public void DisableMeditationCamera()
    {
        if (m_currentAnimCoroutine != null)
            StopCoroutine(m_currentAnimCoroutine);

        m_currentAnimCoroutine = StartCoroutine(coroutine_disableCamera());
    }

    private IEnumerator coroutine_enableCamera()
    {
        while (m_cameraBlend < 1f)
        {
            yield return null;
            m_cameraBlend += Time.deltaTime;
        }

        m_cameraBlend = 1f;
        m_currentAnimCoroutine = null;
    }

    private IEnumerator coroutine_disableCamera()
    {
        while (m_cameraBlend > 0f)
        {
            yield return null;
            m_cameraBlend -= Time.deltaTime;
        }

        m_cameraBlend = 0f;
        m_currentAnimCoroutine = null;
        m_currentMeditationPoint = null;
    }

    public bool IsAnimating() => m_currentAnimCoroutine != null;
}