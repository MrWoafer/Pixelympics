using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrestlingReferee : MonoBehaviour
{
    [Header("Settings")]

    //[Header("Movement Settings")]
    //public float retreatSpeed = 1f;

    [Header("References")]
    private WrestlingConfig config;
    public Wrestler player1;
    public Wrestler player2;
    private Animator anim;
    public GameObject timerObj;
    private KarateTimerController timer;
    public GameObject crowdObj;
    private KarateCrowdController crowd;
    public Text centreTextbox;
    public Text centreTextboxDropShadow;
    private AudioManager audioManager;
    public Text roundTextbox;
    public GameObject shadow;
    private SpriteRenderer shadowSprRenderer;
    private SpriteRenderer spr;
    public GameObject startButton;
    private OlympicsController olympicsController;

    private int roundNum = 1;

    private bool retreating = false;
    private float retreatZ = 0f;
    private bool retreatIncrease = true;

    private bool startedMatch = false;
    private bool startedToStartMatch = false;

    private bool intervening = false;
    private bool doneInterveneAnimation = false;
    private bool startedToEndIntervene = false;
    private bool doneInterveneWaveHandsAnimation = false;

    private float stepDirection = 1f;
    private bool stepping = false;

    private bool startedTimeWarning = false;
    private bool canTimeWarnAgain = true;
    private bool startedTimeWarnAnimation = false;
    private bool startedTimePenaltyIntervene = false;

    private bool interveneGivePlayer1Penalty;
    private bool interveneGivePlayer2Penalty;

    private int hitsThisRally = 0;
    private bool[] rallyHitCountFlag = new bool[20];

    private bool goldenPoint = false;
    private bool roundEnded = false;

    private bool startedWalkIn = false;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<WrestlingConfig>();
        timer = timerObj.GetComponent<KarateTimerController>();
        crowd = crowdObj.GetComponent<KarateCrowdController>();
        anim = GetComponent<Animator>();

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        spr = GetComponent<SpriteRenderer>();
        shadowSprRenderer = shadow.GetComponent<SpriteRenderer>();

        if (config.developmentMode)
        {
            startedWalkIn = true;
            startedToStartMatch = true;
            StartMatch();
            player1.ShowCharacterCreation(false);
            player2.ShowCharacterCreation(false);
            transform.position = new Vector3(0f, transform.position.y, config.refDefaultZ);
        }
        else
        {
            //crowd.Clap();
            startedMatch = false;
            //transform.position = new Vector3(0f, transform.position.y, config.refCloseZ);
            transform.position = new Vector3(0f, transform.position.y, config.refDefaultZ);
        }
        //ShadowCorrection();
        SetCentreText("");
        //Debug.Log("Set it!");
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedWalkIn)
        {
            /*if (Input.GetKeyDown("space"))
            {
                StartWalkIn();
            }*/
        }
        else if (timer.GetTime() <= 0f && !goldenPoint && !roundEnded)
        {
            if (player1.GetPoints() == player2.GetPoints())
            {
                //Debug.Log("Golden Point!");
                /*goldenPoint = true;
                SetCentreText("Golden Point");
                CheckIfCrowdShouldCheer();
                Intervene();
                SFXGoldenPoint();*/
            }
            else
            {
                //EndRound();

                if (player1.GetPoints() > player2.GetPoints())
                {
                    Debug.Log("Player 1 wins!");

                    BroadcastScore(1f, player1.playerID);
                    BroadcastScore(0f, player2.playerID);
                }
                else
                {
                    Debug.Log("Player 2 wins!");

                    BroadcastScore(1f, player2.playerID);
                    BroadcastScore(0f, player1.playerID);
                }

                roundEnded = true;
            }
        }
        else if (!roundEnded)
        {
            if (goldenPoint && (player1.HasBeenHit() || player2.HasBeenHit()) && !intervening)
            {
                CheckIfCrowdShouldCheer();
                EndRound();
            }
            else if (!goldenPoint && (player1.HasBeenHit() || player2.HasBeenHit()) && (player1.ReadyToBeIntervened() && player2.ReadyToBeIntervened()) && config.refIntervenes && !intervening)
            {
                CheckIfCrowdShouldCheer();
                Intervene();
                SFXPoint();
            }
            else if (retreating)
            {
                Retreat();
            }
            else if (!startedMatch)
            {
                if (player1.HasBowed() && player1.IsReady() && !startedToStartMatch && IsAtXCoord(0f))
                {
                    startedToStartMatch = true;
                    anim.SetTrigger("StartMatch");
                    SFXGo();
                    crowd.Idle();
                }
            }
            else if (intervening && !startedToEndIntervene && player1.transform.position.x == -1.4f && player2.transform.position.x == 1.4f && doneInterveneWaveHandsAnimation && player1.IsReady() && player2.IsReady())
            {
                crowd.Idle();
                startedToEndIntervene = true;
                anim.SetTrigger("ResumeMatch");
                SFXGo(true);
                player1.StopHoldMoves();
                player2.StopHoldMoves();

                /*player1.setReadyPlayAnimFlag = true;
                player2.setReadyPlayAnimFlag = true;
                player1.SetReady();
                player2.SetReady();*/
            }
            /*else if (!IsBetweenPlayers())
            {
                if (!stepping)
                {
                    stepping = true;
                    anim.ResetTrigger("StepEnd");
                    anim.SetTrigger("StepStart");
                }

                float x = (player1.transform.position.x + player2.transform.position.x) / 2f;

                if (Mathf.Abs(transform.position.x - x) < config.refStepEndRadius)
                {
                    transform.position += new Vector3(x - transform.position.x, 0f, 0f);
                    stepDirection = 1f;
                    transform.localScale = new Vector3(stepDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    stepping = false;
                    anim.SetTrigger("StepEnd");
                }
                else if (transform.position.x < x)
                {
                    stepDirection = 1f;
                    transform.localScale = new Vector3(stepDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    stepDirection = 1f;
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }*/
            else if (!ShouldTimeWarn() && startedTimeWarning)
            {
                canTimeWarnAgain = true;
                startedTimeWarning = false;
                RetreatTo(config.refDefaultZ);
            }
            else if (ShouldTimeWarn() && !intervening && !startedTimeWarning && canTimeWarnAgain)
            {
                startedTimeWarnAnimation = false;
                startedTimePenaltyIntervene = false;
                canTimeWarnAgain = false;
                startedTimeWarning = true;
                RetreatTo(config.refTimeWarningZ);
            }
            else if (startedTimeWarning && transform.position.z == config.refTimeWarningZ && !startedTimeWarnAnimation)
            {
                startedTimeWarnAnimation = true;
                anim.SetTrigger("TimeWarning");
                SFXTimeWarning();
            }
            else if (!canTimeWarnAgain && ShouldTimePenalise() && !startedTimePenaltyIntervene)
            {
                startedTimePenaltyIntervene = true;
                /// Subtract a small amount from the penaltyTime so that players can both receive a penalty simultaneoously without problems of only one getting it
                //Intervene(player1.GetTimeSinceLastProperAttack() > config.timePenaltyTime - 0.1f, player2.GetTimeSinceLastProperAttack() > config.timePenaltyTime - 0.1f);
                Intervene(player1.GetTimeSinceLastProperAttack() > config.timePenaltyTime - 0.3f, player2.GetTimeSinceLastProperAttack() > config.timePenaltyTime - 0.3f);
                SFXPenalty();
            }

            MoveToBetweenPlayers();
            //transform.position += new Vector3((player1.PositionWithoutExtraMovement() + player2.PositionWithoutExtraMovement()) / 2f - transform.position.x, 0f, 0f);
        }
        else
        {
            if (retreating)
            {
                Retreat();
            }
            if (player1.transform.position.x == -1.4f && player2.transform.position.x == 1.4f)
            {
                if (PlayerHasWon())
                {
                    if (player1.GetRoundWins() >= 2)
                    {
                        SetCentreText(player1.playerName.ToLower() + " has won!");
                    }
                    else
                    {
                        SetCentreText(player2.playerName.ToLower() + " has won!");
                    }
                }
                else if (Input.GetKeyDown("space"))
                {
                    NextRound();
                }
            }

            MoveToBetweenPlayers();
        }

        UpdateShadow();
    }

    private void RetreatTo(float z)
    {
        retreating = true;
        retreatZ = z;
        retreatIncrease = transform.position.z < retreatZ;
        anim.ResetTrigger("WalkEnd");
        anim.SetTrigger("WalkStart");
    }

    /*private void ShadowCorrection()
    {
        if (isShadow)
        {
            transform.position += new Vector3(0f, 0f, 1.25f);
        }
    }*/

    private void StartMatch()
    {
        ResetRallyHitCount();

        player1.ResetTimeSinceLastProperAttack();
        player2.ResetTimeSinceLastProperAttack();

        startedMatch = true;
        timer.StartTimer(config.roundDuration);
        player1.ClearQueuedButton();
        player2.ClearQueuedButton();

        player1.SetDirection(1f);
        player2.SetDirection(-1f);
    }
    private void StartMatchAnimEnd()
    {
        RetreatTo(config.refDefaultZ);
    }

    public bool HasStartedMatch()
    {
        return startedMatch;
    }

    public void Intervene(bool givePlayer1Penalty = false, bool givePlayer2Penalty = false)
    {
        //player1.anim.SetTrigger("Ready");
        //player2.anim.SetTrigger("Ready");
        player1.SetReady();
        player2.SetReady();

        interveneGivePlayer1Penalty = givePlayer1Penalty;
        interveneGivePlayer2Penalty = givePlayer2Penalty;

        intervening = true;
        StartIntervene();
    }
    private void StartIntervene()
    {
        RetreatTo(config.refCloseZ);
        doneInterveneAnimation = false;
        startedToEndIntervene = false;
        doneInterveneWaveHandsAnimation = false;

        startedTimeWarnAnimation = false;
        canTimeWarnAgain = false;
        startedTimeWarning = false;
    }
    public bool HasIntervened()
    {
        return intervening || roundEnded;
    }
    private void InterveneEnd()
    {
        ResetRallyHitCount();

        player1.ResetTimeSinceLastProperAttack();
        player2.ResetTimeSinceLastProperAttack();

        intervening = false;
        player1.ClearQueuedButton();
        player2.ClearQueuedButton();

        player1.SetDirection(1f);
        player2.SetDirection(-1f);
    }
    private void ResumeMatchAnimEnd()
    {
        RetreatTo(config.refDefaultZ);
    }
    private void InterveneWaveHandsAnimationEnd()
    {
        doneInterveneWaveHandsAnimation = true;
        InterveneAction();
    }

    public void TranslatePixels(float pixels)
    {
        transform.position += new Vector3(stepDirection * pixels / 16f * transform.localScale.x, 0f, 0f);
    }

    private bool IsBetweenPlayers()
    {
        return Mathf.Abs(transform.position.x - (player1.transform.position.x + player2.transform.position.x) / 2f) < 0.01f;
        //return IsAtXCoord((player1.PositionWithoutExtraMovement() + player2.PositionWithoutExtraMovement()) / 2f);
    }
    private bool IsAtXCoord(float x)
    {
        return Mathf.Abs(transform.position.x - x) < 0.01f;
    }

    private void InterveneAction()
    {
        player1.WalkTo(-1.4f);
        player2.WalkTo(1.4f);

        if (player1.GetPenalties() == -1)
        {
            //player1.ResetPenaltyDisplay();
            player1.AddPenalties(1);
            player1.AddPoints(-config.penaltyPointLoss, true);
        }
        if (player2.GetPenalties() == -1)
        {
            //player2.ResetPenaltyDisplay();
            player2.AddPenalties(1);
            player2.AddPoints(-config.penaltyPointLoss, true);
        }

        if (interveneGivePlayer1Penalty)
        {
            player1.AddPenalties(1);
            interveneGivePlayer1Penalty = false;
        }
        if (interveneGivePlayer2Penalty)
        {
            player2.AddPenalties(1);
            interveneGivePlayer2Penalty = false;
        }
        player1.ResetTimeSinceLastProperAttack();
        player2.ResetTimeSinceLastProperAttack();
        startedTimePenaltyIntervene = false;
        canTimeWarnAgain = true;

        player1.ResetHasBeenHit();
        player2.ResetHasBeenHit();

        if (roundEnded)
        {
            SetCentreText("");
            AwardRoundWin();
        }
    }

    private bool ShouldTimeWarn()
    {
        return player1.GetTimeSinceLastProperAttack() > config.timeWarningTime || player2.GetTimeSinceLastProperAttack() > config.timeWarningTime;
    }

    private bool ShouldTimePenalise()
    {
        return player1.GetTimeSinceLastProperAttack() > config.timePenaltyTime || player2.GetTimeSinceLastProperAttack() > config.timePenaltyTime;
    }

    private void TimeWarnAnimationEnded()
    {
        startedTimeWarning = false;
        RetreatTo(config.refDefaultZ);
    }

    public void CountHit()
    {
        hitsThisRally++;

        if (hitsThisRally >= 3 && !rallyHitCountFlag[0])
        {
            crowd.Shock(0.05f);
            rallyHitCountFlag[0] = true;
        }
        if (hitsThisRally >= 5 && !rallyHitCountFlag[1])
        {
            crowd.Shock(0.1f);
            rallyHitCountFlag[1] = true;
        }
        if (hitsThisRally >= 7 && !rallyHitCountFlag[2])
        {
            crowd.Shock(0.1f, 0.1f);
            rallyHitCountFlag[2] = true;
        }
        if (hitsThisRally >= 8 && !rallyHitCountFlag[3])
        {
            crowd.Shock(0.1f, 0.2f);
            rallyHitCountFlag[3] = true;
        }
        if (hitsThisRally >= 9 && !rallyHitCountFlag[4])
        {
            crowd.Shock(0.1f, 0.3f);
            rallyHitCountFlag[4] = true;
        }
        if (hitsThisRally >= 10 && !rallyHitCountFlag[5])
        {
            crowd.Shock(0.1f, 0.4f);
            rallyHitCountFlag[5] = true;
        }
        else if (hitsThisRally >= 10)
        {
            crowd.Shock(0.1f, 0.5f);
        }
    }
    private void ResetRallyHitCount()
    {
        hitsThisRally = 0;
        for (int i = 0; i < rallyHitCountFlag.Length; i++)
        {
            rallyHitCountFlag[i] = false;
        }
    }

    private void CheckIfCrowdShouldCheer()
    {
        if (goldenPoint)
        {
            crowd.Cheer(1f);
        }
        else if (hitsThisRally >= 7)
        {
            crowd.Cheer(1f);
        }
        else if (hitsThisRally >= 5)
        {
            crowd.Cheer(0.75f);
        }
        else if (hitsThisRally >= 3)
        {
            crowd.Cheer(0.5f);
        }
        else
        {
            crowd.Clap();
        }
    }

    private void SetCentreText(string text)
    {
        centreTextbox.text = text;
        centreTextboxDropShadow.text = text;
    }

    private void EndRound()
    {
        //SetCentreText("");
        crowd.Cheer(1f);
        roundEnded = true;
        SFXStop();
        Intervene();
    }

    private void Retreat()
    {
        bool finished = false;
        if (retreatIncrease)
        {
            transform.position += new Vector3(0f, 0f, config.refRetreatSpeed * Time.deltaTime);
            if (transform.position.z >= retreatZ)
            {
                finished = true;
            }
        }
        else
        {
            transform.position -= new Vector3(0f, 0f, config.refRetreatSpeed * Time.deltaTime);
            if (transform.position.z <= retreatZ)
            {
                finished = true;
            }
        }
        if (finished)
        {
            transform.position += new Vector3(0f, 0f, retreatZ - transform.position.z);
            retreating = false;
            anim.SetTrigger("WalkEnd");
            //ShadowCorrection();

            if (intervening && !doneInterveneAnimation)
            {
                doneInterveneAnimation = true;
                anim.SetTrigger("Intervene");
            }
        }
    }

    private void SFXGo(bool quickGo = false)
    {
        if (quickGo)
        {
            int index = Random.Range(0, config.refQuickGoNum);
            audioManager.Play("RefQuickGo" + index);
        }
        else
        {
            int index = Random.Range(0, config.refGoNum);
            audioManager.Play("RefGo" + index);
        }
    }
    private void SFXPoint()
    {
        int index = Random.Range(0, config.refPointNum);
        audioManager.Play("RefPoint" + index);
    }
    public void SFXPenalty()
    {
        int index = Random.Range(0, config.refPenaltyNum);
        audioManager.Play("RefPenalty" + index);
    }
    private void SFXTimeWarning()
    {
        int index = Random.Range(0, config.refTimeWarningNum);
        audioManager.Play("RefTimeWarning" + index);
    }
    private void SFXGoldenPoint()
    {
        int index = Random.Range(0, config.refGoldenPointNum);
        audioManager.Play("RefGoldenPoint" + index);
    }
    private void SFXStop()
    {
        int index = Random.Range(0, config.refStopNum);
        audioManager.Play("RefStop" + index);
    }

    private void SetRoundText(string text)
    {
        roundTextbox.text = text;
    }

    private void AwardRoundWin()
    {
        if (player1.GetPoints() > player2.GetPoints())
        {
            player1.SetRoundCircle(roundNum, 2);
            player1.WinRound();
            player2.SetRoundCircle(roundNum, 1);
        }
        else if (player1.GetPoints() < player2.GetPoints())
        {
            player1.SetRoundCircle(roundNum, 1);
            player2.SetRoundCircle(roundNum, 2);
            player2.WinRound();
        }
    }

    private void NextRound()
    {
        roundNum++;
        roundEnded = false;

        SetRoundText("round " + roundNum);
        SetCentreText("");

        retreating = false;
        retreatZ = 0f;
        retreatIncrease = true;

        intervening = false;
        doneInterveneAnimation = false;
        startedToEndIntervene = false;
        doneInterveneWaveHandsAnimation = false;

        stepDirection = 1f;
        stepping = false;

        startedMatch = false;

        startedTimeWarning = false;
        canTimeWarnAgain = true;
        startedTimeWarnAnimation = false;
        startedTimePenaltyIntervene = false;

        ResetRallyHitCount();

        goldenPoint = false;

        timer.SetTimer(config.roundDuration);

        player1.ResetTimeSinceLastProperAttack();
        player2.ResetTimeSinceLastProperAttack();

        player1.AddPenalties(-player1.GetPenalties());
        player2.AddPenalties(-player2.GetPenalties());

        player1.AddPoints(-player1.GetPoints());
        player2.AddPoints(-player2.GetPoints());

        startedToStartMatch = true;
        anim.SetTrigger("StartMatch");
        SFXGo();
        crowd.Idle();
    }

    private void MoveToBetweenPlayers()
    {
        anim.ResetTrigger("StepStart");
        if (!IsBetweenPlayers() && anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle")
        {
            anim.SetTrigger("StepStart");
        }

        transform.position += new Vector3((player1.transform.position.x + player2.transform.position.x) / 2f - transform.position.x, 0f, 0f);
    }

    private bool PlayerHasWon()
    {
        return player1.GetRoundWins() >= 2 || player2.GetRoundWins() >= 2;
    }

    public bool HasStartedWalkIn()
    {
        return startedWalkIn;
    }
    public void StartWalkIn()
    {
        if (!startedWalkIn)
        {
            startButton.SetActive(false);
            startedWalkIn = true;

            player1.ShowCharacterCreation(false);
            player2.ShowCharacterCreation(false);

            //transform.position = new Vector3(0f, 0f, config.refCloseZ);
            RetreatTo(config.refCloseZ);

            player1.TranslatePixels(-56f);
            player1.WalkTo(-1.4f * player1.direction);

            player2.TranslatePixels(-56f);
            player2.WalkTo(-1.4f * player2.direction);

            crowd.Clap();
        }
    }

    private void UpdateShadow()
    {
        shadowSprRenderer.sprite = spr.sprite;
        shadow.transform.position = new Vector3(transform.position.x, -1.48f, transform.position.z + 1.25f);
        shadowSprRenderer.color = new Color(0f, 0f, 0f, Mathf.Clamp(0.5f - Mathf.Abs(transform.position.z) / 15f, 0f, 1f));
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score, int playerID)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score, playerID);
        }
    }
}
