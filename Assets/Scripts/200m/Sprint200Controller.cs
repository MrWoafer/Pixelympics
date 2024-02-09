using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint200Controller : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    private int playerID;
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";

    private int lastButton = 0;

    private float speed;

    [Header("References")]
    public GameObject raceControllerObj;
    private Sprint200RaceController raceController;
    private OlympicsController olympicsController;

    private bool running = false;
    private bool finished = false;
    private bool usedDip = false;
    private float dipCountdown = -1f;
    private bool dipFinished = false;

    private float willStartAt;
    //private float speedMultiplier;
    private float dipX;
    private float maxSpeed = 0f;

    private int penalties = 0;
    private int presses = 0;
    private float finishTime = 0f;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;
    private Sprint200Config config;

    private bool alreadySet = false;
    private bool doneBend2 = false;
    private bool startedHomeStraight = false;

    private float aiT;

    private bool eligibleForRecord;

    private float t = 0f;
    private bool displayT = true;

    // Start is called before the first frame update
    void Start()
    {
        raceController = raceControllerObj.GetComponent<Sprint200RaceController>();
        config = GameObject.Find("Config").GetComponent<Sprint200Config>();

        speed = config.minSpeed;

        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings != null)
        {
            playerID = playerSettings.playerIDs[playerNum];
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
            //willStartAt = Random.Range(0.2f, 0.5f);
            willStartAt = GetAIStartTime();
            //speedMultiplier = Random.Range(config.aiMinSpeedMultiplier, config.aiMaxSpeedMultiplier);
            dipX = Random.Range(config.finishX - 0.9f, config.finishX - 0.5f);
            //maxSpeed = Random.Range(config.aiMinMaxSpeed, config.aiMaxMaxSpeed);
            maxSpeed = config.maxSpeed;
        }
        else
        {
            maxSpeed = config.maxSpeed;
        }

        aiT = GetAIT();
    }

    // Update is called once per frame
    void Update()
    {
        if (raceController.HasStarted() && !alreadySet)
        {
            anim.SetTrigger("GetSet");
            alreadySet = true;
        }

        if (!isAI)
        {
            if ((Input.GetKeyDown(button1) || Input.GetKeyDown(button2)) && lastButton == 0)
            {
                if (raceController.GetCountdown() > 0f)
                {
                    raceController.FalseStart(playerName);
                }
                else
                {
                    running = true;
                    speed = CalculateStartingSpeed(Mathf.Abs(raceController.GetCountdown()));

                    anim.SetTrigger("Start");

                    Debug.Log(playerName + ", your reaction time: " + Mathf.Abs(raceController.GetCountdown()).ToString());
                    presses += 1;
                }
            }

            if (running)
            {
                t += Time.deltaTime;
                if (Input.GetKeyDown(button1))
                {
                    if (lastButton == 1)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;

                        if (displayT)
                        {
                            Debug.Log("T: " + t.ToString("n6"));
                            t = 0f;
                        }
                    }
                    else
                    {
                        speed += config.speedChange;
                        lastButton = 1;
                        presses += 1;

                        if (displayT)
                        {
                            Debug.Log("T: " + t.ToString("n6"));
                            t = 0f;
                        }
                    }
                }
                else if (Input.GetKeyDown(button2))
                {
                    if (lastButton == 2)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;

                        if (displayT)
                        {
                            Debug.Log("T: " + t.ToString("n6"));
                            t = 0f;
                        }
                    }
                    else
                    {
                        speed += config.speedChange;
                        lastButton = 2;
                        presses += 1;

                        if (displayT)
                        {
                            Debug.Log("T: " + t.ToString("n6"));
                            t = 0f;
                        }
                    }
                }
                else if (Input.GetKeyDown(button3) && !usedDip)
                {
                    usedDip = true;
                    dipCountdown = config.dipDuration;
                    speed = config.dipSpeed;
                    presses += 1;
                    anim.SetTrigger("Dip");
                }
            }
        }
        else
        {
            if (running)
            {
                if (transform.position.x > dipX && !usedDip && doneBend2)
                {
                    usedDip = true;
                    dipCountdown = config.dipDuration;
                    speed = config.dipSpeed;
                    anim.SetTrigger("Dip");
                }
            }
            else if (raceController.GetCountdown() < -willStartAt)
            {
                running = true;
                speed = CalculateStartingSpeed(willStartAt);
                anim.SetTrigger("Start");
                Debug.Log(playerName + ", your reaction time: " + willStartAt.ToString());
            }
        }

        speed -= config.speedDecay * Time.deltaTime;

        if (!usedDip || dipFinished)
        {
            if (speed < config.minSpeed)
            {
                speed = config.minSpeed;
            }
            else if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
        if (dipCountdown < 0 && !dipFinished && usedDip)
        {
            speed = config.afterDipSpeed;
            dipFinished = true;
            anim.SetTrigger("DipEnded");
        }

        anim.SetBool("IsMoving", speed > 0);

        if (isAI && running)
        {
            if (!usedDip)
            {
                //speed += config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f));
                aiT -= Time.deltaTime;

                if (aiT <= 0f)
                {
                    aiT = GetAIT();
                    speed += config.speedChange;
                }
            }
        }

        if (config.straightX1 <= transform.position.x && transform.position.x < 0 && transform.position.y < 0 && !startedHomeStraight)
        {
            //maxSpeed = config.homeStraightMaxSpeed;
            /*if (isAI)
            {
                ChangeHomeStraightSpeed();
            }
            else
            {
                maxSpeed = config.homeStraightMaxSpeed;
            }
            startedHomeStraight = true;*/
        }
        if (transform.position.x >= config.straightX2)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Time.deltaTime * config.anglePlus + Mathf.Rad2Deg * (Mathf.PI / 2 + Mathf.Atan(transform.position.y / (transform.position.x - config.straightX2))));
        }
        else if (transform.position.x <= config.straightX1)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Time.deltaTime * config.anglePlus + Mathf.Rad2Deg * (3 * Mathf.PI / 2 - Mathf.Atan(transform.position.y / (config.straightX1 - transform.position.x))));
            doneBend2 = true;
        }

        //speed = 0.02f;

        transform.Translate(new Vector3(Time.deltaTime * speed, 0f, 0f));

        if (usedDip)
        {
            dipCountdown -= Time.deltaTime;
        }

        if (transform.position.x >= config.finishX - 0.5 && !finished && doneBend2)
        {
            finishTime = raceController.GetTime();

            BroadcastScore(finishTime);

            raceController.Finish(playerName, finishTime, eligibleForRecord);

            finished = true;
            Debug.Log(playerName + ", your time: " + finishTime.ToString());

            if (!isAI)
            {
                Debug.Log(playerName + ", you got " + penalties.ToString() + " penalties out of " + presses.ToString() + " presses.");
                Debug.Log(playerName + ", you did an average of " + (presses / finishTime).ToString() + " presses per second.");
            }
        }
    }

    public void FixedUpdate()
    {
        
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float CalculateStartingSpeed(float time)
    {
        return config.maxSpeed / 2.5f;
    }

    private void ChangeHomeStraightSpeed()
    {
        //maxSpeed = Random.Range(config.aiMinHomeStraightMaxSpeed, config.aiMaxHomeStraightMaxSpeed);
    }

    private float GetAIT()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinWait, config.aiOlympicMaxWait);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinWait, config.aiHardMaxWait);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinWait, config.aiMediumMaxWait);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinWait, config.aiEasyMaxWait);
        }
        else
        {
            throw new System.Exception("Unknown/unimplemented difficulty: " + difficulty);
        }
    }

    private float GetAIStartTime()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinStart, config.aiOlympicMaxStart);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinStart, config.aiHardMaxStart);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinStart, config.aiMediumMaxStart);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinStart, config.aiEasyMaxStart);
        }
        else
        {
            throw new System.Exception("Unknown/unimplemented difficulty: " + difficulty);
        }
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score, playerID);
        }
    }
}
