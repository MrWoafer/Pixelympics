using UnityEngine;

public class SpeedSkatingPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    [Min(0f)]
    public float forceScalar = 1f;
    [Min(0f)]
    public float maxSpeed = 4f;
    [Min(0f)]
    public float torque = 1f;
    [Min(0f)]
    public float maxAngularSpeed = 1f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A) && rb.angularVelocity < maxAngularSpeed)
        {
            rb.AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.D) && rb.angularVelocity > -maxAngularSpeed)
        {
            rb.AddTorque(-torque);
        }

        if (Vector2.Dot(rb.linearVelocity, rb.transform.up) < maxSpeed)
        {
            rb.AddRelativeForce(Vector2.up * forceScalar);
        }

        rb.linearVelocity = Vector2.ClampMagnitude(rb.linearVelocity, maxSpeed);
        rb.angularVelocity = Mathf.Clamp(rb.angularVelocity, -maxAngularSpeed, maxAngularSpeed);
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = $"Velocity: {rb.linearVelocity.x:F2}, {rb.linearVelocity.y:F2}\nSpeed: {rb.linearVelocity.magnitude:F2}\nAngular Velocity: {rb.angularVelocity:F2}";

        GUI.Label(
            new Rect(10, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
