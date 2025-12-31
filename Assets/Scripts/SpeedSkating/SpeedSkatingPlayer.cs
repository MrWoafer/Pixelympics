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
    private SpriteRenderer spr;
    private Animator anim;

    private SpeedSkatingConfig config;
    private SpeedSkatingTrack track;

    const float GRAVITY = 9.81f;

    private float angleSinceFixedUpdate = 0f;

    private float slipAngle => Vector2.SignedAngle(rb.linearVelocity, rb.transform.up);

    private bool isInBend => Mathf.Abs(transform.position.x) > track.bendX;

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
            timeInMovementState = 0f;

            switch (value)
            {
                case SpeedSkatingMovementState.Idle:
                    movementStateCountdown = 0f;
                    anim.SetTrigger("Idle");
                    break;
                case SpeedSkatingMovementState.Pushing:
                    movementStateCountdown = 0f;
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
    private float timeInMovementState = 0f;
    private float movementStateCountdown = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        config = FindFirstObjectByType<SpeedSkatingConfig>();
        track = FindFirstObjectByType<SpeedSkatingTrack>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddRelativeForce(Vector2.up * config.force, ForceMode2D.Impulse);
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

        timeInMovementState += Time.deltaTime;
        if (movementStateCountdown > 0f)
        {
            movementStateCountdown -= Time.deltaTime;
            if (movementStateCountdown < 0f)
            {
                movementStateCountdown = 0f;
            }
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
            rb.AddRelativeForce(FrictionForce() * Vector2.left);
        }

        if (movementState == SpeedSkatingMovementState.Pushing)
        {
            rb.AddRelativeForce(config.force * Vector2.up);
        }

        if (angleSinceFixedUpdate != 0f)
        {
            rb.SetRotation(rb.transform.eulerAngles.z + angleSinceFixedUpdate);
            angleSinceFixedUpdate = 0f;
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, config.maxSpeed);
    }

    private float FrictionForce()
    {
        float frictionFromAngle = Mathf.Sign(slipAngle) * MaxFriction() * config.slipAngleToFrictionScalar.Evaluate(Mathf.Abs(slipAngle) / config.frictionMaxGripAngle);
        return Mathf.Min(frictionFromAngle, MaxFriction());
    }

    private float MaxFriction()
    {
        float normalForce = rb.mass * GRAVITY;
        return config.frictionCoefficient * normalForce;
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

        string text = $"Velocity: {rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2}\nSpeed: {rb.linearVelocity.magnitude:F2}\nAngular Velocity: {rb.angularVelocity:F2}\nSlip Angle: {slipAngle:F2}\nFriction: {FrictionForce():F2} / {MaxFriction():F2}\nMovement State: {movementState}\nGross Angle: {grossAngle:F2} / {config.grossAngleMax:F2}";

        GUI.Label(
            new Rect(10, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
