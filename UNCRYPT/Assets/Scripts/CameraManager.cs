using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : SceneSingleton<CameraManager>
{
    [SerializeField] private CinemachineCamera mainCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin noiseController;
    [SerializeField] private CinemachineCamera dialogueCamera;
    [SerializeField] private CinemachineCamera nearPlayerCamera;
    [SerializeField] private CinemachineCamera hubCamera;
    [SerializeField] private EventSystem eventSystem;
    
    [Header("Small Shake")]
    [SerializeField] private float smallShakeDuration = 0.1f;
    [SerializeField] private float smallShakeIntensity = 2f;
    
    [Header("Big Shake")]
    [SerializeField] private float bigShakeDuration = 0.2f;
    [SerializeField] private float bigShakeIntensity = 5f;

    private float _shakeTimer;
    private float _startTimer;

    private Coroutine _shakeCoroutine;
    
    protected override void OnAwake()
    {
        dialogueCamera.gameObject.SetActive(false);
        nearPlayerCamera.gameObject.SetActive(false);
    }

    public void FollowPlayer()
    {
        dialogueCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        SetTarget(mainCamera, GameManager.Instance.PlayerTransform);
    }
    
    public void ZoomToPlayer()
    {
        mainCamera.gameObject.SetActive(false);
        nearPlayerCamera.gameObject.SetActive(true);
        SetTarget(nearPlayerCamera, GameManager.Instance.PlayerTransform);
    }

    public void DialogueView(Transform characterTransform)
    {
        mainCamera.gameObject.SetActive(false);
        dialogueCamera.gameObject.SetActive(true);
        SetTarget(dialogueCamera, characterTransform);
    }

    public void HubView()
    {
        mainCamera.gameObject.SetActive(false);
        hubCamera.gameObject.SetActive(true);
    }

    private void SetTarget(CinemachineCamera camera, Transform target)
    {
        camera.Follow = target;
        camera.LookAt = target;
    }

    private void Shake(float intensity, float duration, bool instant)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        if (instant)
        {
            StartCoroutine(InstantShake(intensity, duration));
        }
        else
        {
            _shakeCoroutine = StartCoroutine(SmoothShake(intensity, duration));
        }
    }

    private IEnumerator SmoothShake(float intensity, float duration)
    {
        float timer = duration;
        noiseController.AmplitudeGain = intensity;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            noiseController.AmplitudeGain = Mathf.Lerp(intensity, 0, 1 - (timer / duration));
            yield return null;
        }

        noiseController.AmplitudeGain = 0;
    }

    private IEnumerator InstantShake(float intensity, float duration)
    {
        noiseController.AmplitudeGain = intensity;
        yield return new WaitForSeconds(duration);
        noiseController.AmplitudeGain = 0;
    }

    public void BigShake()
    {
        Shake(bigShakeIntensity, bigShakeDuration, false); 
    }

    public void SmallShake(float intensityMultiplier = 1)
    {
        Shake(smallShakeIntensity * intensityMultiplier, smallShakeDuration, true); // Instant on/off for small shake
    }
    
    public EventSystem EventSystem => eventSystem;
}