using System;

using UnityEngine;

enum SpeedSkatingMovementState
{
    Idle,
    Pushing,
    TurningLeft,
    TurningRight,
}

public class SpeedSkatingTestPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    [Min(0f)]
    public float maxSpeed = 4f;
    [Min(0f)]
    public float angularVelocity = 1f;
    [Min(0f)]
    public float frictionCoefficient = 1f;
    [Min(0f)]
    public float frictionMaxGripAngle = 15f;
    [Min(0f)]
    public float force = 1f;
    [Min(0f)]
    public float pushAngleThreshold = 1f;
    [Min(0f)]
    public float pushSpeedThreshold = 1f;
    [Min(0f)]
    public float pushTime = 1f;
    private float timeSlipAngleAlignedFor = 0f;
    public AnimationCurve slipAngleToFrictionScalar;

    private Rigidbody2D rb;
    private SpriteRenderer spr;
    private Animator anim;

    const float GRAVITY = 9.81f;

    private float angleSinceFixedUpdate = 0f;

    private float slipAngle => Vector2.SignedAngle(rb.linearVelocity, rb.transform.up);

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
        spr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.AddRelativeForce(Vector2.up * force, ForceMode2D.Impulse);
        }
        if (Input.GetKey(KeyCode.A))
        {
            angleSinceFixedUpdate += Time.deltaTime * angularVelocity;
        }
        if (Input.GetKey(KeyCode.D))
        {
            angleSinceFixedUpdate -= Time.deltaTime * angularVelocity;
        }

        UpdateMovementState();
    }

    private void FixedUpdate()
    {
        rb.AddRelativeForce(FrictionForce() * Vector2.left);

        if (Mathf.Abs(slipAngle) < pushAngleThreshold)
        {
            timeSlipAngleAlignedFor += Time.fixedDeltaTime;
        }
        else
        {
            timeSlipAngleAlignedFor = 0f;
        }
        if (movementState == SpeedSkatingMovementState.Pushing)
        {
            rb.AddRelativeForce(force * Vector2.up);
        }

        if (angleSinceFixedUpdate > 0f)
        {
            movementState = SpeedSkatingMovementState.TurningLeft;
        }
        if (angleSinceFixedUpdate < 0f)
        {
            movementState = SpeedSkatingMovementState.TurningRight;
        }
        if (angleSinceFixedUpdate != 0f)
        {
            rb.SetRotation(rb.transform.eulerAngles.z + angleSinceFixedUpdate);
            angleSinceFixedUpdate = 0f;
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    private float FrictionForce()
    {
        float frictionFromAngle = Mathf.Sign(slipAngle) * MaxFriction() * slipAngleToFrictionScalar.Evaluate(Mathf.Abs(slipAngle) / frictionMaxGripAngle);
        return Mathf.Min(frictionFromAngle, MaxFriction());
    }

    private float MaxFriction()
    {
        float normalForce = rb.mass * GRAVITY;
        return frictionCoefficient * normalForce;
    }

    private void UpdateMovementState()
    {
        if (rb.linearVelocity.magnitude < pushSpeedThreshold || (Mathf.Abs(slipAngle) < pushAngleThreshold && rb.linearVelocity.magnitude < maxSpeed && timeSlipAngleAlignedFor >= pushTime))
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = $"Velocity: {rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2}\nSpeed: {rb.linearVelocity.magnitude:F2}\nAngular Velocity: {rb.angularVelocity:F2}\nSlip Angle: {slipAngle:F2}\nFriction: {FrictionForce():F2} / {MaxFriction():F2}";

        GUI.Label(
            new Rect(400, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
