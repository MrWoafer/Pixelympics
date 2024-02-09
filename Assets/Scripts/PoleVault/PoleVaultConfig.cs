using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleVaultConfig : MonoBehaviour
{
    //[Header("Path Settings")]

    [Header("Player Settings")]
    public float startingSpeed = 0.1f;
    public float maxButtonCountdown = 1f;
    public float maxSpeedGain = 1f;
    public float needleRotationSpeed = 100f;
    public float rotationSpeed = 40f;

    [Header("Bar Settings")]
    public int defaultHeight = 120;

    [Header("Pole Settings")]
    public float gripX = 0.777f;
    public float gripY = 0.02f;
    public float armLength = 0.4f;
    public float stickingSpeed = 1f;
    public float vaultSpeed = 10f;
    public float vaultSpeedLoss = 0.5f;

    [Header("Jump Settings")]
    public float gravity = 9.81f;
    public float horizontalSpeedScalar = 2f;
    public float verticalSpeedScalar = 2f;
    public float slowMo = 2f;
    public float archHeightOffset = 0.5f;

    [Header("AI Settings")]
    public float aiStartTime = 1f;

    [Header("AI Hard Settings")]
    public float aiReactTimeMaxHard = 0.07f;
    public float aiReactTimeMinHard = 0f;
    public float aiMinStickDistanceHard = 7.3f;
    public float aiMaxStickDistanceHard = 7.7f;
    public float aiMinVaultAngleHard = 88f;
    public float aiMaxVaultAngleHard = 92f;
    public float aiMinReleaseAngleHard = 82f;
    public float aiMaxReleaseAngleHard = 86f;

    [Header("AI Medium Settings")]
    public float aiReactTimeMaxMedium = 0.07f;
    public float aiReactTimeMinMedium = 0f;
    public float aiMinStickDistanceMedium = 7.3f;
    public float aiMaxStickDistanceMedium = 7.7f;
    public float aiMinVaultAngleMedium = 88f;
    public float aiMaxVaultAngleMedium = 92f;
    public float aiMinReleaseAngleMedium = 82f;
    public float aiMaxReleaseAngleMedium = 86f;

    [Header("AI Easy Settings")]
    public float aiReactTimeMaxEasy = 0.07f;
    public float aiReactTimeMinEasy = 0f;
    public float aiMinStickDistanceEasy = 7.3f;
    public float aiMaxStickDistanceEasy = 7.7f;
    public float aiMinVaultAngleEasy = 88f;
    public float aiMaxVaultAngleEasy = 92f;
    public float aiMinReleaseAngleEasy = 82f;
    public float aiMaxReleaseAngleEasy = 86f;
}
