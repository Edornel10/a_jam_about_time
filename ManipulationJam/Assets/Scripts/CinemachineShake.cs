using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour
{
    private CinemachineVirtualCamera vc;
    CinemachineBasicMultiChannelPerlin mcp;

    private float timer;

    private void Awake()
    {
        vc = GetComponent<CinemachineVirtualCamera>();
        mcp = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0f)
            {
                mcp.m_AmplitudeGain = 0f;
            }
        }
    }

    public void Shake(float duration, float magnitude, float frequency)
    {
        
        mcp.m_AmplitudeGain = magnitude;
        mcp.m_FrequencyGain = frequency;
        timer = duration;
    }
}
