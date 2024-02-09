using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprint100Controller : MonoBehaviour
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

    //private float speed = config.minSpeed;
    private float speed;

    [Header("References")]
    public GameObject raceControllerObj;
    private Sprint100RaceController raceController;
    private OlympicsController olympicsController;

    private bool running = false;
    private bool finished = false;
    private bool usedDip = false;
    private float dipCountdown = -1f;
    private bool dipFinished = false;

    private float willStartAt;
    //private float speedMultiplier;
    private float dipX;

    private int penalties = 0;
    private int presses = 0;
    private float finishTime = 0f;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;
    public Sprint100Config config;

    private bool alreadySet = false;

    private bool eligibleForRecord;

    private float aiT;

    private float t = 0f;
    private bool displayT = false;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Sprint100Config>();

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

        //StartTimer();
        raceController = raceControllerObj.GetComponent<Sprint100RaceController>();

        if (isAI)
        {
            //willStartAt = Random.Range(0f, 0.4f);
            //willStartAt = Random.Range(0.1f, 0.5f);
            //willStartAt = Random.Range(0.1f, 0.4f);
            //willStartAt = Random.Range(0.2f, 0.5f);
            willStartAt = GetAIStartTime();
            //speedMultiplier = Random.Range(0.085f, 0.092f);
            //speedMultiplier = Random.Range(0.09f, 0.092f);
            //speedMultiplier = Random.Range(0.19f, 0.23f);
            dipX = Random.Range(8.65f, 9f);
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
                //speed = config.maxSpeed / 3;

                if (raceController.GetCountdown() > 0f)
                {
                    //Debug.Log(playerName + " got a false start! There were " + raceController.GetCountdown().ToString() + " seconds left.");
                    raceController.FalseStart(playerName);
                }
                else
                {
                    //speed = config.startingSpeed;
                    //raceController.StartTimer();
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
                    //speed = 15f;
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
                //speed += config.speedChange * 0.1f;

                //if (transform.position.x > 8.65 && !usedDip)
                if (transform.position.x > dipX && !usedDip)
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

        if (!usedDip || dipFinished) {
            if (speed < config.minSpeed)
            {
                speed = config.minSpeed;
            }
            else if (speed > config.maxSpeed)
            {
                speed = config.maxSpeed;
            }
        }
        if (dipCountdown < 0f && !dipFinished && usedDip)
        {
            //Debug.Log("Dip ended!");
            //speed = 2f;
            //speed = 0.002f;
            //speed = 0.0002f;
            //speed = 0.005f;
            speed = config.afterDipSpeed;
            dipFinished = true;
            anim.SetTrigger("DipEnded");
        }

        //speed = config.maxSpeed;

        anim.SetBool("IsMoving", speed > 0);

        if (isAI && running)
        {
            if (!usedDip)
            {
                //speed += config.speedChange * 0.09f;
                //speed += config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f));
                //speed += config.speedChange * speedMultiplier;
                //speed += config.speedChange * 0.2f;
                //speed += config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f)) * Time.deltaTime; ;

                aiT -= Time.deltaTime;

                if (aiT <= 0f)
                {
                    aiT = GetAIT();
                    speed += config.speedChange;
                }
            }
        }
        transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));

        if (usedDip)
        {
            dipCountdown -= Time.deltaTime;
        }

        if (transform.position.x >= config.finishX - 0.5 && !finished)
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
        //return config.maxSpeed / (2.3f + time);
        return config.maxSpeed / 2.5f;
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

    public void ResetStarting()
    {
        anim.SetTrigger("Ready");
        alreadySet = false;
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
