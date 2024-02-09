using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Player Settings")]
    //public bool doesThisVariableDoAnything = true;
    public bool thisVariableEqualsTrue = true;
    public float jumpForce = 10f;
    public float startPushForce = 4f;

    [Header("AI Settings")]
    public float aiStartTime = 1f;
    public float aiAfterPushWaitTime = 1f;
    public float aiAirAngleTolerance = 10f;
    public float aiAngularVelocityTolerance = 5f;
    [Header("AI Ramp Angle")]
    public float aiOlympicMaxRampAngle = -45f;
    public float aiOlympicMinRampAngle = -45f;
    public float aiHardMaxRampAngle = -45f;
    public float aiHardMinRampAngle = -45f;
    public float aiMediumMaxRampAngle = -45f;
    public float aiMediumMinRampAngle = -45f;
    public float aiEasyMaxRampAngle = -45f;
    public float aiEasyMinRampAngle = -45f;
    [Header("AI Air Angle")]
    public float aiOlympicMaxAirAngle = 10f;
    public float aiOlympicMinAirAngle = 0f;
    public float aiHardMaxAirAngle = 10f;
    public float aiHardMinAirAngle = 0f;
    public float aiMediumMaxAirAngle = 10f;
    public float aiMediumMinAirAngle = 0f;
    public float aiEasyMaxAirAngle = 10f;
    public float aiEasyMinAirAngle = 0f;

    [Header("Ski Front Settings")]
    [Tooltip("Forward Ground Torque Ski Front")]
    public float forwardGroundTorqueSkiFront = 1f;
    [Tooltip("Backward Ground Torque Ski Front")]
    public float backwardGroundTorqueSkiFront = 1f;
    public float forwardAirTorque = 1f;

    [Header("Ski Back Settings")]
    [Tooltip("Forward Ground Torque Ski Back")]
    public float forwardGroundTorqueSkiBack = 1f;
    [Tooltip("Backward Ground Torque Ski Back")]
    public float backwardGroundTorqueSkiBack = 1f;
    public float backwardAirTorque = 1f;
    //public float forwardGroundVelocity = 1f;
    //public float backwardGroundVelocity = 1f;
    //public float forwardAirVelocity = 1f;
    //public float backwardAirVelocity = 1f;

    [Header("Wind Settings")]
    public float windTorque = 1f;
    //public float windVelocity = 1f;
    public float offRampWindTorque = 1f;

    [Header("Particle Settings")]
    public float snowParticlesAngle = 30f;

    [Header("Ramp Settings")]
    public Transform rampStart;
    public float rampStartX
    {
        get
        {
            return rampStart.position.x;
        }
    }
    public Transform rampEnd;
    public float rampEndX
    {
        get
        {
            return rampEnd.position.x;
        }
    }
    public Transform rampCurveStart;
    public float rampCurveStartX
    {
        get
        {
            return rampCurveStart.position.x;
        }
    }
    public Transform hillEnd;
    public float hillEndX
    {
        get
        {
            return hillEnd.position.x;
        }
    }
    public Transform finish;
    public float finishX
    {
        get
        {
            return finish.position.x;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rotation">The rotation of the player in degrees.</param>
    /// <returns></returns>
    public float GetDrag(float rotation)
    {
        return Mathf.Abs(rotation) / 360f;
    }
}
