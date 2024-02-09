using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightliftingConfig : MonoBehaviour
{
    [Header("Player Settings")]
    public float powerGain = 1f;
    public float powerLoss = 0f;
    public float angleSpeed = 120f;

    [Header("AI Settings")]
    public float aiMinPressTime = 0.045f;
    public float aiMaxPressTime = 0.12f;
    public float aiMinAngle = 82f;
    public float aiMaxAngle = 98f;
}
