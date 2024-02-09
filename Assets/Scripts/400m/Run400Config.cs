using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run400Config : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Track Settings")]
    public float finishX = 5;
    public float straightX1 = -5;
    public float straightX2 = 5;

    [Header("Speed Settings")]
    public float minTime = 9f;
    //public const float maxSpeed = 0.039f;
    //public const float maxSpeed = 0.015f;
    public float maxSpeed = 0.018f;
    public float homeStraightMaxSpeed = 0.022f;
    public float minSpeed = 0f;

    public float speedChange = 0.0004f;
    public float speedPenalty = 0.001f;
    public float speedDecay = 0.000025f;

    [HideInInspector]
    public float startingSpeed;
    public float maxMPS = 11f;

    public float dipSpeed = 3f;
    public float dipDuration = 0.2f;
    public float afterDipSpeed = 0.2f;

    //public const float aiMinSpeedMultiplier = 0.14f;
    //public const float aiMinSpeedMultiplier = 0.18f;
    //public float aiMinSpeedMultiplier = 0.17f;
    //public const float aiMaxSpeedMultiplier = 0.18f;
    //public float aiMaxSpeedMultiplier = 0.2f;

    //public float aiMinMaxSpeed = 0.016f;
    //public const float aiMaxMaxSpeed = 0.015f;
    //public float aiMaxMaxSpeed = maxSpeed;

    //public float aiMaxHomeStraightMaxSpeed = 0.0205f;
    //public float aiMinHomeStraightMaxSpeed = homeStraightMaxSpeed;

    //public const float anglePlus = 1f;
    //public const float anglePlus = 0.5f;
    //public const float anglePlus = 0.05f;
    public float anglePlus = 0.1f;

    [Header("AI Settings")]
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

    [Header("AI Max Speed")]
    public float aiOlympicMaxMaxSpeed = 0.1f;
    public float aiOlympicMinMaxSpeed = 0.1f;
    public float aiHardMaxMaxSpeed = 0.1f;
    public float aiHardMinMaxSpeed = 0.1f;
    public float aiMediumMaxMaxSpeed = 0.1f;
    public float aiMediumMinMaxSpeed = 0.1f;
    public float aiEasyMaxMaxSpeed = 0.1f;
    public float aiEasyMinMaxSpeed = 0.1f;

    [Header("AI Home Straight Max Speed")]
    public float aiOlympicMaxHomeStraightMaxSpeed = 0.1f;
    public float aiOlympicMinHomeStraightMaxSpeed = 0.1f;
    public float aiHardMaxHomeStraightMaxSpeed = 0.1f;
    public float aiHardMinHomeStraightMaxSpeed = 0.1f;
    public float aiMediumMaxHomeStraightMaxSpeed = 0.1f;
    public float aiMediumMinHomeStraightMaxSpeed = 0.1f;
    public float aiEasyMaxHomeStraightMaxSpeed = 0.1f;
    public float aiEasyMinHomeStraightMaxSpeed = 0.1f;

    private void Start()
    {
        startingSpeed = maxSpeed / 2.5f;
    }
}
