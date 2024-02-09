using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint100Config : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Line Settings")]
    public float finishX = 10;
    public float startX = -10;

    [Header("Speed Settings")]
    public float minTime = 9f;
    //public const float maxSpeed = 0.03f;
    //public const float maxSpeed = (finishX - startX) / (60 * (minTime-1f));
    //public const float maxSpeed = 0.045f;
    public float maxSpeed = 0.044f;
    public float minSpeed = 0f;

    public float speedChange = 0.00057f;
    //public const float speedPenalty = 0.001f;
    //public const float speedPenalty = 0.005f;
    public float speedPenalty = 0.001f;
    public float speedDecay = 0.00002f;

    public float startingSpeed;
    public float maxMPS = 12f;

    public float dipSpeed = 6f;
    //public const float dipDuration = 0.2f;
    public float dipDuration = 0.3f;
    //public const float dipDuration = 1f;
    public float afterDipSpeed = 0.5f;

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
    }

    private void Update()
    {
        
    }
}