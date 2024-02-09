using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeirinConfig : MonoBehaviour
{
    [Header("Beat Settings")]
    public float beatFrequency = 1f;
    public float beatLightDuration = 0.2f;
    public Color beatLightOnColour = new Color(255f, 255f, 255f);
    public Color beatLightOffColour = new Color(0f, 0f, 0f);

    [Header("Movement Settings")]
    public float speedMultiplier = 2f;
    public float maxSpeedGain = 0.5f;
    public float speedLossCoefficient = 1f;
    public float speedPenalty = 0.5f;
    public float crashSpeedLossMultiplier = 0.5f;
    public float behindSpeedGain = 0.2f;

    [Header("Player Settings")]

    [Header("AI Settings")]
    public float aiHardMaxMultiplier = 1f;
    public float aiHardMinMultiplier = 0.8f;
    public float aiMediumMaxMultiplier = 0.8f;
    public float aiMediumMinMultiplier = 0.6f;
    public float aiEasyMaxMultiplier = 0.6f;
    public float aiEasyMinMultiplier = 0.4f;
    public float aiLaneChangeTimerMax = 1f;
    public float aiLaneChangeTimerMin = 0.5f;
    public float aiMaxLaneChangeDistance = 1f;

    [Header("Track Settings")]
    public float leftBendX = -5f;
    public float rightBendX = 5f;
    public float centreY = 0f;
    public float startLineX = -1.5f;
    public float lapCheckX = -0.95f;

    public float GetLaneRadius(int lane)
    {
        switch (lane)
        {
            case 0:
                return 3.555f;
            case 1:
                return 4.193f;
            case 2:
                return 4.821f;
             case 3:
                return 5.454f;
            default:
                return 3.555f;
        }
    }

    public float TrackLength(int lane)
    {
        return (rightBendX - leftBendX) * 2f + GetLaneRadius(lane) * 2f * Mathf.PI;
    }
}
