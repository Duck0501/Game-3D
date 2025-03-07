using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin noise;
    public float shakeDuration = 0.3f; // Thời gian rung
    public float shakeAmplitude = 2f; // Cường độ rung

    void Start()
    {
        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        noise.m_AmplitudeGain = shakeAmplitude;
        yield return new WaitForSeconds(shakeDuration);
        noise.m_AmplitudeGain = 0; // Dừng rung sau thời gian nhất định
    }
}