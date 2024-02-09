using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinPlayerController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public string button4 = "s";

    private int lastButton = 0;

    private float speed;

    [Header("Info")]
    [Tooltip("The current speed. Read-only.")]
    public float Speed;

    [Header("References")]
    public GameObject arm;
    public GameObject javelin;
    public GameObject controllerObj;
    private JavelinRaceController controller;
    public PowerTarget powerTarget;
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

    private bool fouled = false;

    [HideInInspector]
    public int bob = 0;

    [Header("AI Stuff")]
    private float drawX;
    private float aiAngle;
    //private float startWait = 2f;
    private float startWait;
    private JavelinConfig config;

    [HideInInspector]
    public bool eligibleForRecord;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<JavelinConfig>();

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

        if (difficulty == Difficulty.Olympic)
        {
            eligibleForRecord = true;
        }
        else if (difficulty == Difficulty.Hard)
        {
            eligibleForRecord = true;
        }
        else
        {
            eligibleForRecord = false;
        }
        if (config.disableRecordEligibility)
        {
            eligibleForRecord = false;
        }

        if (isAI)
        {
            //speedMultiplier = Random.Range(config.aiMinSpeedMultiplier, config.aiMaxSpeedMultiplier);
            //maxSpeed = Random.Range(config.aiMinMaxSpeed, config.aiMaxMaxSpeed);
            maxSpeed = config.maxSpeed;

            drawX = Random.Range(config.aiMinDrawX, config.aiMaxDrawX);
            aiAngle = Random.Range(config.aiMinAngle, config.aiMaxAngle);
            startWait = Random.Range(1f, 2f);
        }
        else
        {
            maxSpeed = config.maxSpeed;

            if (difficulty == Difficulty.Easy)
            {
                powerTarget.zoneUpperPower = 0.6f;
                powerTarget.zoneLowerPower = 0.4f;
            }
            else if (difficulty == Difficulty.Medium)
            {
                powerTarget.zoneUpperPower = 0.575f;
                powerTarget.zoneLowerPower = 0.425f;
            }
            else
            {
                powerTarget.zoneUpperPower = 0.55f;
                powerTarget.zoneLowerPower = 0.45f;
            }
        }

        controller = controllerObj.GetComponent<JavelinRaceController>();

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
                if (Input.GetKeyDown(button4))
                {
                    running = true;
                    speed = config.startingSpeed;

                    Debug.Log("Starting Speed: " + speed);

                    //powerTarget.power = 0.5f;
                    powerTarget.power = powerTarget.zoneUpperPower;

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
                    powerTarget.PowerGain();
                }


                if (Input.GetKeyDown(button3) && running)
                {
                    if (!drawback)
                    {
                        speedlock = true;
                        drawback = true;
                        powerTarget.gameObject.SetActive(false);
                    }
                    else
                    {
                        released = true;

                        javelin.transform.parent = null;
                        javelin.GetComponent<JavelinJavelinController>().Throw(javelin.transform.eulerAngles.z, speed, javelin.transform.position);

                        speed = 0f;
                        Debug.Log("You had " + (config.lineX - transform.position.x).ToString() +" metres left to the board.");
                        Debug.Log("You got " + penalties.ToString() + " penalties.");
                    }
                }
            }

            /*if (!speedlock)
            {
                speed -= config.speedDecay * Time.deltaTime;
            }*/
        }
        else
        {
            if (startWait < 0f)
            {
                if (!released && !drawback)
                {
                    speed += config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f));
                }
                if (!drawback && transform.position.x > drawX)
                {
                    speedlock = true;
                    drawback = true;
                }
                if (drawback && javelin.transform.eulerAngles.z > aiAngle && !released)
                {
                    released = true;

                    javelin.transform.parent = null;
                    javelin.GetComponent<JavelinJavelinController>().Throw(javelin.transform.eulerAngles.z, speed, javelin.transform.position);

                    speed = 0f;
                    Debug.Log("You had " + (config.lineX - transform.position.x).ToString() + " metres left to the board.");
                }
            }
        }


        if (running && !released && powerTarget.InZone() && !speedlock)
        {
            speed += config.speedChange * Time.deltaTime;
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
                arm.transform.Rotate(0f, 0f, config.armRotationSpeed * Time.deltaTime);
            }
            else if (arm.transform.eulerAngles.z > 315 || arm.transform.eulerAngles.z < 180)
            {
                arm.transform.Rotate(0f, 0f, -config.armRotationSpeed2 * Time.deltaTime);
            }
        }

        if (transform.position.x > config.lineX && !fouled)
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

        Speed = speed;
    }

    public void FixedUpdate()
    {
        
    }

    public float GetSpeed()
    {
        return speed;
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
