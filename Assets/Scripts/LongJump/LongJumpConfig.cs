using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJumpConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Track Settings")]
    public float lineX = 10f;

    [Header("Speed Settings")]
    public float maxSpeed = 0.08f;
    public float minSpeed = 0f;

    public float speedChange = 0.001f;
    public float speedPenalty = 0.001f;
    public float speedDecay = 0.000025f;

    //[HideInInspector]
    public float startingSpeed;
    public float maxMPS = 10f;

    //public float dipDuration = 0.2f;

    [Header("AI Settings")]
    public float aiMinSpeedMultiplier = 0.08f;
    public float aiMaxSpeedMultiplier = 0.1f;

    public float aiMinMaxSpeed = 0.07f;
    //public const float aiMaxMaxSpeed = 0.079f;
    public float aiMaxMaxSpeed = 0.0797f;

    //public const float aiMinDrawX = 3f;
    public float aiMinDrawX = 9.5f;
    public float aiMaxDrawX = 10.2f;
    //public const float aiMinAngle = 30f;
    //public const float aiMinAngle = 35f;
    public float aiMinAngle = 40f;
    public float aiMaxAngle = 50f;

    //public float anglePlus = 0.1f;
    [Header("Needle Settings")]
    //public const float needleRotationSpeed = 50f;
    public float needleRotationSpeed = 100f;

    [Header("Physics Settings")]
    //public const float releaseSpeedMultiplier = 40f;
    public float releaseSpeedMultiplierU = 100f;
    //public const float releaseSpeedMultiplierR = 200f;
    //public const float releaseSpeedMultiplierR = 190f;
    //public const float releaseSpeedMultiplierR = 160f;
    public float releaseSpeedMultiplierR = 140f;
    public float gravity = 9.81f;

    //public const float slowMo = 3f;
    //public const float slowMo = 2f;
    //public const float slowMo = 0.5f;
    public float slowMo = 1f;

    private void Start()
    {
        //startingSpeed = maxSpeed / 2.5f;
    }
}
