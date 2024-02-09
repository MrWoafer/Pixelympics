using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedClimber : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

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
    public string button4 = "s";

    [Header("References")]
    private SpeedClimbingConfig config;
    private SpeedClimbingHold[] holds;
    private Animator anim;
    private SpriteRenderer spr;
    private LineRenderer lr;
    public GameObject lineAttachPoint;
    public Text timerText;
    private OlympicsController olympicsController;

    private int holdNum;
    private bool atHold = true;

    private bool flying = false;

    float angle, distance;
    private Vector3 from, to;
    private float timeToReach;
    private float t = 0f;

    private bool finished = false;
    private float finishingTime;

    private bool eligibleForRecord;

    private bool firstPress = true;

    private float aiT;

    private float penaltyTime;

    private int mistakes = 0;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<SpeedClimbingConfig>();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        lr = GetComponent<LineRenderer>();

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
        if (config.randomiseButtons)
        {
            eligibleForRecord = false;
        }

        aiT = Random.Range(config.aiMinStartTime, config.aiMaxStartTime);

        Debug.Log(playerName + "'s PB: " + PlayerPrefs.GetFloat("Speed Climbing Course " + config.currentCourseName + " PB " + playerName, 100f).ToString("n3") + "s");
    }

    // Update is called once per frame
    void Update()
    {
        if (config.CountdownHasStarted())
        {
            if (atHold)
            {
                if (holdNum < holds.Length)
                {
                    if (!isAI)
                    {
                        penaltyTime -= Time.deltaTime;

                        if (penaltyTime <= 0f)
                        {
                            if (Input.GetKeyDown(GetButton(holds[holdNum].button)))
                            //if (Input.GetKeyDown(holds[holdNum].button))
                            {
                                if (config.RaceHasStarted())
                                {
                                    NextHold();
                                }
                                else
                                {
                                    FalseStart();
                                }
                            }
                            else if (PressedWrongButton())
                            {
                                mistakes++;
                                penaltyTime = config.wrongButtonPenalty;
                            }
                        }
                    }
                    else if (config.RaceHasStarted())
                    {
                        aiT -= Time.deltaTime;

                        if (aiT <= 0f)
                        {
                            NextHold();
                            GetAIWaitTime();
                        }
                    }
                }
                else
                {
                    if (!finished)
                    {
                        Finish();
                    }
                }
            }
            else if (flying)
            {
                if (Input.GetKeyDown(button1) || Input.GetKeyDown(button2) || Input.GetKeyDown(button3) || Input.GetKeyDown(button4))
                {
                    mistakes++;
                }

                t += Time.deltaTime;

                float param;
                //param = t / timeToReach;
                if (t <= timeToReach / 2f)
                {
                    param = 2f / timeToReach / timeToReach * t * t;
                }
                else
                {
                    param = 1f - 2f / timeToReach / timeToReach * (timeToReach - t) * (timeToReach - t);
                }

                transform.position = Functions.Lerp(from, to, param);

                if (Vector3.Distance(transform.position, to) < config.snapDistance)
                {
                    transform.position = to;
                    flying = false;
                    holdNum++;
                    atHold = true;

                    //anim.SetTrigger("Hold");
                    if (to.x - from.x > config.minSwingXDifference)
                    {
                        spr.flipX = false;
                        anim.SetTrigger("SwingR");
                    }
                    else if (to.x - from.x < -config.minSwingXDifference)
                    {
                        spr.flipX = true;
                        anim.SetTrigger("SwingR");
                    }
                    else
                    {
                        anim.SetTrigger("Hold");
                    }
                }
            }
        }

        UpdateLine();
        UpdateTimer();
    }

    public void SetHolds(SpeedClimbingHold[] _holds)
    {
        holds = _holds;
    }

    public string GetButton(string key)
    {
        if (key == "w")
        {
            return button3;
        }
        else if (key == "a")
        {
            return button1;
        }
        else if (key == "d")
        {
            return button2;
        }
        else if (key == "s")
        {
            return button4;
        }
        else
        {
            throw new System.ArgumentException("Unknown/unimplemented key: " + key);
        }
    }

    public void Launch()
    {
        from = transform.position;
        to = holds[holdNum].transform.position;

        //timeToReach = distance / 3f;
        timeToReach = Mathf.Log(distance + 1) / 7f;
        //timeToReach = Mathf.Log(distance + 1) / 6f;
        //timeToReach = Mathf.Log(distance + 1) / 20f;

        flying = true;
        t = 0f;

        if (angle < 5f)
        {
            anim.SetTrigger("ReachU");
        }
        else if (angle < 45f)
        {
            if (from.x < to.x)
            {
                spr.flipX = false;
                anim.SetTrigger("ReachUR");
            }
            else
            {
                spr.flipX = true;
                anim.SetTrigger("ReachUR");
            }
        }
        else
        {
            if (from.x < to.x)
            {
                spr.flipX = false;
                anim.SetTrigger("ReachR");
            }
            else
            {
                spr.flipX = true;
                anim.SetTrigger("ReachR");
            }
        }
    }

    public void TranslatePixelsX(float pixels)
    {
        transform.position += new Vector3(pixels / 16f * transform.localScale.x, 0f, 0f);
    }
    public float PixelsToDistance(float pixels)
    {
        return pixels / 16f * transform.localScale.x;
    }
    public void TranslatePixelsY(float pixels)
    {
        transform.position += new Vector3(0f, pixels / 16f * transform.localScale.y, 0f);
    }

    private void UpdateLine()
    {
        Vector3[] points = new Vector3[] { GetLinePoint(), holds[holds.Length - 1].transform.position + new Vector3(0f, config.paddingTop, 0f) };
        for (int i = 0; i < points.Length; i++)
        {
            points[i].z = -1f;
        }
        lr.SetPositions(points);
    }

    private Vector3 GetLinePoint()
    {
        if (!spr.flipX)
        {
            return lineAttachPoint.transform.position;
        }
        else
        {
            return new Vector3(transform.position.x - (lineAttachPoint.transform.position.x - transform.position.x), lineAttachPoint.transform.position.y, lineAttachPoint.transform.position.z);
        }
    }

    private void UpdateTimer()
    {
        if (!finished)
        {
            timerText.text = config.raceTimeElapsed.ToString("n3");
        }
        else
        {
            timerText.text = finishingTime.ToString("n3");
        }
    }

    private void Finish()
    {
        finished = true;
        finishingTime = config.raceTimeElapsed;

        BroadcastScore(finishingTime);

        Debug.Log(playerName + " finished in " + finishingTime.ToString("n3") + "s");
        Debug.Log(playerName + " made " + mistakes + " mistake" + (mistakes == 1 ? "" : "s"));
        CheckRecords();
    }

    private void FalseStart()
    {
        Debug.Log(playerName + " made a false start. They had " + config.GetCountdown().ToString("n5") + "s left.");
        config.FalseStart();
    }

    private void CheckRecords()
    {
        if (eligibleForRecord)
        {
            if (finishingTime < PlayerPrefs.GetFloat("Speed Climbing Course " + config.currentCourseName + " PB " + playerName, 100f))
            {
                Debug.Log(playerName + " got a new PB!");
                PlayerPrefs.SetFloat("Speed Climbing Course " + config.currentCourseName + " PB " + playerName, finishingTime);
            }

            if (finishingTime < PlayerPrefs.GetFloat("Speed Climbing Course " + config.currentCourseName + " Record", 100f))
            {
                PlayerPrefs.SetFloat("Speed Climbing Course " + config.currentCourseName + " Record", finishingTime);
                Debug.Log("New Record!");
            }
        }
    }

    public void Null()
    {
    }

    private void NextHold()
    {
        if (firstPress)
        {
            Debug.Log(playerName + "'s start time: " + (-config.GetCountdown()).ToString("n5") + "s");
            firstPress = false;
        }

        if (holdNum == 0)
        {
            //angle = Vector3.Angle(holds[holdNum].transform.position, transform.position);
            angle = Mathf.Atan(Mathf.Abs(holds[holdNum].transform.position.x - transform.position.x) / Mathf.Abs(holds[holdNum].transform.position.y - transform.position.y));
            distance = Vector3.Distance(holds[holdNum].transform.position, transform.position);
        }
        else
        {
            //angle = Vector3.Angle(holds[holdNum].transform.position, holds[holdNum - 1].transform.position);
            angle = Mathf.Atan(Mathf.Abs(holds[holdNum].transform.position.x - holds[holdNum - 1].transform.position.x) / Mathf.Abs(holds[holdNum].transform.position.y - holds[holdNum - 1].transform.position.y));
            distance = Vector3.Distance(holds[holdNum].transform.position, holds[holdNum - 1].transform.position);
        }
        angle *= Mathf.Rad2Deg;
        //Debug.Log(angle.ToString("n2") + "°");

        //anim.SetTrigger("PullU");
        Launch();

        atHold = false;

        //transform.position = holds[holdNum].transform.position;
        //holdNum++;
    }

    private void GetAIWaitTime()
    {
        if (difficulty == Difficulty.Olympic)
        {
            aiT = Random.Range(config.aiOlympicMaxWait, config.aiOlympicMinWait);
        }
        else if (difficulty == Difficulty.Hard)
        {
            aiT = Random.Range(config.aiHardMinWait, config.aiHardMaxWait);
        }
        else if (difficulty == Difficulty.Medium)
        {
            aiT = Random.Range(config.aiMediumMinWait, config.aiMediumMaxWait);
        }
        else if (difficulty == Difficulty.Easy)
        {
            aiT = Random.Range(config.aiEasyMinWait, config.aiEasyMaxWait);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    private bool PressedWrongButton()
    {
        if (GetButton(holds[holdNum].button) == button1)
        {
            return Input.GetKeyDown(button2) || Input.GetKeyDown(button3) || Input.GetKeyDown(button4);
        }
        else if (GetButton(holds[holdNum].button) == button2)
        {
            return Input.GetKeyDown(button1) || Input.GetKeyDown(button3) || Input.GetKeyDown(button4);
        }
        else if (GetButton(holds[holdNum].button) == button3)
        {
            return Input.GetKeyDown(button2) || Input.GetKeyDown(button1) || Input.GetKeyDown(button4);
        }
        else if (GetButton(holds[holdNum].button) == button4)
        {
            return Input.GetKeyDown(button2) || Input.GetKeyDown(button3) || Input.GetKeyDown(button1);
        }
        else
        {
            return false;
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
