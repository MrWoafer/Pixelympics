using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Relay200Config
{
    public const float finishX = 5;
    public const float straightX1 = -5;
    public const float straightX2 = 5;

    public const float minTime = 9f;
    public const float maxSpeed = 0.018f;
    public const float homeStraightMaxSpeed = 0.022f;
    public const float minSpeed = 0f;

    public const float speedChange = 0.0004f;
    public const float speedPenalty = 0.001f;
    public const float speedDecay = 0.00002f;

    public const float startingSpeed = maxSpeed / 2.5f;
    public const float maxMPS = 11f;

    public const float dipDuration = 0.2f;

    //public const float aiMinSpeedMultiplier = 0.18f;
    public const float aiMinSpeedMultiplier = 0.19f;
    //public const float aiMaxSpeedMultiplier = 0.21f;
    public const float aiMaxSpeedMultiplier = 0.23f;

    public const float aiMinMaxSpeed = 0.016f;
    public const float aiMaxMaxSpeed = maxSpeed;

    public const float anglePlus = 0.1f;

    //public const float tagTime = 4f;
    public const float tagTime = 3f;

    //public const float aiMinStartDist = 0.2f;
    public const float aiMinStartDist = 0.5f;
    //public const float aiMaxStartDist = 0.5f;
    public const float aiMaxStartDist = 1.5f;
    //public const float aiMaxStartDist = 2.5f;
    //public const float aiMaxStartDist = 2f;

    public const float aiMaxHomeStraightMaxSpeed = 0.021f;
    public const float aiMinHomeStraightMaxSpeed = homeStraightMaxSpeed;
}
