using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run400Controller : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

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
    private Run400RaceController raceController;
    private Run400Config config;
    private OlympicsController olympicsController;

    private bool running = false;
    private bool finished = false;
    private bool usedDip = false;
    private float dipCountdown = -1f;
    private bool dipFinished = false;

    private float willStartAt;
    private float speedMultiplier;
    private float dipX;
    private float maxSpeed = 0f;

    private int penalties = 0;
    private int presses = 0;
    private float finishTime = 0f;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;

    private bool alreadySet = false;
    private bool doneBend2 = false;
    //private bool onHomeStraight = false;

    private float statChangeTime = 0f;
    private bool startedHomeStraight = false;

    private bool eligibleForRecord;

    private float aiT;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Run400Config>();
        raceController = raceControllerObj.GetComponent<Run400RaceController>();

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
            //ChangeSpeedMultiplier();
            dipX = Random.Range(config.finishX - 0.9f, config.finishX - 0.5f);
            ChangeMaxSpeed();
            NewStatChangeTime();
        }
        else
        {
            maxSpeed = config.maxSpeed;
        }

        speed = config.minSpeed;

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
                if (Input.GetKeyDown(button1))
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
                else if (Input.GetKeyDown(button2))
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

        //////
        //speed = 0.02f;
        //speed = 0.1f;
        ///////

        /*if (config.straightX1 < transform.position.x && transform.position.x < config.straightX2)
        {
            if (transform.position.y > 0)
            {
                transform.Translate(new Vector3(-speed, 0f, 0f));
            }
            else
            {
                transform.Translate(new Vector3(speed, 0f, 0f));
            }
        }*/
        ///else
        //if (config.straightX1 <= transform.position.x && transform.position.x < 0 && transform.position.y < 0)
        if (config.straightX1 <= transform.position.x && transform.position.x < 0 && transform.position.y < 0 && !startedHomeStraight)
        {
            if (isAI)
            {
                ChangeHomeStraightSpeed();
            }
            else
            {
                maxSpeed = config.homeStraightMaxSpeed;
            }
            startedHomeStraight = true;
        }
        if (transform.position.x >= config.straightX2)
        {
            //transform.eulerAngles = new Vector3(0f, 0f, 180f);
            transform.eulerAngles = new Vector3(0f, 0f, config.anglePlus * Time.deltaTime + Mathf.Rad2Deg * (Mathf.PI / 2 + Mathf.Atan(transform.position.y / (transform.position.x - config.straightX2))));
        }
        else if (transform.position.x <= config.straightX1)
        {
            transform.eulerAngles = new Vector3(0f, 0f, config.anglePlus * Time.deltaTime + Mathf.Rad2Deg * (3 * Mathf.PI / 2 - Mathf.Atan(transform.position.y / (config.straightX1 - transform.position.x))));
            doneBend2 = true;
        }
        transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));

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

        if (isAI)
        {
            statChangeTime -= Time.deltaTime;
            if (statChangeTime < 0f)
            {
                if (!startedHomeStraight)
                {
                    NewStatChangeTime();
                    ChangeMaxSpeed();
                    //ChangeSpeedMultiplier();
                }
                else
                {
                    NewStatChangeTime();
                    ChangeHomeStraightSpeed();
                    //ChangeSpeedMultiplier();
                }
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

    private void ChangeMaxSpeed()
    {
        //maxSpeed = Random.Range(config.aiMinMaxSpeed, config.aiMaxMaxSpeed);
        //maxSpeed = config.maxSpeed;
        if (difficulty == Difficulty.Olympic)
        {
            maxSpeed = Random.Range(config.aiOlympicMinMaxSpeed, config.aiOlympicMaxMaxSpeed);
        }
        else if (difficulty == Difficulty.Hard)
        {
            maxSpeed = Random.Range(config.aiHardMinMaxSpeed, config.aiHardMaxMaxSpeed);
        }
        else if (difficulty == Difficulty.Medium)
        {
            maxSpeed = Random.Range(config.aiMediumMinMaxSpeed, config.aiMediumMaxMaxSpeed);
        }
        else if (difficulty == Difficulty.Easy)
        {
            maxSpeed = Random.Range(config.aiEasyMinMaxSpeed, config.aiEasyMaxMaxSpeed);
        }
        else
        {
            throw new System.Exception("Unknown/unimplemented difficulty: " + difficulty);
        }
    }
    /*private void ChangeSpeedMultiplier()
    {
        speedMultiplier = Random.Range(config.aiMinSpeedMultiplier, config.aiMaxSpeedMultiplier);
    }*/
    private void NewStatChangeTime()
    {
        statChangeTime = Random.Range(4f, 6f);
    }
    private void ChangeHomeStraightSpeed()
    {
        //maxSpeed = Random.Range(config.aiMinHomeStraightMaxSpeed, config.aiMaxHomeStraightMaxSpeed);
        //maxSpeed = config.homeStraightMaxSpeed;

        if (difficulty == Difficulty.Olympic)
        {
            maxSpeed = Random.Range(config.aiOlympicMinHomeStraightMaxSpeed, config.aiOlympicMaxHomeStraightMaxSpeed);
        }
        else if (difficulty == Difficulty.Hard)
        {
            maxSpeed = Random.Range(config.aiHardMinHomeStraightMaxSpeed, config.aiHardMaxHomeStraightMaxSpeed);
        }
        else if (difficulty == Difficulty.Medium)
        {
            maxSpeed = Random.Range(config.aiMediumMinHomeStraightMaxSpeed, config.aiMediumMaxHomeStraightMaxSpeed);
        }
        else if (difficulty == Difficulty.Easy)
        {
            maxSpeed = Random.Range(config.aiEasyMinHomeStraightMaxSpeed, config.aiEasyMaxHomeStraightMaxSpeed);
        }
        else
        {
            throw new System.Exception("Unknown/unimplemented difficulty: " + difficulty);
        }
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
