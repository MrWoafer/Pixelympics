using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DivingConfig
{
    //public const float jumpSpeedU = 10f;
    public const float jumpSpeedU = 6f;
    //public const float jumpSpeedR = 5f;
    public const float jumpSpeedR = 1f;

    public const float diverStartX = -2.5f;
    public const float diverStartY = 10.5f;
    public const float diverStartZ = 3f;

    //The time in seconds it takes to do a full turn.
    //public const float rotationSpeed = 1f;
    //public const float rotationSpeed = 0.7f;

    public const float gravity = 9.81f;
    //public const float gravity = 4.5f;
    //public const float gravity = 18f;

    public const float successPoints = 10f;
    public const float anglePointsMultiplier = 10f;

    //public const float rotationSpeed = 0.7f;
    //public const float fullTurns = 3.5f;
    //public const float fullTurns = 1.5f;
    //public const float fullTurns = 5f;
    //public const float fullTurns = 3.5f;
    //public const float fullTurns = 3.2f;
    //public const float fullTurns = 2.5f;
    //public const float fullTurns = 1f;
    //public const float fullTurns = 4f;
    //public const float fullTurns = 2.5f;
    //public const float fullTurns = 4.75f;
    //public const float fullTurns = 4f;
    //public readonly static float rotationSpeed = fullTurns / (jumpSpeedU - Mathf.Sqrt(jumpSpeedU * jumpSpeedU - 21f));
    //public readonly static float rotationSpeed = (jumpSpeedU - Mathf.Sqrt(jumpSpeedU * jumpSpeedU - 21f)) / fullTurns;
    //public readonly static float timeToFall = (jumpSpeedU + Mathf.Sqrt(jumpSpeedU * jumpSpeedU + 21f * gravity)) / gravity;
    ///public readonly static float rotationSpeed = timeToFall / fullTurns;

    public const float hangTimeScalar = 10f;
    //public const float hangTimeScalar = 5f;
}

public enum DivingStartingPositions
{
    Forward = 0,
    Reverse = 1,
    Handstand = 2,
    ReverseHandstand = 3
}
