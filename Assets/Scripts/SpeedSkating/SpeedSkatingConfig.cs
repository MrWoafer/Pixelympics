using UnityEngine;

public class SpeedSkatingConfig : MonoBehaviour
{
    [Header("Movement Settings")]
    [Min(0f)]
    public float maxSpeed = 14f;
    [Min(0f)]
    public float pushingForce = 5f;
    [Min(0f)]
    public float angularVelocity = 150f;
    [Min(0f)]
    public float pushAngleThreshold = 5f;
    [Min(0f)]
    public float pushSpeedThreshold = 1f;
    [Min(0f)]
    public float pushTime = 0.4f;
    [Min(0f)]
    public float slipAngleSnapThreshold = 1f;
    [Min(0f)]
    public float angleDecayPerSecond = 50f;
    [Min(0f)]
    public float grossAnglePushingToTurningThreshold = 12f;
    [Min(0f)]
    public float grossAngleTurningToPushingThreshold = 0f;
    [Min(0f)]
    public float grossAngleMax = 15f;

    [Header("Friction Settings")]
    [Min(0f)]
    public float maxFriction = 22.5f;
    [Min(0f)]
    public float frictionMaxGripAngle = 45f;
    public AnimationCurve slipAngleToFrictionScalar;
}
