using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwimmingFreestylePlayer : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    private int playerID;
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;
    public Color32 colour = new Color32(79, 148, 231, 255);

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";

    [Header("Info")]
    public bool inWater = false;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer sprRen;
    [SerializeField]
    private SpriteRenderer normalSprRen;
    [SerializeField]
    private SpriteRenderer aboveWaterSprRen;
    [SerializeField]
    private SpriteRenderer belowWaterSprRen;
    private SwimmingFreestyleConfig config;
    private OlympicsController olympicsController;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private GameObject bigSplashParticles;
    [SerializeField]
    private GameObject smallSplashParticles;
    [SerializeField]
    private Sprite arrowUnlit;
    [SerializeField]
    private Sprite arrowLit;
    [SerializeField]
    private Sprite arrowPenalty;
    [SerializeField]
    private SpriteRenderer sprLeftArrow;
    [SerializeField]
    private SpriteRenderer sprRightArrow;
    [SerializeField]
    private SpriteRenderer sprUpArrow;
    [SerializeField]
    private GameObject arrows;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Text finishTimeText;
    [SerializeField]
    private SpriteRenderer sprBox;
    [SerializeField]
    private SpriteRenderer sprMedal;
    [SerializeField]
    private Sprite goldMedalSpr;
    [SerializeField]
    private Sprite silverMedalSpr;
    [SerializeField]
    private Sprite bronzeMedalSpr;

    private bool eligibleForRecord;

    private bool started = false;

    private float t = 0f;
    private Vector3 diveStartPoint;

    private float speed = 0f;
    private float direction = 1f;

    private int strokesThisBreath = 0;
    private bool litArrowThisBreath = false;

    private bool gliding = false;
    private int glideCount = 0;

    private bool rolling = false;

    private float arrowX;
    private float arrowSideSwapT = 0f;

    private float penaltyT = 0f;
    private bool penaltyFinished = true;
    private int penaltyCount = 0;

    private bool finished = false;
    private float finishTime;
    private int finishPosition;

    private bool resetBoxColour = false;

    private Vector3 finishTimeTextOriginalPos;

    [Header("AI Settings")]
    private float aiMultiplier = 1f;
    private float aiPressTime;
    private float aiGlideDistance;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<SwimmingFreestyleConfig>();

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

        arrowX = arrows.transform.localPosition.x;

        normalSprRen.material.SetColor("_TrunksColour", colour);
        aboveWaterSprRen.material.SetColor("_TrunksColour", colour);
        belowWaterSprRen.material.SetColor("_TrunksColour", colour);

        canvas.worldCamera = Camera.main;
        finishTimeText.text = "";

        if (isAI)
        {
            if (difficulty == Difficulty.Hard || difficulty == Difficulty.Olympic)
            {
                aiMultiplier = Random.Range(config.aiMinMultiplier, config.aiMaxMultiplier);
            }
            
            aiPressTime = aiMultiplier * AIGetStartTime(difficulty);
            aiGlideDistance = aiMultiplier * AIGetGlideDistance(difficulty);
        }

        float pb = PlayerPrefs.GetFloat("Swimming Freestyle PB " + playerName, OlympicsConfig.GetDefaultRecord("100m Freestyle"));
        Debug.Log(playerName + "'s PB: " + pb.ToString("n2") + "s");
        finishTimeText.text = pb.ToString("n2");
        finishTimeTextOriginalPos = finishTimeText.transform.localPosition;
        finishTimeText.transform.localPosition += Functions.Vector2To3(config.pbTimeTextOffset);

        ShowArrows(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSprites();

        if (config.startedCountdown)
        {
            if (!started)
            {
                if (!resetBoxColour)
                {
                    resetBoxColour = true;
                    sprBox.color = config.defaultBoxColour;

                    finishTimeText.text = "";
                    if (!isAI || !config.hideAIArrows)
                    {
                        ShowArrows(true);
                    }
                }

                bool start = false;
                if (!isAI)
                {
                    if (Input.GetKeyDown(button1) || Input.GetKeyDown(button2) || Input.GetKeyDown(button3) || Input.GetKeyDown(button4) || Input.GetKeyDown(button5))
                    {
                        if (!config.started)
                        {
                            config.FalseStart(playerName);
                            sprBox.color = config.falseStartBoxColour;
                            resetBoxColour = false;
                        }
                        else
                        {
                            start = true;
                        }
                    }
                }
                else if (config.started && config.raceTimeElapsed >= aiPressTime)
                {
                    start = true;
                }

                if (start)
                {
                    started = true;
                    inWater = false;
                    diveStartPoint = transform.position;
                    anim.SetTrigger("Dive");

                    Debug.Log(playerName + ", your reaction time: " + config.raceTimeElapsed.ToString("n5") + "s");
                }
            }
            else if (!inWater)
            {
                t += Time.deltaTime;

                transform.position = diveStartPoint + new Vector3(config.diveHorizontalSpeed * t, config.diveVerticalSpeed * t - 0.5f * t * t * config.gravity, 0f);

                if (transform.position.y <= diveStartPoint.y - config.diveHeight)
                {
                    inWater = true;
                    anim.SetTrigger("SwimArmReady");

                    BigSplash();

                    speed = config.startingSpeed;
                    t = config.timeBetweenStrokes;
                    strokesThisBreath = 0;
                    litArrowThisBreath = false;
                    UnlightArrows();

                    transform.position = new Vector3(transform.position.x, diveStartPoint.y - config.diveHeight, transform.position.z);

                    if (isAI)
                    {
                        aiPressTime = aiMultiplier * AIGetPressTime(difficulty);
                    }
                }
            }
            else if (inWater && !finished)
            {
                if (gliding)
                {
                    if (rolling)
                    {
                        t += Time.deltaTime;

                        if (t > config.timeBetweenStrokes)
                        {
                            gliding = false;
                            rolling = false;

                            anim.SetTrigger("SwimArmReady");

                            strokesThisBreath = 0;
                            litArrowThisBreath = false;
                            UnlightArrows();
                        }
                    }
                    speed -= Time.deltaTime * config.glideSpeedLossPerSecond;
                    speed = speed < config.minGlideSpeed ? config.minGlideSpeed : speed;
                }
                else if (!rolling)
                {
                    speed -= Time.deltaTime * config.speedLossPerSecond;
                    speed = speed < 0f ? 0f : speed;

                    t += Time.deltaTime;

                    if ((!isAI && Input.GetKeyDown(button5)) || (isAI && transform.position.x * direction >= config.poolEndX - aiGlideDistance))
                    {
                        gliding = true;
                        glideCount++;

                        aiGlideDistance = aiMultiplier * AIGetGlideDistance(difficulty);

                        speed = config.glideStartSpeed;
                        anim.SetTrigger("Glide");
                    }
                    if (t > config.timeBetweenStrokes)
                    {
                        if (!litArrowThisBreath)
                        {
                            if (strokesThisBreath >= config.strokesBeforeBreath)
                            {
                                sprUpArrow.sprite = arrowLit;
                            }
                            else if (strokesThisBreath % 2 == 0)
                            {
                                sprLeftArrow.sprite = arrowLit;
                            }
                            else
                            {
                                sprRightArrow.sprite = arrowLit;
                            }

                            litArrowThisBreath = true;
                        }
                    }

                    bool doneStroke = false;
                    bool incorrectStroke = false;

                    if (!isAI)
                    {
                        if (Input.GetKeyDown(button3))
                        {
                            if (t > config.timeBetweenStrokes && strokesThisBreath >= config.strokesBeforeBreath && penaltyT <= 0f)
                            {
                                doneStroke = true;
                                strokesThisBreath = -1;
                                anim.SetTrigger("Breathe" + Random.Range(1, 3));
                            }
                            else
                            {
                                incorrectStroke = true;
                            }

                        }
                        else if (Input.GetKeyDown(button1))
                        {
                            if (t > config.timeBetweenStrokes && strokesThisBreath < config.strokesBeforeBreath && strokesThisBreath % 2 == 0 && penaltyT <= 0f)
                            {
                                doneStroke = true;
                                anim.SetTrigger("SwimL");
                            }
                            else
                            {
                                incorrectStroke = true;
                            }
                        }
                        else if (Input.GetKeyDown(button2))
                        {
                            if (t > config.timeBetweenStrokes && strokesThisBreath < config.strokesBeforeBreath && strokesThisBreath % 2 == 1 && penaltyT <= 0f)
                            {
                                doneStroke = true;
                                anim.SetTrigger("SwimR");
                            }
                            else
                            {
                                incorrectStroke = true;
                            }
                        }
                    }
                    else
                    {
                        if (t - config.timeBetweenStrokes >= aiPressTime)
                        {
                            doneStroke = true;
                            aiPressTime = aiMultiplier * AIGetPressTime(difficulty);

                            if (t > config.timeBetweenStrokes && strokesThisBreath >= config.strokesBeforeBreath && penaltyT <= 0f)
                            {
                                strokesThisBreath = -1;
                                anim.SetTrigger("Breathe" + Random.Range(1, 3));
                            }
                            else if (t > config.timeBetweenStrokes && strokesThisBreath < config.strokesBeforeBreath && strokesThisBreath % 2 == 0 && penaltyT <= 0f)
                            {
                                anim.SetTrigger("SwimL");
                            }
                            else if (t > config.timeBetweenStrokes && strokesThisBreath < config.strokesBeforeBreath && strokesThisBreath % 2 == 1 && penaltyT <= 0f)
                            {
                                anim.SetTrigger("SwimR");
                            }
                        }
                    }

                    if (doneStroke)
                    {
                        speed = Mathf.Max(config.speedMinIncrease, config.speedMaxIncrease * Mathf.Sqrt(Mathf.Max(0f, 1f - (t - config.timeBetweenStrokes))));

                        t = 0f;
                        strokesThisBreath++;
                        litArrowThisBreath = false;
                        UnlightArrows();

                        SmallSplash();
                    }

                    if (incorrectStroke)
                    {
                        UnlightArrows();
                        penaltyT = config.incorrectStrokePenalty;
                        penaltyFinished = false;
                        penaltyCount++;

                        sprLeftArrow.sprite = arrowPenalty;
                        sprRightArrow.sprite = arrowPenalty;
                        sprUpArrow.sprite = arrowPenalty;
                        litArrowThisBreath = true;
                    }
                }

                penaltyT -= Time.deltaTime;
                if (penaltyT <= 0f && !penaltyFinished)
                {
                    penaltyFinished = true;
                    UnlightArrows();
                    litArrowThisBreath = false;
                    t = config.timeBetweenStrokes;
                }

                transform.position += new Vector3(speed * direction * Time.deltaTime, 0f, 0f);

                if (transform.position.x >= config.poolEndX && direction == 1f && !rolling)
                {
                    transform.position = new Vector3(config.poolEndX, transform.position.y, transform.position.z);
                    anim.SetTrigger("Roll");
                    gliding = false;
                    rolling = true;
                    speed = 0f;
                    UnlightArrows();
                    arrowSideSwapT = 0f;

                    SmallSplash();
                }

                if (transform.position.x <= -config.poolEndX && direction == -1f && !finished)
                {
                    transform.position = new Vector3(-config.poolEndX, transform.position.y, transform.position.z);
                    anim.SetTrigger("PullUp");
                    speed = 0f;
                    ShowArrows(false);

                    finished = true;
                    finishTime = config.raceTimeElapsed;
                    finishTimeText.text = finishTime.ToString("n2");
                    finishTimeText.transform.localPosition = finishTimeTextOriginalPos;

                    finishPosition = config.RecordTime(finishTime);

                    switch (finishPosition)
                    {
                        case 1: sprMedal.sprite = goldMedalSpr; break;
                        case 2: sprMedal.sprite = silverMedalSpr; break;
                        case 3: sprMedal.sprite = bronzeMedalSpr; break;
                        default: break;
                    }

                    BroadcastScore(finishTime);
                    CheckRecords(finishTime);

                    Debug.Log(playerName + ", you finished in: " + finishTime.ToString("n2") + "s.   You had " + penaltyCount + " penalties");
                }

                if (rolling)
                {
                    arrowSideSwapT += Time.deltaTime;
                    arrows.transform.localPosition = new Vector3(Mathf.Lerp(arrowX, -arrowX, arrowSideSwapT / config.arrowSideSwapDuration), arrows.transform.localPosition.y,
                        arrows.transform.localPosition.z);
                }
            }
        }
    }

    private void UpdateSprites()
    {
        if (inWater)
        {
            normalSprRen.enabled = false;
            aboveWaterSprRen.enabled = true;
            belowWaterSprRen.enabled = true;

            aboveWaterSprRen.sprite = sprRen.sprite;
            belowWaterSprRen.sprite = sprRen.sprite;
        }
        else
        {
            normalSprRen.enabled = true;
            aboveWaterSprRen.enabled = false;
            belowWaterSprRen.enabled = false;

            normalSprRen.sprite = sprRen.sprite;
        }
    }

    private void OnValidate()
    {
        UpdateSprites();
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

    private void UnlightArrows()
    {
        sprLeftArrow.sprite = arrowUnlit;
        sprRightArrow.sprite = arrowUnlit;
        sprUpArrow.sprite = arrowUnlit;
    }
    private void ShowArrows(bool show)
    {
        sprLeftArrow.enabled = show;
        sprRightArrow.enabled = show;
        sprUpArrow.enabled = show;
    }

    public void PushOffEnd()
    {
        direction = -1f;
        sprRen.flipX = true;
        normalSprRen.flipX = true;
        aboveWaterSprRen.flipX = true;
        belowWaterSprRen.flipX = true;

        gliding = true;
        glideCount++;

        speed = config.pushOffSpeed;

        t = config.timeBetweenStrokes - config.pushOffTimeUntilSwim;
    }


    private void BigSplash()
    {
        Instantiate(bigSplashParticles, transform.position - new Vector3(0f, 3f / 16f * transform.lossyScale.y, 0f), Quaternion.Euler(0f, 0f, -15f));
    }
    private void SmallSplash()
    {
        Instantiate(smallSplashParticles, transform.position - new Vector3(0f, 1f / 16f * transform.lossyScale.y, 0f), Quaternion.identity);
    }

    private float AIGetStartTime(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return Random.Range(config.aiMinStartTime, config.aiMaxStartTime);
            case Difficulty.Medium: return Random.Range(config.aiMinStartTime, config.aiMaxStartTime);
            case Difficulty.Hard: return Random.Range(config.aiMinStartTime, config.aiMaxStartTime);
            case Difficulty.Olympic: return Random.Range(config.aiMinStartTime, config.aiMaxStartTime);
            default: throw new System.Exception("Unimplemented difficulty: " + difficulty);
        }
    }

    private float AIGetPressTime(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return Random.Range(config.aiEasyMinPressTime, config.aiEasyMaxPressTime);
            case Difficulty.Medium: return Random.Range(config.aiMediumMinPressTime, config.aiMediumMaxPressTime);
            case Difficulty.Hard: return Random.Range(config.aiHardMinPressTime, config.aiHardMaxPressTime);
            case Difficulty.Olympic: return Random.Range(config.aiOlympicMinPressTime, config.aiOlympicMaxPressTime);
            default: throw new System.Exception("Unimplemented difficulty: " + difficulty);
        }
    }

    private float AIGetGlideDistance(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return Random.Range(config.aiMinGlideDistance, config.aiMaxGlideDistance);
            case Difficulty.Medium: return Random.Range(config.aiMinGlideDistance, config.aiMaxGlideDistance);
            case Difficulty.Hard: return Random.Range(config.aiMinGlideDistance, config.aiMaxGlideDistance);
            case Difficulty.Olympic: return Random.Range(config.aiMinGlideDistance, config.aiMaxGlideDistance);
            default: throw new System.Exception("Unimplemented difficulty: " + difficulty);
        }
    }

    private void CheckRecords(float finishTime)
    {
        if (eligibleForRecord)
        {
            if (finishTime < PlayerPrefs.GetFloat("Swimming Freestyle PB " + playerName, OlympicsConfig.GetDefaultRecord("100m Freestyle")))
            {
                Debug.Log(playerName + " got a new PB!");
                PlayerPrefs.SetFloat("Swimming Freestyle PB " + playerName, finishTime);
            }

            if (finishTime < PlayerPrefs.GetFloat("Swimming Freestyle Record", OlympicsConfig.GetDefaultRecord("100m Freestyle")))
            {
                PlayerPrefs.SetFloat("Swimming Freestyle Record", finishTime);
                Debug.Log("New Record!");
            }
        }
    }
}
