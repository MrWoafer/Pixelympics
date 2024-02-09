using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowingPlayer : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    private int playerID;
    public string playerName = "Test";
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "e";

    [Header("Info")]
    [Tooltip("Boat's current spead. Read-only.")]
    [SerializeField]
    private float Speed;
    [Tooltip("The stored speed value used to calculate the actual movement speed. Read-only.")]
    [SerializeField]
    private float SpeedInput;

    [Header("References")]
    public GameObject[] rowers;
    private RowingConfig config;
    private Animator[] anims;
    public FocusRing focusRing;
    public SpriteRenderer innerDot;
    private OlympicsController olympicsController;

    private float[] timesSincePress;

    private bool eligibleForRecord;

    private float speed = 0f;

    private float timeSinceStroke;
    private bool[] canStroke;
    private float previousTimeSinceStroke;

    private float[] aiStrokeTimes;
    private float aiStartTime;

    private bool firstStroke = true;
    private bool finished = false;
    private float finishingTime;

    private float overallTimeOff = 0f;
    private int strokes = 0;
    private int individualStrokes = 0;

    private float rhythm;
    private float timeBetweenStrokes;

    private bool resetCanStroke = false;

    private float nextRhythm;
    private float nextTimeBetweenStrokes;

    private float totalSpeedIncrease = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<RowingConfig>();

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

        if (difficulty == Difficulty.Easy)
        {
            eligibleForRecord = false;
        }
        else if (difficulty == Difficulty.Medium)
        {
            eligibleForRecord = false;
        }
        else
        {
            eligibleForRecord = true;
        }
        if (playerName == "Test")
        {
            eligibleForRecord = false;
        }
        if (config.disableRecordEligibility)
        {
            eligibleForRecord = false;
        }

        anims = new Animator[rowers.Length];
        for (int i = 0; i < rowers.Length; i++)
        {
            anims[i] = rowers[i].GetComponent<Animator>();
        }
        timesSincePress = new float[rowers.Length];
        canStroke = new bool[rowers.Length];
        aiStrokeTimes = new float[rowers.Length];

        focusRing.SetVisible(false);

        timeSinceStroke = 1000000f;

        GetAIStrokeTimes();
        aiStartTime = Random.Range(config.aiMinStartTime, config.aiMaxStartTime);

        rhythm = config.rhythm;
        timeBetweenStrokes = config.timeBetweenStrokes;

        Debug.Log(playerName + "'s PB: " + PlayerPrefs.GetFloat("Rowing PB " + playerName, 210f).ToString("n2") + "s");
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStroke += Time.deltaTime;
        for (int i = 0; i < timesSincePress.Length; i++)
        {
            timesSincePress[i] += Time.deltaTime;
        }

        if (timeSinceStroke >= timeBetweenStrokes && !resetCanStroke)
        {
            for (int i = 0; i < canStroke.Length; i++)
            {
                canStroke[i] = true;
            }

            if (!firstStroke)
            {
                //focusRing.StartShrink(config.focusRingSize, 0.1f + focusRing.thickness, rhythm - timeBetweenStrokes);
                focusRing.StartShrink(config.focusRingSize, 0.1f + focusRing.thickness, rhythm - timeSinceStroke);
                focusRing.SetVisible(true);

                rhythm = nextRhythm;
                timeBetweenStrokes = nextTimeBetweenStrokes;
            }

            resetCanStroke = true;
        }

        if (!isAI && config.CountdownHasStarted() && !finished)
        {
            //if (focusRing.HasFinished() && focusRing.isVisible)
            /*if(config.RaceHasStarted() && timeSinceStroke >= rhythm)
            {
                Row(0);
                Row(1);
            }*/
            if (Input.GetKeyDown(button1) && canStroke[0])
            {
                Row(0);
            }
            if (Input.GetKeyDown(button2) && canStroke[1])
            {
                Row(1);
            }
            if (Input.GetKeyDown(button3) && canStroke[2])
            {
                Row(2);
            }
            if (Input.GetKeyDown(button4) && canStroke[3])
            {
                Row(3);
            }
        }
        else if (config.RaceHasStarted() && config.raceTimeElapsed > aiStartTime && !finished)
        {
            for (int i = 0; i < canStroke.Length; i++)
            {
                if (canStroke[i])
                {
                    bool firstOfStroke = true;
                    for (int j = 0; j < canStroke.Length; j++)
                    {
                        firstOfStroke &= canStroke[j];
                    }

                    if (firstOfStroke)
                    {
                        if (timeSinceStroke > rhythm + aiStrokeTimes[i])
                        {
                            Row(i);
                        }
                    }
                    else
                    {
                        if (previousTimeSinceStroke + timeSinceStroke > rhythm + aiStrokeTimes[i])
                        {
                            Row(i);
                        }
                    }
                }
            }
        }

        transform.position += new Vector3(GetSpeed(speed) * Time.deltaTime, 0f, 0f);

        speed -= (config.speedDecay + 0.001f * Mathf.Pow(speed, 3f)) * Time.deltaTime;
        //speed -= config.speedDecay * Time.deltaTime;
        if (speed < 0f)
        {
            speed = 0f;
        }

        if (GetFrontOfBoat() > config.finishX && !finished)
        {
            Finish();
        }

        Speed = GetSpeed(speed);
        SpeedInput = speed;
    }

    private void Row(int rower)
    {
        if (config.RaceHasStarted())
        {
            bool wasFirstOfStroke = false;

            anims[rower].SetTrigger("Row");

            if (timeSinceStroke >= timeBetweenStrokes)
            {
                if (firstStroke)
                {
                    timeSinceStroke = rhythm;
                    Debug.Log(playerName + "'s start time: " + (-config.countdown).ToString("n5") + "s");
                }
                firstStroke = false;

                resetCanStroke = false;

                //Debug.Log("New Stroke.");
                strokes++;

                previousTimeSinceStroke = timeSinceStroke;
                timeSinceStroke = 0f;

                if (previousTimeSinceStroke + timeSinceStroke < rhythm)
                {
                    nextRhythm = config.rhythm + rhythm - (previousTimeSinceStroke + timeSinceStroke);
                    //nextRhythm = config.rhythm;
                    innerDot.color = Color.green;
                }
                else
                {
                    nextRhythm = config.rhythm;
                    //nextRhythm = Mathf.Max(config.rhythm - (rhythm - (previousTimeSinceStroke + timeSinceStroke)), 1f);
                    innerDot.color = Color.red;
                }
                nextTimeBetweenStrokes = nextRhythm - (config.rhythm - config.timeBetweenStrokes);

                //focusRing.SetVisible(false);
                focusRing.StopShrink();

                wasFirstOfStroke = true;
            }
            else
            {

            }

            float offFromOthers = 0f;
            for (int i = 0; i < timesSincePress.Length; i++)
            {
                if (!canStroke[i] && i != rower)
                {
                    //offFromOthers += Mathf.Sqrt(timesSincePress[i]);
                    offFromOthers += Mathf.Pow(timesSincePress[i], 0.5f);
                }
            }

            //float speedIncrease = (1f - 0.1f * Mathf.Pow(10f * Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke)), 0.4f)) * config.speedGain;
            //float speedIncrease = (1f - Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke))) * config.speedGain;
            //float speedIncrease = (1f - 0.3f * Mathf.Pow(Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke)), 0.4f)) * config.speedGain;
            float speedIncrease = (1f - 0.4f * Mathf.Pow(Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke)), 0.3f)) * config.speedGain;
            speedIncrease = Mathf.Max(0f, speedIncrease);
            speed += speedIncrease;
            //speed = speedIncrease;
            if (!isAI)
            {
                //speed = config.speedGain;
            }

            totalSpeedIncrease += speedIncrease;

            if (!isAI)
            {
                //Debug.Log(Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke)));
            }
            //Debug.Log(playerNum + ": " + Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke)));

            //overallTimeOff += previousTimeSinceStroke + timeSinceStroke;
            overallTimeOff += Mathf.Abs(rhythm - (previousTimeSinceStroke + timeSinceStroke));

            //Debug.Log("Time " + rower + ": " + (previousTimeSinceStroke + timeSinceStroke).ToString("n4"));
            //Debug.Log("Speed Increase " + rower + ": " + speedIncrease.ToString("n4"));

            float timeSince = timesSincePress[rower];

            //Debug.Log("Rower: " + rower);

            timesSincePress[rower] = 0f;
            canStroke[rower] = false;

            individualStrokes++;

            if (isAI)
            {
                GetAIStrokeTime(rower);
            }
        }
        else
        {
            Debug.Log(playerName + " made a false start! They had " + config.countdown.ToString("n5") + "s remaining.");
            config.FalseStart();
        }
    }

    public void GetReady()
    {
        for (int i = 0; i < anims.Length; i++)
        {
            anims[i].SetTrigger("Ready");
        }
    }

    public float GetSpeed(float _speed)
    {
        //return 10f - Mathf.Exp(-0.1f * Mathf.Pow(_speed, 1.5f) + Mathf.Log(10f));
        //return 1.8f * _speed;
        //return Mathf.Pow(individualStrokes / overallTimeOff, 0.1f);

        //return 7f - Mathf.Exp(-0.15f * Mathf.Pow(_speed, 1.2f) + Mathf.Log(7f));
        return 1.8f * (6.7f - Mathf.Exp(-0.07f * Mathf.Pow(_speed, 1.2f) + Mathf.Log(6.7f)));
    }

    private void Finish()
    {
        finished = true;
        finishingTime = config.raceTimeElapsed;
        BroadcastScore(finishingTime);
        Debug.Log(playerName + " has finished! Time: " + finishingTime.ToString("n2"));
        Debug.Log(playerName + " overall time off: " + overallTimeOff.ToString("n4") + "  Mean: " + (overallTimeOff / individualStrokes).ToString("n5"));
        Debug.Log(playerName + " made " + individualStrokes + " strokes.");
        Debug.Log(playerName + " had a mean speed increase of " + (totalSpeedIncrease / individualStrokes).ToString("n5"));
        CheckRecords();
    }

    private void GetAIStrokeTimes()
    {
        for (int i = 0; i < aiStrokeTimes.Length; i++)
        {
            GetAIStrokeTime(i);
        }
    }
    private void GetAIStrokeTime(int i)
    {
        if (difficulty == Difficulty.Hard)
        {
            aiStrokeTimes[i] = Random.Range(config.aiHardMinStrokeOffBy, config.aiHardMaxStrokeOffBy);
        }
        else if (difficulty == Difficulty.Medium)
        {
            aiStrokeTimes[i] = Random.Range(config.aiMediumMinStrokeOffBy, config.aiMediumMaxStrokeOffBy);
        }
        else
        {
            aiStrokeTimes[i] = Random.Range(config.aiEasyMinStrokeOffBy, config.aiEasyMaxStrokeOffBy);
        }
    }

    public float GetFrontOfBoat()
    {
        return transform.position.x + config.frontOfBoatOffset;
    }

    private void CheckRecords()
    {
        if (eligibleForRecord)
        {
            if (finishingTime < PlayerPrefs.GetFloat("Rowing PB " + playerName, 210f))
            {
                Debug.Log(playerName + " got a new PB!");
                PlayerPrefs.SetFloat("Rowing PB " + playerName, finishingTime);
            }

            if (finishingTime < PlayerPrefs.GetFloat("Rowing Record", 210f))
            {
                PlayerPrefs.SetFloat("Rowing Record", finishingTime);
                Debug.Log("New Record!");
            }
        }
    }

    public bool HasFinished()
    {
        return finished;
    }
    public float GetFinishTime()
    {
        return finishingTime;
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
