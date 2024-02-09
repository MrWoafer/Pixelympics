using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJumpConfig : MonoBehaviour
{
    [Header("Path Settings")]
    public float curveCoefficient = 1f;
    public float jumpX = 19f;
    public float startOffset = 5f;

    [Header("Player Settings")]
    public float startingSpeed = 0.1f;
    public float maxButtonCountdown = 1f;
    public float maxSpeedGain = 1f;
    public float rotationSpeed = 250f;

    [Header("Bar Settings")]
    public int defaultHeight = 120;

    [Header("Jump Settings")]
    public float gravity = 9.81f;
    public float horizontalSpeed = 0.5f;
    public float verticalSpeedScalar = 2f;
    public float slowMo = 2f;
    public float archHeightOffset = 0.5f;

    [Header("AI Settings")]
    public float aiStartTime = 1f;

    [Header("AI Hard Settings")]
    public float aiReactTimeMaxHard = 0.07f;
    public float aiReactTimeMinHard = 0f;
    public float aiMinTHard = 0.05f;
    public float aiMaxTHard = 0.2f;
    public float aiMinAngleHard = 84f;
    public float aiMaxAngleHard = 87f;

    [Header("AI Medium Settings")]
    public float aiReactTimeMaxMedium = 0.07f;
    public float aiReactTimeMinMedium = 0f;
    public float aiMinTMedium = 0.05f;
    public float aiMaxTMedium = 0.2f;
    public float aiMinAngleMedium = 84f;
    public float aiMaxAngleMedium = 87f;

    [Header("AI Easy Settings")]
    public float aiReactTimeMaxEasy = 0.07f;
    public float aiReactTimeMinEasy = 0f;
    public float aiMinTEasy = 0.05f;
    public float aiMaxTEasy = 0.2f;
    public float aiMinAngleEasy = 84f;
    public float aiMaxAngleEasy = 87f;
}
