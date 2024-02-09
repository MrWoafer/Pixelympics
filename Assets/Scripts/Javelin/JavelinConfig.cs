using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Track Settings")]
    public float lineX = 10;
    
    [Header("Speed Settings")]
    public float maxSpeed = 0.08f;
    public float minSpeed = 0f;

    public float speedChange = 0.001f;
    //public float speedPenalty = 0.001f;
    //public float speedDecay = 0.000025f;

    public float startingSpeed;
    public float maxMPS = 10f;

    //public float dipDuration = 0.2f;

    //public float aiMinSpeedMultiplier = 0.08f;
    //public float aiMaxSpeedMultiplier = 0.1f;

    //public float aiMinMaxSpeed = 0.07f;
    //public float aiMaxMaxSpeed = maxSpeed;

    [Header("AI Settings")]
    //public const float aiMinDrawX = 3f;
    public float aiMinDrawX = 5f;
    public float aiMaxDrawX = 6f;
    public float aiMinAngle = 30f;
    public float aiMaxAngle = 50f;

    //public float anglePlus = 0.1f;

    [Header("Arm Settings")]
    public float armRotationSpeed = 50f;
    public float armRotationSpeed2 = 1000f;

    [Header("Physics Settings")]
    public float releaseSpeedMultiplier = 400f;
    public float gravity = 9.81f;

    private void Start()
    {
        //startingSpeed = maxSpeed / 2.5f;
    }
}
