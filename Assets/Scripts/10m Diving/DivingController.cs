using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingController : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Easy;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public string button4 = "w";
    public string button5 = "s";

    private bool jumped = false;
    private bool rotating = false;
    private float t = 0f;
    private float t2 = 0f;
    private bool splashed = false;
    private bool extended = false;

    [Header("Dives")]
    public string[] setDive = new string[] { };
    public float[] setDiveDurations = new float[] { };
    public DivingStartingPositions startingPosition = DivingStartingPositions.Forward;
    public int rotationDirection = 1;
    public float fullTurns = 3.2f;
    private float timeToFall;
    private float rotationSpeed;
    public float points = 0f;
    private float angle = 0f;
    private int moveNum = 0;
    private float angleCount = 0f;
    private float startingRotation = 0f;
    private float hangTime = 0f;
    private bool correctDive = true;

    [Header("References")]
    public Animator anim;
    public GameObject arrow;
    public GameObject splash;

    // Start is called before the first frame update
    void Start()
    {
        timeToFall = (DivingConfig.jumpSpeedU + Mathf.Sqrt(DivingConfig.jumpSpeedU * DivingConfig.jumpSpeedU + 21f * DivingConfig.gravity)) / DivingConfig.gravity;
        rotationSpeed = timeToFall / fullTurns;

        Debug.Log("Rotation Speed: " + rotationSpeed.ToString() + " for " + fullTurns + " full turns.");

        if (startingPosition == DivingStartingPositions.Handstand)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 180f);
            startingRotation = 180f;
        }
        else if (startingPosition == DivingStartingPositions.Reverse)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (startingPosition == DivingStartingPositions.ReverseHandstand)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 180f);
            GetComponent<SpriteRenderer>().flipX = true;
            startingRotation = 180f;
        }
        
        if (difficulty == Difficulty.Easy)
        {
            arrow.SetActive(true);
        }
        else
        {
            arrow.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(button4) && ! jumped)
        {
            jumped = true;
            //rotating = true;
            anim.SetTrigger("Jump");
        }
        if (jumped)
        {
            if ((Input.GetKeyDown(button1) || Input.GetKeyDown(button2) || Input.GetKeyDown(button3)) && !rotating)
            {
                rotating = true;
                hangTime = t;
                Debug.Log("hang time points: " + (hangTime * DivingConfig.hangTimeScalar).ToString());
            }
            if (Input.GetKeyDown(button1) && !extended)
            {
                anim.SetTrigger("Tuck");
                ProcessMove("Tuck");
            }
            else if (Input.GetKeyDown(button2) && !extended)
            {
                anim.SetTrigger("Pike");
                ProcessMove("Pike");
            }
            else if (Input.GetKeyDown(button3) && !extended)
            {
                anim.SetTrigger("Twist");
                ProcessMove("Twist");
            }
            else if (Input.GetKeyDown(button5))
            {
                anim.SetTrigger("Extend");
                extended = true;
                rotating = false;
                ProcessMove("Extend");
            }
        }

        if (jumped)
        {
            transform.position = new Vector3(
                DivingConfig.diverStartX + DivingConfig.jumpSpeedR * t,
                DivingConfig.diverStartY + DivingConfig.jumpSpeedU * t - DivingConfig.gravity * t * t / 2f, 
                DivingConfig.diverStartZ);
        }
        if (rotating)
        {
            //float newAngle = -rotationDirection * 360f * t2 / DivingConfig.rotationSpeed;
            //angleCount += Mathf.Abs(newAngle - transform.eulerAngles.z);
            transform.eulerAngles = new Vector3(0f, 0f, -rotationDirection * 360f * t2 / rotationSpeed + startingRotation);
        }

        if (transform.position.y < 0f && !splashed)
        {
            splashed = true;
            GetComponent<SpriteRenderer>().sortingLayerName = "VeryBack";
            arrow.GetComponent<SpriteRenderer>().sortingLayerName = "VeryBack";
            //splash.GetComponent<Animator>().SetTrigger("Splash");
            splash.GetComponent<SplashController>().Splash(GetAngle());

            if (correctDive)
            {
                points += hangTime * DivingConfig.hangTimeScalar;
            }
            else
            {
                Debug.Log("Sorry, you do not get your hang time points");
            }

            //Debug.Log("Points: " + points.ToString("n2"));
            //Debug.Log("Points: " + points.ToString("n2") + " / " + ((setDive.Length + 1) * 20f - 10f).ToString());
            //Debug.Log("Points: " + points.ToString("n2") + " / " + ((setDive.Length + 1) * 20f - 10f).ToString() + hangTime * DivingConfig.hangTimeScalar);
            Debug.Log("Points: " + points.ToString("n2") + " / " + ((setDive.Length + 1) * 20f - 10f).ToString());
            //Debug.Log("angleCount: " + angleCount.ToString());
            //Debug.Log("angleCount: " + transform.eulerAngles.z.ToString());
        }
    }

    public void FixedUpdate()
    {
        if (jumped)
        {
            t += Time.fixedDeltaTime;
        }
        if (rotating)
        {
            t2 += Time.fixedDeltaTime;
            angleCount += Time.fixedDeltaTime / rotationSpeed;
        }
    }

    public bool HasJumped()
    {
        return jumped;
    }
    public bool HasSplashed()
    {
        return splashed;
    }

    private float GetAngle()
    {
        return Mathf.Abs(Mathf.Abs(transform.eulerAngles.z) - 180f);
    }
    private float GetAnglePoints(float angle)
    {
        //return DivingConfig.anglePointsMultiplier * 1f / (GetAngle() + 1);
        //return Functions.RoundToRange(-GetAngle()/3f + 10f, 0, 10f);
        return Functions.RoundToRange(-angle / 3f + 10f, 0, 10f);
    }

    private void ProcessMove(string move)
    {
        Debug.Log(move);
        if (moveNum > 0 && moveNum <= setDive.Length)
        {
            // 1 / 12 == 30 / 360
            if (Mathf.Abs(angleCount - setDiveDurations[moveNum-1]) < 1f / 12f)
            {
                //Debug.Log(points);
                points += GetAnglePoints(Mathf.Abs(angleCount - setDiveDurations[moveNum-1]) * 360f);
                //Debug.Log(points);
            }
            else
            {
                correctDive = false;
            }
            Debug.Log((Mathf.Abs(angleCount - setDiveDurations[moveNum-1]) * 360f).ToString() + "°");
            angleCount = (angleCount - setDiveDurations[moveNum - 1]);
        }
        else
        {
            angleCount = 0f;
        }
        //if (moveNum == setDive.Length || move == setDive[moveNum])
        if (move == "Extend" || (moveNum < setDive.Length && move == setDive[moveNum]))
        {
            points += DivingConfig.successPoints;
            Debug.Log("Correct move!");
        }
        else
        {
            Debug.Log("Wrong move!");
            correctDive = false;
        }
        //angleCount = 0f;
        //angleCount = Mathf.Floor((angleCount - setDiveDurations[moveNum - 1]) / 0.5f) * 0.5f;
        //Debug.Log("angleCount: " + angleCount.ToString());
        moveNum += 1;
        while (angleCount > 0.25f)
        {
            angleCount -= 0.5f;
        }
        while (angleCount < -0.25f)
        {
            angleCount += 0.5f;
        }
    }

    public float GetTimeToFall()
    {
        return timeToFall;
    }
}
