using System;

using UnityEngine;

enum SpeedSkatingMovementState
{
    Idle,
    Pushing,
    TurningLeft,
    TurningRight,
}

public class SpeedSkatingPlayer : MonoBehaviour
{
    private float timeSlipAngleAlignedFor = 0f;   

    private float grossAngle = 0f;

    private Rigidbody2D rb;
    private Animator anim;

    private SpeedSkatingConfig config;

    const float GRAVITY = 9.81f;

    private float angleSinceFixedUpdate = 0f;

    private float slipAngle => Vector2.SignedAngle(rb.linearVelocity, rb.transform.up);

    private float frictionForce
    {
        get
        {
            float frictionFromAngle = Mathf.Sign(slipAngle) * maxFriction * config.slipAngleToFrictionScalar.Evaluate(Mathf.Abs(slipAngle) / config.frictionMaxGripAngle);
            return Mathf.Min(frictionFromAngle, maxFriction);
        }
    }

    private float maxFriction
    {
        get
        {
            float normalForce = rb.mass * GRAVITY;
            return config.frictionCoefficient * normalForce;
        }
    }

    private SpeedSkatingMovementState _movement = SpeedSkatingMovementState.Idle;
    private SpeedSkatingMovementState movementState
    {
        get => _movement;
        set
        {
            
            if (value == _movement)
            {
                return;
            }
            _movement = value;

            switch (value)
            {
                case SpeedSkatingMovementState.Idle:
                    anim.SetTrigger("Idle");
                    break;
                case SpeedSkatingMovementState.Pushing:
                    anim.SetTrigger("Pushing");
                    break;
                case SpeedSkatingMovementState.TurningLeft:
                    anim.SetTrigger("Turn Left");
                    break;
                case SpeedSkatingMovementState.TurningRight:
                    anim.SetTrigger("Turn Right");
                    break;
                default:
                    throw new NotImplementedException();
            }

        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        config = FindFirstObjectByType<SpeedSkatingConfig>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddRelativeForce(Vector2.up * config.pushingForce, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            angleSinceFixedUpdate += Time.deltaTime * config.angularVelocity;
            grossAngle += Time.deltaTime * config.angularVelocity;
            if (grossAngle > config.grossAngleMax)
            {
                grossAngle = config.grossAngleMax;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            angleSinceFixedUpdate -= Time.deltaTime * config.angularVelocity;
            grossAngle -= Time.deltaTime * config.angularVelocity;
            if (grossAngle < -config.grossAngleMax)
            {
                grossAngle = -config.grossAngleMax;
            }
        }

        if (Mathf.Min(Mathf.Abs(slipAngle), 180 - Mathf.Abs(slipAngle)) < config.pushAngleThreshold)
        {
            timeSlipAngleAlignedFor += Time.deltaTime;
        }
        else
        {
            timeSlipAngleAlignedFor = 0f;
        }

        if (grossAngle > 0f)
        {
            grossAngle -= config.angleDecayPerSecond * Time.deltaTime;
            if (grossAngle < 0f)
            {
                grossAngle = 0f;
            }
        }
        else if (grossAngle < 0f)
        {
            grossAngle += config.angleDecayPerSecond * Time.deltaTime;
            if (grossAngle > 0f)
            {
                grossAngle = 0f;
            }
        }

        if ((movementState == SpeedSkatingMovementState.TurningLeft || movementState == SpeedSkatingMovementState.TurningRight) && Mathf.Abs(grossAngle) < config.grossAngleTurningToPushingThreshold)
        {
            grossAngle = 0f;
        }

        UpdateMovementState();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(slipAngle) < config.slipAngleSnapThreshold)
        {
            rb.linearVelocity = rb.linearVelocity.magnitude * rb.transform.up;
        }
        else
        {
            rb.AddRelativeForce(frictionForce * Vector2.left);
        }

        if (movementState == SpeedSkatingMovementState.Pushing)
        {
            rb.AddRelativeForce(config.pushingForce * Vector2.up);
        }

        if (angleSinceFixedUpdate != 0f)
        {
            rb.SetRotation(rb.transform.eulerAngles.z + angleSinceFixedUpdate);
            angleSinceFixedUpdate = 0f;
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, config.maxSpeed);
    }

    private void UpdateMovementState()
    {
        if (grossAngle > config.grossAnglePushingToTurningThreshold)
        {
            movementState = SpeedSkatingMovementState.TurningLeft;
        }
        if (grossAngle < -config.grossAnglePushingToTurningThreshold)
        {
            movementState = SpeedSkatingMovementState.TurningRight;
        }
        else if (grossAngle == 0f)
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
        else if (
            rb.linearVelocity.magnitude < config.pushSpeedThreshold
            || (
                ((Mathf.Abs(slipAngle) < config.pushAngleThreshold && rb.linearVelocity.magnitude < config.maxSpeed)
                || 180 - Mathf.Abs(slipAngle) < config.pushAngleThreshold)
                && timeSlipAngleAlignedFor >= config.pushTime
            )
        )
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = $"Velocity: {rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2}\nSpeed: {rb.linearVelocity.magnitude:F2}\nAngular Velocity: {rb.angularVelocity:F2}\nSlip Angle: {slipAngle:F2}\nFriction: {frictionForce:F2} / {maxFriction:F2}\nMovement State: {movementState}\nGross Angle: {grossAngle:F2} / {config.grossAngleMax:F2}";

        GUI.Label(
            new Rect(10, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
