using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisConfig : MonoBehaviour
{
    [Header("Player Settings")]
    public float movementSpeed = 3f;
    [Tooltip("0: default; 1: lob; 2: drop")]
    public float[] hitAnglesVertical = new float[] { 20f, 40f };
    [Tooltip("0: default; 1: side")]
    public float[] hitAnglesHorizontal = new float[] { 0f, 20f, 40f };
    [Tooltip("0: default; 1: lob; 2: drop")]
    public float[] hitSpeeds = new float[] { 14f };

    [Header("Ball Settings")]
    public float ballSpeed = 0.1f;
    public float drag = 0.1f;
    public float bounceVerticalDampingFactor = 0.9f;
    public float bounceHorizontalDampingFactor = 0.9f;

    [Header("Physics Settings")]
    public float g = 9.81f;
}
