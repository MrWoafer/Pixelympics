using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint200Config : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Track Settings")]
    public float finishX = 5;
    public float straightX1 = -5;
    public float straightX2 = 5;

    [Header("Speed Settings")]
    public float minTime = 9f;
    public float maxSpeed = 0.019f;
    public float homeStraightMaxSpeed = 0.022f;
    public float minSpeed = 0f;

    public float speedChange = 0.0004f;
    public float speedPenalty = 0.001f;
    public float speedDecay = 0.000025f;

    [HideInInspector]
    public float startingSpeed;
    public float maxMPS = 12f;

    public float dipSpeed = 6f;
    public float dipDuration = 0.2f;
    public float afterDipSpeed = 0.5f;

    //public float aiMinSpeedMultiplier = 0.18f;
    //public float aiMaxSpeedMultiplier = 0.21f;

    //public float aiMinMaxSpeed = 0.017f;
    //public float aiMaxMaxSpeed;

    public float anglePlus = 0.1f;

    //public const float aiMaxHomeStraightMaxSpeed = 0.021f;
    //public const float aiMinHomeStraightMaxSpeed = homeStraightMaxSpeed;

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

    private void Start()
    {
        startingSpeed = maxSpeed / 2.5f;
        //aiMaxMaxSpeed = maxSpeed;
    }
}
