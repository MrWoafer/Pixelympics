using System;

using UnityEngine;
using UnityEngine.Events;

enum SpeedSkatingMovementState
{
    WaitingToStart,
    Pushing,
    TurningLeft,
    TurningRight,
}

public class SpeedSkatingPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName = "Test";
    private int playerID;
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty aiDifficulty = Difficulty.Hard;
    public Color32 colour = new Color32(79, 148, 231, 255);

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";

    private bool eligibleForRecord = true;

    private Rigidbody2D rb;
    private Animator anim;

    private SpeedSkatingConfig config;

    private float angleSinceFixedUpdate = 0f;
    private float grossAngle = 0f;

    private float slipAngle => Vector2.SignedAngle(rb.linearVelocity, rb.transform.up);
    private float timeSlipAngleAlignedFor = 0f;

    private float frictionForce
    {
        get
        {
            float frictionFromAngle = Mathf.Sign(slipAngle) * config.maxFriction * config.slipAngleToFrictionScalar.Evaluate(Mathf.Abs(slipAngle) / config.frictionMaxGripAngle);
            return Mathf.Min(frictionFromAngle, config.maxFriction);
        }
    }

    private SpeedSkatingMovementState _movement = SpeedSkatingMovementState.WaitingToStart;
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
                case SpeedSkatingMovementState.WaitingToStart:
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

    public UnityEvent onStart { get; private set; } = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        config = FindFirstObjectByType<SpeedSkatingConfig>();

        PlayerSettingsScript playerSettings = null;
        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings is not null)
        {
            playerName = playerSettings.names[playerNum];
            playerID = playerSettings.playerIDs[playerNum];
            isAI = playerSettings.isAI[playerNum];
            aiDifficulty = playerSettings.difficulty[playerNum];
        }

        if (playerName == "Test")
        {
            eligibleForRecord = false;
        }
        if (config.disableRecordEligibility)
        {
            eligibleForRecord = false;
        }
    }

    private void Update()
    {
        if (movementState == SpeedSkatingMovementState.WaitingToStart)
        {
            if (Input.GetKeyDown(button3))
            {
                movementState = SpeedSkatingMovementState.Pushing;
                onStart.Invoke();
            }
            return;
        }

        if (Input.GetKeyDown(button3))
        {
            rb.AddRelativeForce(Vector2.up * config.pushingForce, ForceMode2D.Impulse);
        }
        if (Input.GetKey(button1))
        {
            angleSinceFixedUpdate += Time.deltaTime * config.angularVelocity;
            grossAngle += Time.deltaTime * config.angularVelocity;
            if (grossAngle > config.grossAngleMax)
            {
                grossAngle = config.grossAngleMax;
            }
        }
        if (Input.GetKey(button2))
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
            grossAngle -= config.grossAngleDecayPerSecond * Time.deltaTime;
            if (grossAngle < 0f)
            {
                grossAngle = 0f;
            }
        }
        else if (grossAngle < 0f)
        {
            grossAngle += config.grossAngleDecayPerSecond * Time.deltaTime;
            if (grossAngle > 0f)
            {
                grossAngle = 0f;
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
        if (movementState == SpeedSkatingMovementState.WaitingToStart)
        {
            return;
        }
        else if (rb.linearVelocity.magnitude < config.overrideToPushSpeedThreshold)
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
        else if (grossAngle > config.grossAnglePushingToTurningThreshold)
        {
            movementState = SpeedSkatingMovementState.TurningLeft;
        }
        else if (grossAngle < -config.grossAnglePushingToTurningThreshold)
        {
            movementState = SpeedSkatingMovementState.TurningRight;
        }
        else if ((movementState == SpeedSkatingMovementState.TurningLeft || movementState == SpeedSkatingMovementState.TurningRight) && Mathf.Abs(grossAngle) < config.grossAngleTurningToPushingThreshold)
        {
            grossAngle = 0f;
            movementState = SpeedSkatingMovementState.Pushing;
        }
        else if (grossAngle == 0f)
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
        else if (
            (
                (Mathf.Abs(slipAngle) < config.pushAngleThreshold && rb.linearVelocity.magnitude < config.maxSpeed)
                || 180 - Mathf.Abs(slipAngle) < config.pushAngleThreshold
            )
            && timeSlipAngleAlignedFor >= config.requiredTimeSlipAngleAlignedForPush
        )
        {
            movementState = SpeedSkatingMovementState.Pushing;
        }
    }

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector2.zero;

        angleSinceFixedUpdate = 0f;
        grossAngle = 0f;

        timeSlipAngleAlignedFor = 0f;

        movementState = SpeedSkatingMovementState.WaitingToStart;
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = $"Velocity: {rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2}\nSpeed: {rb.linearVelocity.magnitude:F2}\nAngular Velocity: {rb.angularVelocity:F2}\nSlip Angle: {slipAngle:F2}\nFriction: {frictionForce:F2} / {config.maxFriction:F2}\nMovement State: {movementState}\nGross Angle: {grossAngle:F2} / {config.grossAngleMax:F2}";

        GUI.Label(
            new Rect(10, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
