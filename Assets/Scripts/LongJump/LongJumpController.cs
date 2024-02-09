using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJumpController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public string button4 = "s";

    private int lastButton = 0;

    private float speed;

    [Header("References")]
    public GameObject needle;
    public GameObject controllerObj;
    private LongJumpRaceController controller;
    public GameObject aimDial;
    public GameObject sandpile;
    private LongJumpConfig config;
    private OlympicsController olympicsController;

    private bool running = false;

    private float speedMultiplier;
    private float maxSpeed = 0f;

    private int penalties = 0;
    private int presses = 0;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;

    private bool speedlock = false;
    private bool drawback = false;
    private bool released = false;

    [HideInInspector]
    public int bob = 0;

    [Header("AI Stuff")]
    private float drawX;
    private float aiAngle;
    //private float startWait = 2f;
    private float startWait;

    private float lockSpeed;
    private float angle = 0f;
    private float t = 0f;
    private float speedU;
    private float speedR;
    private Vector3 releasePoint;
    private bool displayedDistance = false;
    private bool fouled = false;
    private bool allowedToGetRecord;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<LongJumpConfig>();

        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings != null)
        {
            isAI = playerSettings.isAI[playerNum];
            playerName = playerSettings.names[playerNum];
            difficulty = playerSettings.difficulty[playerNum];
        }

        if (isAI)
        {
            startWait = Random.Range(1f, 2f);
            speedMultiplier = Random.Range(config.aiMinSpeedMultiplier, config.aiMaxSpeedMultiplier);
            maxSpeed = Random.Range(config.aiMinMaxSpeed, config.aiMaxMaxSpeed);

            drawX = Random.Range(config.aiMinDrawX, config.aiMaxDrawX);
            aiAngle = Random.Range(config.aiMinAngle, config.aiMaxAngle);
        }
        else
        {
            maxSpeed = config.maxSpeed;
        }

        if (difficulty == Difficulty.Easy)
        {
            transform.position = new Vector3(-10f, -2.5f, 0f);
            allowedToGetRecord = false;
        }
        else if (difficulty == Difficulty.Medium)
        {
            transform.position = new Vector3(-8f, -2.5f, 0f);
            allowedToGetRecord = true;
        }
        else
        {
            transform.position = new Vector3(-6f, -2.5f, 0f);
            allowedToGetRecord = true;
        }
        if (config.disableRecordEligibility)
        {
            allowedToGetRecord = false;
        }

        controller = controllerObj.GetComponent<LongJumpRaceController>();

        //aimDial.SetActive(false);
        sandpile.SetActive(false);

        speed = config.minSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAI)
        {
            /*if ((Input.GetKeyDown(button1) || Input.GetKeyDown(button2)) && lastButton == 0 && !running)
            {
                running = true;
                speed = config.startingSpeed;

                anim.SetTrigger("Start");
                presses += 1;
            }*/
            if (!running)
            {
                if (Input.GetKeyDown(button4) && lastButton == 0)
                {
                    running = true;
                    speed = config.startingSpeed;

                    anim.SetTrigger("Start");
                    presses += 1;
                }
            }

            //if (running)
            else
            {
                /*if (Input.GetKeyDown(button1) && !speedlock)
                {
                    if (lastButton == 1)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else
                    {
                        speed += config.speedChange;
                        lastButton = 1;
                        presses += 1;
                    }
                }
                else if (Input.GetKeyDown(button2) && !speedlock)
                {
                    if (lastButton == 2)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else
                    {
                        speed += config.speedChange;
                        lastButton = 2;
                        presses += 1;
                    }
                }*/
                if (Input.GetKeyDown(button4) && !speedlock)
                {
                    speed += config.speedChange;
                    presses += 1;
                }
                //else if (Input.GetKeyDown(button3) && running &&)
                if (Input.GetKeyDown(button3) && running && !released)
                {
                    if (!drawback)
                    {
                        lockSpeed = speed;
                        drawback = true;
                        speed = 0f;
                        speedlock = true;
                        anim.SetTrigger("Aim");

                        aimDial.SetActive(true);
                    }
                    else
                    {
                        anim.SetTrigger("Jump");

                        //aimDial.SetActive(false);
                        //aimDial.transform.parent = null;

                        released = true;
                        t = 0f;

                        speed = 0f;
                        Debug.Log("You had " + (config.lineX - transform.position.x).ToString() + " metres left to the board.");
                        Debug.Log("You got " + penalties.ToString() + " penalties.");

                        releasePoint = transform.position;

                        speedU = lockSpeed * config.releaseSpeedMultiplierU * Mathf.Sin(angle * Mathf.Deg2Rad);
                        speedR = lockSpeed * config.releaseSpeedMultiplierR * Mathf.Cos(angle * Mathf.Deg2Rad);

                        Debug.Log(angle.ToString() + "°, " + speed.ToString() + " (" + (lockSpeed / config.maxSpeed * config.maxMPS).ToString("n2") + "m/s), " + releasePoint.ToString());
                    }
                }
            }

            if (!speedlock)
            {
                speed -= config.speedDecay * Time.deltaTime;
            }
        }
        else
        {
            if (startWait < 0f)
            {
                if (!released && !drawback)
                {
                    speed += config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f));
                    anim.SetTrigger("Start");
                }
                if (!drawback && transform.position.x > drawX)
                {
                    lockSpeed = speed;
                    drawback = true;
                    speed = 0f;
                    speedlock = true;
                    anim.SetTrigger("Aim");

                    aimDial.SetActive(true);
                }
                if (drawback && needle.transform.eulerAngles.z - 270f > aiAngle && !released)
                {
                    anim.SetTrigger("Jump");

                    //aimDial.SetActive(false);
                    //aimDial.transform.parent = null;

                    released = true;
                    t = 0f;

                    speed = 0f;
                    Debug.Log("You had " + (config.lineX - transform.position.x).ToString() + " metres left to the board.");
                    Debug.Log("You got " + penalties.ToString() + " penalties.");

                    releasePoint = transform.position;

                    speedU = lockSpeed * config.releaseSpeedMultiplierU * Mathf.Sin(angle * Mathf.Deg2Rad);
                    speedR = lockSpeed * config.releaseSpeedMultiplierR * Mathf.Cos(angle * Mathf.Deg2Rad);

                    Debug.Log(angle.ToString() + "°, " + speed.ToString() + " (" + (lockSpeed / config.maxSpeed * config.maxMPS).ToString("n2") + "m/s), " + releasePoint.ToString());
                }
            }
        }

        if (released)
        {
            if (transform.position.y > -3f)
            {
                transform.position = new Vector3(releasePoint.x + speedR * t, releasePoint.y + speedU * t - config.gravity * t * t / 2, 0f);
            }
            else if (!displayedDistance)
            {
                displayedDistance = true;
                var distanceJumped = Functions.RoundToRange(transform.position.x - config.lineX, 0f, 10000f);
                Debug.Log("Distance jumped: " + distanceJumped.ToString("n2") + " m");

                controller.SetWinnerDistance(distanceJumped);

                if (!fouled)
                {
                    BroadcastScore(distanceJumped);
                }

                if (distanceJumped > PlayerPrefs.GetFloat("Long Jump PB " + playerName, 0f) && !fouled && allowedToGetRecord)
                {
                    Debug.Log(playerName + " got a new PB!");
                    PlayerPrefs.SetFloat("Long Jump PB " + playerName, distanceJumped);
                }

                if (distanceJumped > PlayerPrefs.GetFloat("Long Jump Record", 9f) && !fouled && allowedToGetRecord)
                {
                    PlayerPrefs.SetFloat("Long Jump Record", distanceJumped);
                    Debug.Log("New Record!");
                    controller.SetRecordDistance("WR");
                }
                else if (distanceJumped < PlayerPrefs.GetFloat("Long Jump Worst Record", 1f) && distanceJumped > 0f && !fouled)
                {
                    PlayerPrefs.SetFloat("Long Jump Worst Record", distanceJumped);
                    Debug.Log("New Worst Record!");
                    controller.SetRecordDistance("WR");
                }

                sandpile.SetActive(true);
            }
        }


        if (speed < config.minSpeed)
        {
            speed = config.minSpeed;
        }
        else if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }

        //anim.SetBool("IsMoving", speed > 0);

        transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));

        if (drawback)
        {
            if (!released)
            {
                needle.transform.Rotate(0f, 0f, config.needleRotationSpeed * Time.deltaTime);
                if (needle.transform.eulerAngles.z > 0f && needle.transform.eulerAngles.z < 270f)
                {
                    needle.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                }
                //angle = Mathf.Abs(needle.transform.eulerAngles.z) - 270f;
                angle = needle.transform.eulerAngles.z - 270f;
            }
            else
            {
                //t += Time.fixedDeltaTime;
                t += Time.deltaTime / config.slowMo;
            }
        }

        if (transform.position.x > config.lineX && !released && !fouled)
        {
            //Debug.Log("Foul!");
            controller.Foul(playerName);
            fouled = true;

            BroadcastScore(float.MinValue);
        }

        if (isAI)
        {
            startWait -= Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        
    }

    public float GetSpeed()
    {
        return speed;
    }
    public float GetAngle()
    {
        return angle;
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score);
        }
    }
}
