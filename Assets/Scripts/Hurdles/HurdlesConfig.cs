using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdlesConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Track Settings")]
    public float finishX = 11;
    public float startX = -11;

    public float[] hurdleXs = { -8f, -4f, 0f, 4f, 8f, 100f };

    [Header("Speed Settings")]
    public float minTime = 9f;
    //public const float maxSpeed = 0.044f;
    public float maxSpeed = 0.039f;
    public float minSpeed = 0f;

    public float speedChange = 0.00057f;
    public float speedPenalty = 0.001f;
    public float speedDecay = 0.00002f;

    [HideInInspector]
    public float startingSpeed;
    //public const float maxMPS = 12f;
    public float maxMPS = 10f;

    public float dipSpeed = 6f;
    public float dipDuration = 0.3f;
    public float afterDipSpeed = 0.5f;

    //public const float hitPenalty = 0.005f;
    public float hitPenalty = 0.01f;
    //public const float jumpLength = 1f;
    public float jumpLength = 0.7f;

    [Header("AI Settings")]

    //public const float aiMinJumpDist = 0f;
    [Tooltip("Easy, Medium, Hard, Olympic")]
    public float[] aiMinJumpDist = { 0.3f, 0.35f, 0.4f, 0.4f };
    //public const float aiMaxJumpDist = 0.3f;
    //public const float aiMaxJumpDist = 0.65f;
    public float[] aiMaxJumpDist = { 0.8f, 0.8f, 0.8f, 0.8f };

    //public float aiMinSpeedMultiplier = 0.16f;
    //public const float aiMinSpeedMultiplier = 0.19f;
    //public float aiMaxSpeedMultiplier = 0.2f;
    //    public const float aiMaxSpeedMultiplier = 0.23f;

    //public const float aiMinMaxSpeed = 0.034f;
    //public float aiMinMaxSpeed = 0.037f;
    //public float aiMaxMaxSpeed = 0.039f;
    
    [Header("AI Wait")]
    public float aiOlympicMaxWait = 0.1f;
    public float aiOlympicMinWait = 0.1f;
    public float aiHardMaxWait = 0.1f;
    public float aiHardMinWait = 0.1f;
    public float aiMediumMaxWait = 0.1f;
    public float aiMediumMinWait = 0.1f;
    public float aiEasyMaxWait = 0.1f;
    public float aiEasyMinWait = 0.1f;

    [Header("AI Start")]
    public float aiOlympicMaxStart = 0.1f;
    public float aiOlympicMinStart = 0.1f;
    public float aiHardMaxStart = 0.1f;
    public float aiHardMinStart = 0.1f;
    public float aiMediumMaxStart = 0.1f;
    public float aiMediumMinStart = 0.1f;
    public float aiEasyMaxStart = 0.1f;
    public float aiEasyMinStart = 0.1f;

    private void Start()
    {
        startingSpeed = maxSpeed / 2.5f;
    }
}
