using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdlesController : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    private int playerID;
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public string button4 = "w";

    private int lastButton = 0;

    private float speed;

    [Header("References")]
    public GameObject raceControllerObj;
    private HurdlesRaceController raceController;
    private OlympicsController olympicsController;

    private bool running = false;
    private bool finished = false;
    private bool usedDip = false;
    private float dipCountdown = -1f;
    private bool dipFinished = false;

    private float willStartAt;
    //private float speedMultiplier;
    private float dipX;
    private float hurdleJumpDist;
    private int hurdleNum = 0;
    private float maxSpeed;

    private int penalties = 0;
    private int presses = 0;
    private float finishTime = 0f;

    private bool jumping = false;
    private float jumpCountdown = 0f;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;
    private HurdlesConfig config;

    private bool alreadySet = false;

    private bool eligibleForRecord;

    private float aiT;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<HurdlesConfig>();
        raceController = raceControllerObj.GetComponent<HurdlesRaceController>();

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

        if (isAI)
        {
            //willStartAt = Random.Range(0.2f, 0.5f);
            willStartAt = GetAIStartTime();
            //speedMultiplier = Random.Range(config.aiMinSpeedMultiplier, config.aiMaxSpeedMultiplier);
            dipX = Random.Range(config.finishX - 1.35f, config.finishX - 1f);
            hurdleJumpDist = RandomJumpDist(difficulty);
            //hurdleJumpDist = 0f;
            //maxSpeed = Random.Range(config.aiMinMaxSpeed, config.aiMaxMaxSpeed);
            maxSpeed = config.maxSpeed;
        }
        else
        {
            maxSpeed = config.maxSpeed;
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
                    if (lastButton == 1 || jumping)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else if (!jumping)
                    {
                        speed += config.speedChange;
                        lastButton = 1;
                        presses += 1;
                    }
                }
                else if (Input.GetKeyDown(button2))
                {
                    if (lastButton == 2 || jumping)
                    {
                        speed -= config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else if (!jumping)
                    {
                        speed += config.speedChange;
                        lastButton = 2;
                        presses += 1;
                    }
                }
                else if (Input.GetKeyDown(button3) && !usedDip && !jumping)
                {
                    usedDip = true;
                    dipCountdown = config.dipDuration;
                    speed = config.dipSpeed;
                    presses += 1;
                    anim.SetTrigger("Dip");
                }
                else if (Input.GetKeyDown(button4) && !jumping)
                {
                    jumping = true;
                    jumpCountdown = config.jumpLength;
                    presses += 1;
                    anim.SetTrigger("Jump");
                }
            }
        }
        else
        {
            if (running)
            {
                if (config.hurdleXs[hurdleNum] - hurdleJumpDist < transform.position.x)
                {
                    hurdleNum += 1;
                    hurdleJumpDist = RandomJumpDist(difficulty);
                    jumping = true;
                    jumpCountdown = config.jumpLength;
                    anim.SetTrigger("Jump");
                }
                else if (transform.position.x > dipX && !usedDip)
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

        if (!jumping)
        {
            speed -= config.speedDecay * Time.deltaTime;
        }

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
        if (jumpCountdown < 0 && jumping)
        {
            jumping = false;
            anim.SetTrigger("JumpEnded");
        }

        anim.SetBool("IsMoving", speed > 0);

        if (isAI && running)
        {
            if (!usedDip && !jumping)
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

        //Debug.Log("Move");
        transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));

        if (usedDip)
        {
            dipCountdown -= Time.deltaTime;
        }
        if (jumping)
        {
            jumpCountdown -= Time.deltaTime;
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
            else
            {
                Debug.Log(playerName + ", you got " + penalties.ToString() + " penalties.");
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

    public void OnCollisionEnter2D(Collision2D col)
    {
        //speed -= config.speedPenalty * 10f;
        if (!jumping && col.gameObject.tag != "Player")
        {
            //Debug.Log("Hit!");
            speed -= config.hitPenalty;
            penalties += 1;
            if (col.gameObject.tag == "Hurdle")
            {
                col.gameObject.GetComponent<Animator>().SetTrigger("Topple");
            }
        }
    }

    private float RandomJumpDist(Difficulty difficulty)
    {
        return Random.Range(config.aiMinJumpDist[(int)difficulty], config.aiMaxJumpDist[(int)difficulty]);
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
