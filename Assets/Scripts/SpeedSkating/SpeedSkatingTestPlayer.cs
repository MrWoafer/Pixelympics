using UnityEngine;

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
    public float pushThreshold = 1f;
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
    }

    private void FixedUpdate()
    {
        rb.AddRelativeForce(FrictionForce() * Vector2.left);

        if (Mathf.Abs(slipAngle) < pushThreshold)
        {
            timeSlipAngleAlignedFor += Time.fixedDeltaTime;
        }
        else
        {
            timeSlipAngleAlignedFor = 0f;
        }
        if (Mathf.Abs(slipAngle) < pushThreshold && rb.linearVelocity.magnitude < maxSpeed && timeSlipAngleAlignedFor >= pushTime)
        {
            rb.AddRelativeForce(force * Vector2.up);

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Pushing"))
            {
                anim.SetTrigger("Pushing");
            }
        }

        if (angleSinceFixedUpdate > 0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Turn Left"))
        {
            anim.SetTrigger("Turn Left");
        }
        if (angleSinceFixedUpdate < 0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Turn Right"))
        {
            anim.SetTrigger("Turn Right");
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
