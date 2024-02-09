using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJumpConfig : MonoBehaviour
{
    [Header("Player Settings")]
    public float speed = 1f;
    public float releaseSpeedMultiplierU = 100f;
    public float releaseSpeedMultiplierR = 140f;
    public float needleRotationSpeed = 100f;
    public float defaultAngle = 45f;

    [Header("AI Settings")]
    public float aiStartTime = 1f;

    [Header("Hard AI Settings")]
    public float aiBoardXMinHard = 0.01f;
    public float aiBoardXMaxHard = 0.1f;
    public float aiAngleMinHard = 42f;
    public float aiAngleMaxHard = 48f;
    public float aiJumpHeightMinHard = 0.01f;
    public float aiJumpHeightMaxHard = 0.1f;

    [Header("Medium AI Settings")]
    public float aiBoardXMinMedium = 0.01f;
    public float aiBoardXMaxMedium = 0.1f;
    public float aiAngleMinMedium = 42f;
    public float aiAngleMaxMedium = 48f;
    public float aiJumpHeightMinMedium = 0.01f;
    public float aiJumpHeightMaxMedium = 0.1f;

    [Header("Easy AI Settings")]
    public float aiBoardXMinEasy = 0.01f;
    public float aiBoardXMaxEasy = 0.1f;
    public float aiAngleMinEasy = 42f;
    public float aiAngleMaxEasy = 48f;
    public float aiJumpHeightMinEasy = 0.01f;
    public float aiJumpHeightMaxEasy = 0.1f;


    [Header("Track Settings")]
    public float lineX = 0f;
    public float sandX = 0f;

    [Header("Misc Settings")]
    public float gravity = 9.81f;
    public float slowMo = 1f;
    public float normalY = -2.5f;
    public float circleMaxScale = 2f;

    [Header("Camera Settings")]
    public float cameraMinX = -17f;
    public float cameraMaxX = 1000f;
    public float cameraMinY = -1f;
    public float cameraMaxY = 1000f;
}
