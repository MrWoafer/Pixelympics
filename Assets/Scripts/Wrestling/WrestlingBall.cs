using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotationDirection
{
    clockwise = 1,
    anticlockwise = -1
}

public class WrestlingBall : MonoBehaviour
{
    [Header("References")]
    public Wrestler p1;
    public Wrestler p2;
    public SpriteRenderer spr1;
    public SpriteRenderer spr2;
    private SpriteRenderer p1Spr;
    private SpriteRenderer p2Spr;
    private WrestlingConfig config;
    public GameObject rotator;
    private Rigidbody rb;

    private Wrestler onTop;
    private Wrestler onBottom;

    private float rotation = 0f;

    private float timeBetweenPoints = 0.5f;
    private float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<WrestlingConfig>();

        p1Spr = p1.GetComponent<SpriteRenderer>();
        p2Spr = p2.GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotation != 0)
        {
            rotation /= Mathf.Abs(rotation);
        }

        float angle = rotation * config.rotationSpeed * Time.deltaTime;
        rotator.transform.Rotate(new Vector3(0f, 0f, angle));
        transform.Translate(new Vector3(transform.lossyScale.x / 2f * -angle * Mathf.Deg2Rad, 0f, 0f));

        rb.AddForce(new Vector3(0f, -10f, 0f));

        //rb.AddTorque(new Vector3(angle, angle, angle));

        rotation = 0f;

        t -= Time.deltaTime;
        if (t <= 0f)
        {
            t = timeBetweenPoints;

            if (onTop != null && onBottom != null)
            {
                if (Functions.Mod(rotator.transform.eulerAngles.z, 360f) < 180f)
                {
                    onBottom.AddPoints(1);
                }
                else
                {
                    onTop.AddPoints(1);
                }
            }
        }
    }

    public void Rotate(RotationDirection direction)
    {
        if (direction == RotationDirection.clockwise)
        {
            rotation -= 1f;
        }
        else if (direction == RotationDirection.anticlockwise)
        {
            rotation += 1f;
        }
    }

    public void MakeBall(Wrestler _onTop)
    {
        onTop = _onTop;

        p1.inBall = true;
        p2.inBall = true;
        p1.anim.SetTrigger("Ball");
        p2.anim.SetTrigger("Ball");
        
        if (onTop == p1)
        {
            onBottom = p2;
        }
        else
        {
            onBottom = p1;
        }

        p1Spr.enabled = false;
        p2Spr.enabled = false;

        spr1.enabled = true;
        spr2.enabled = true;

        spr1.material.SetColor("_GiColour", p1.giColour);
        spr1.material.SetColor("_SkinColour", p1.skinColour);
        spr1.material.SetColor("_HairColour", p1.hairColour);
        spr1.material.SetColor("_BeltColour", config.gradeBeltColours[p1.grade]);

        spr2.material.SetColor("_GiColour", p2.giColour);
        spr2.material.SetColor("_SkinColour", p2.skinColour);
        spr2.material.SetColor("_HairColour", p2.hairColour);
        spr2.material.SetColor("_BeltColour", config.gradeBeltColours[p2.grade]);

        rotator.transform.eulerAngles = new Vector3(0f, 0f, 90f * onBottom.direction);

        transform.position = onBottom.transform.position;
    }
}
