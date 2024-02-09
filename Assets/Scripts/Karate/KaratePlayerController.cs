using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KaratePlayerController : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    public int playerNum = 0;
    [HideInInspector]
    public int playerID;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;
    public bool isShadow = false;

    [Header("Controls")]
    public float direction = 1f;
    private float originalDirection;
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";
    public string button6 = "q";
    public string button7 = "r";
    private string originalButton1;
    private string originalButton2;

    [Header("Combo Settings")]
    public bool enableCombos = true;
    public bool enableSpinHookKick = true;
    public bool enableKnee = true;
    public bool enableFrontKick = true;
    public bool enableHighSideKick = true;
    public bool enableElbow = true;

    [Header("Grade Settings")]
    //public Color beltColour = Color.black;
    public int grade = 0;

    [Header("Cosmetic Settings")]
    public Color giColour = Color.white;
    public Color skinColour = new Color(253f, 218f, 191f);
    public Color hairColour = new Color(250f, 225f, 128f);
    private string currentlyEditing = "gi";

    [Header("Hit Settings")]
    public GameObject roundKickHitPoint;
    public GameObject punchHitPoint;
    public GameObject sideKickHitPoint;
    public GameObject roundKickToSpinFrontKickHitPoint;
    public GameObject punchToKneeGrabHitPoint;
    public GameObject punchToKneeHitHitPoint;
    public GameObject punchToFrontKickHitPoint;
    public GameObject sideKickToHighSideKickHitPoint;
    public GameObject sideKickToElbowHitPoint;

    [Header("Shadow")]
    public GameObject shadow;
    private SpriteRenderer shadowSprRenderer;

    [Header("Sounds")]
    public AudioSource whoosh;
    public AudioSource block;

    [Header("References")]
    public GameObject configObj;
    private KarateConfig config;
    public Animator anim;
    public ParticleSystem hitParticles;
    public GameObject hitEffect;
    private SpriteRenderer sprRenderer;
    public Text pointsTextbox;
    public Text nameTextbox;
    public GameObject penaltyBox1Off;
    public GameObject penaltyBox1On;
    public GameObject penaltyBox2Off;
    public GameObject penaltyBox2On;
    public GameObject penaltyBox3Off;
    public GameObject penaltyBox3On;
    public GameObject refereeObj;
    private KarateRefereeController referee;
    public GameObject timerObj;
    private KarateTimerController timer;
    public GameObject opponentObj;
    private KaratePlayerController opponent;
    private AudioManager audioManager;
    public Sprite roundNotPlayed;
    public Sprite roundWon;
    public Sprite roundLost;
    public Image[] roundCircles;
    private KarateAI ai;
    public ColourPicker colourPicker;
    public InputField nameInputBox;
    public GameObject characterCreationCanvas;
    public GameObject characterCreationPhysicalCanvas;

    private bool isReady = true;
    private bool blocking = false;
    private bool stunned = false;
    private float stunTimer = 0f;
    private bool ducking = false;
    private bool walking = false;
    private float walkTarget = 0f;
    private bool bowed = false;

    private int points = 0;
    private int penalties = 0;

    private float movementOffset = 0f;
    private float movementOffsetY = 0f;

    // 0 - both feet on mat; 1 - one foot off mat; 2 - both feet off mat
    private int steppedOffMat = 0;

    private float timeSinceProperAttack = 0f;

    private int roundKicks = 0;
    private int roundKickHits = 0;
    private int punches = 0;
    private int punchHits = 0;
    private int sideKicks = 0;
    private int sideKickHits = 0;
    private int spinHookKicks = 0;
    private int spinHookKickHits = 0;
    private int blockCount = 0;
    private int blocksSuccessful = 0;
    private int ducks = 0;
    private int ducksSuccessful = 0;
    private int frontKicks = 0;
    private int frontKickHits = 0;
    private int highSideKicks = 0;
    private int highSideKickHits = 0;
    private int elbows = 0;
    private int elbowHits = 0;
    private int knees = 0;
    private int kneeHits = 0;

    private bool timerEnded = false;

    private string queuedButton = "";
    private float queuedButtonTimer = 0f;

    private bool roundKickComboWindowOpen = false;
    private bool punchComboWindowOpen = false;
    private bool sideKickComboWindowOpen = false;
    private bool hitLanded = false;

    private bool hasBeenHit = false;

    private int lastHitGruntIndex = -1;
    private int lastHurtGruntIndex = -1;

    private int roundsWon = 0;

    private bool isInSwingPhase = false;

    [HideInInspector]
    public bool setReadyPlayAnimFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        originalButton1 = button1;
        originalButton2 = button2;

        config = configObj.GetComponent<KarateConfig>();
        referee = refereeObj.GetComponent<KarateRefereeController>();
        timer = timerObj.GetComponent<KarateTimerController>();
        opponent = opponentObj.GetComponent<KaratePlayerController>();

        shadowSprRenderer = shadow.GetComponent<SpriteRenderer>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        ai = GetComponent<KarateAI>();

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

        sprRenderer = GetComponent<SpriteRenderer>();

        if (!isShadow)
        {
            nameTextbox.text = playerName.ToLower();
        }

        AddPenalties(0);

        if (!config.developmentMode)
        {
            //TranslatePixels(-56);
            //WalkTo(-1.4f * direction);
        }
        else
        {
            bowed = true;
        }

        originalDirection = direction;

        ShowCharacterCreation(true);
        LoadPlayer(playerName);
        SetBeltColour(grade);
        //UpdateAppearance();

        for (int i = 1; i <= 3; i++)
        {
            SetRoundCircle(i, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAppearance();

        timeSinceProperAttack += Time.deltaTime;

        queuedButtonTimer -= Time.deltaTime;
        if (queuedButtonTimer <= 0f && queuedButton != "")
        {
            queuedButton = "";
        }

        if (!stunned)
        {
            if (walking)
            {
                if (Mathf.Abs(transform.position.x - walkTarget) < config.walkEndRadius)
                {
                    direction = originalDirection;
                    transform.localScale = new Vector3(3f * direction, 3f, 3f);
                    walking = false;
                    transform.position += new Vector3(walkTarget - transform.position.x, 0f, 0f);
                    if (bowed)
                    {
                        anim.SetTrigger("WalkEnd");
                    }
                    else
                    {
                        isReady = false;
                        bowed = true;
                        anim.SetTrigger("Bow");
                    }
                }
                else if (transform.position.x > walkTarget)
                {
                    //direction = -1f;
                    transform.localScale = new Vector3(-3f, 3f, 3f);
                }
                else if (transform.position.x < walkTarget)
                {
                    //direction = 1f;
                    transform.localScale = new Vector3(3f, 3f, 3f);
                }
            }
            else if (referee.HasStartedMatch() && !referee.HasIntervened())
            {
                /// Is in READY stance
            //if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Ready")
                if (isReady)
                {
                    if (Input.GetKeyDown(button3) || queuedButton == button3)
                    {
                        RoundKick();
                    }
                    if (Input.GetKeyDown(button5) || queuedButton == button5)
                    {
                        Punch();
                    }
                    if (Input.GetKeyDown(button1) || queuedButton == button1)
                    {
                        isReady = false;
                        anim.SetTrigger("JumpBack");
                    }
                    //if ((Input.GetKeyDown(button2) || queuedButton == button2) && Mathf.Abs(transform.position.x - opponent.transform.position.x) > config.minDistanceBetweenPlayers)
                    if (Input.GetKeyDown(button2) || queuedButton == button2)
                    {
                        isReady = false;
                        anim.SetTrigger("JumpForward");
                    }
                    if (Input.GetKeyDown(button6) || queuedButton == button6)
                    {
                        BlockStart();
                    }
                    if (Input.GetKeyDown(button4) || queuedButton == button4)
                    {
                        DuckStart();
                    }
                    if (Input.GetKeyDown(button7) || queuedButton == button7)
                    {
                        SideKick();
                    }

                    SetQueuedButton("");
                }
                /// Is NOT in READY stance
                else
                {
                    if (ducking)
                    {
                        if (Input.GetKeyUp(button4) && ducking)
                        {
                            DuckEnd();
                        }
                    }
                    else
                    {
                        if (Input.GetKeyUp(button6) && blocking)
                        {
                            BlockEnd();
                        }
                    }

                    ///Button queueing
                    if (Input.GetKeyDown(button3))
                    {
                        if (roundKickComboWindowOpen && enableCombos && enableSpinHookKick)
                        {
                            RoundKickToSpinFrontKick();
                        }
                        else if (punchComboWindowOpen && enableCombos && enableKnee)
                        {
                            PunchToKneeGrab();
                        }
                        else if (sideKickComboWindowOpen && enableCombos && enableHighSideKick)
                        {
                            SideKickToHighSideKick();
                        }
                        else
                        {
                            SetQueuedButton(button3);
                        }
                    }
                    if (Input.GetKeyDown(button5))
                    {
                        if (punchComboWindowOpen && enableCombos && enableFrontKick)
                        {
                            PunchToFrontKick();
                        }
                        else if (sideKickComboWindowOpen && enableCombos && enableElbow)
                        {
                            SideKickToElbow();
                        }
                        else
                        {
                            SetQueuedButton(button5);
                        }
                    }
                    if (Input.GetKeyDown(button1))
                    {
                        //SetQueuedButton(button1);
                    }
                    if (Input.GetKeyDown(button2))
                    {
                        //SetQueuedButton(button2);
                    }
                    if (Input.GetKeyDown(button6))
                    {
                        SetQueuedButton(button6);
                    }
                    if (Input.GetKeyUp(button6))
                    {
                        SetQueuedButton("");
                    }
                    if (Input.GetKeyDown(button4))
                    {
                        SetQueuedButton(button4);
                    }
                    if (Input.GetKeyUp(button4))
                    {
                        SetQueuedButton("");
                    }
                    if (Input.GetKeyDown(button7))
                    {
                        if (sideKickComboWindowOpen)
                        {
                            
                        }
                        else
                        {
                            SetQueuedButton(button7);
                        }
                    }
                }
            }
        }
        else
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                stunned = false;
                anim.SetTrigger("StunEnd");
            }
        }

        /// Make attacking players be seen in front of non-attacking players
        if (!isShadow)
        {
            if (isReady || ducking || blocking)
            {
                sprRenderer.sortingOrder = 0;
            }
            else
            {
                sprRenderer.sortingOrder = 1;
            }
        }

        /// Set belt colour
        //sprRenderer.sprite = SpriteEdit.ReplaceColour(SpriteEdit.CopySprite(sprRenderer.sprite), Color.black, beltColour);

        /// Check if player is out
        if (bowed)
        {
            if ((direction == 1f && transform.position.x < -config.oneFootOffX) || (direction == -1f && transform.position.x > config.oneFootOffX))
            {
                if (((direction == 1f && transform.position.x < -config.bothFeetOffX) || (direction == -1f && transform.position.x > config.bothFeetOffX)) && steppedOffMat < 2)
                {
                    steppedOffMat = 2;
                    //AddPenalties(1);
                    AddPenalties(2);
                }
                else if (steppedOffMat == 0)
                {
                    steppedOffMat = 1;
                    AddPenalties(1);
                }
            }
            else
            {
                steppedOffMat = 0;
            }
        }

        if (timer.GetTime() <= 0f && !timerEnded)
        {
            timerEnded = true;

            Debug.Log(playerName + ", you got " + points + " points");
            Debug.Log(playerName + ", you used round kick " + roundKicks + " times, with " + roundKickHits + " landing");
            Debug.Log(playerName + ", you used punch " + punches + " times, with " + punchHits + " landing");
            Debug.Log(playerName + ", you used side kick " + sideKicks + " times, with " + sideKickHits + " landing");
            Debug.Log(playerName + ", you used spin hook kick " + spinHookKicks + " times, with " + spinHookKickHits + " landing");
            Debug.Log(playerName + ", you used knee " + knees + " times, with " + kneeHits + " landing");
            Debug.Log(playerName + ", you used front kick " + frontKicks + " times, with " + frontKickHits + " landing");
            Debug.Log(playerName + ", you used high side kick " + highSideKicks + " times, with " + highSideKickHits + " landing");
            Debug.Log(playerName + ", you used elbow " + elbows + " times, with " + elbowHits + " landing");
            Debug.Log(playerName + ", you used block " + blockCount + " times, with " + blocksSuccessful + " successfully blocking an attack");
            Debug.Log(playerName + ", you used duck " + ducks + " times, with " + ducksSuccessful + " successfully ducking an attack");
        }

        if (isReady && config.turnWhenPastOpponent)
        {
            if (transform.position.x < opponent.transform.position.x && transform.localScale.x < 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                string temp = button1;
                button1 = button2;
                button2 = temp;
                direction = 1f;
            }
            if (transform.position.x > opponent.transform.position.x && transform.localScale.x > 0f)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                string temp = button1;
                button1 = button2;
                button2 = temp;
                direction = -1f;
            }
        }

        UpdateShadow();

        //Debug.Log(playerName + ": " + MovementsToOpponent());
    }

    public void BlockStart()
    {
        blockCount++;
        anim.ResetTrigger("BlockEnd");
        blocking = true;
        isReady = false;
        anim.SetTrigger("BlockStart");
    }
    public void BlockEnd()
    {
        blocking = false;
        isReady = true;
        anim.SetTrigger("BlockEnd");
        anim.ResetTrigger("BlockStart");
        SetQueuedButton("");
    }

    public void DuckStart()
    {
        ducks++;
        anim.ResetTrigger("DuckEnd");
        ducking = true;
        isReady = false;
        anim.SetTrigger("DuckStart");
    }
    public void DuckEnd()
    {
        ducking = false;
        isReady = true;
        anim.SetTrigger("DuckEnd");
        anim.ResetTrigger("DuckStart");
        SetQueuedButton("");
    }

    public void RoundKick()
    {
        isInSwingPhase = true;
        isReady = false;
        anim.SetTrigger("RoundKick");
        CheckIfWouldHaveHit(roundKickHitPoint.transform.position + new Vector3(PixelsToDistance(4f), 0f, 0f));
        roundKicks++;
    }
    private void RoundKickParticles()
    {
        //ParticleSystem hitPart = Instantiate(hitParticles, roundKickHitPoint.transform.position, Quaternion.identity);
        //hitPart.Play();
        //Destroy(hitPart.gameObject, 2f);
        isInSwingPhase = false;
        DoHit(roundKickHitPoint.transform.position, "RoundKick");
    }
    private void RoundKickComboWindow(int state)
    {
        roundKickComboWindowOpen = state == 1 ? true : false;
    }

    public void Punch()
    {
        isInSwingPhase = true;
        isReady = false;
        anim.SetTrigger("Punch");
        CheckIfWouldHaveHit(punchHitPoint.transform.position);
        punches++;
    }
    private void PunchParticles()
    {
        //ParticleSystem hitPart = Instantiate(hitParticles, punchHitPoint.transform.position, Quaternion.identity);
        //hitPart.Play();
        //Destroy(hitPart.gameObject, 2f);
        isInSwingPhase = false;
        DoHit(punchHitPoint.transform.position, "Punch");
    }
    private void PunchComboWindow(int state)
    {
        punchComboWindowOpen = state == 1 ? true : false;
    }

    public void SideKick()
    {
        isInSwingPhase = true;
        isReady = false;
        anim.SetTrigger("SideKick");
        CheckIfWouldHaveHit(sideKickHitPoint.transform.position);
        sideKicks++;
    }
    private void SideKickParticles()
    {
        isInSwingPhase = false;
        DoHit(sideKickHitPoint.transform.position, "SideKick");
    }
    private void SideKickComboWindow(int state)
    {
        sideKickComboWindowOpen = state == 1 ? true : false;
    }

    public void RoundKickToSpinFrontKick()
    {
        isInSwingPhase = true;
        spinHookKicks++;
        hitLanded = false;
        roundKickComboWindowOpen = false;
        isReady = false;
        anim.SetTrigger("RoundKickToSpinFrontKick");
        CheckIfWouldHaveHit(roundKickToSpinFrontKickHitPoint.transform.position);
    }
    private void RoundKickToSpinFrontKickParticles()
    {
        isInSwingPhase = false;
        movementOffset = -1f;
        DoHit(roundKickToSpinFrontKickHitPoint.transform.position, "RoundKickToSpinFrontKick");
    }

    public void PunchToFrontKick()
    {
        isInSwingPhase = true;
        frontKicks++;
        hitLanded = false;
        punchComboWindowOpen = false;
        isReady = false;
        anim.SetTrigger("PunchToFrontKick");
        CheckIfWouldHaveHit(punchToFrontKickHitPoint.transform.position);
    }
    private void PunchToFrontKickParticles()
    {
        isInSwingPhase = false;
        DoHit(punchToFrontKickHitPoint.transform.position, "PunchToFrontKick");
    }

    public void SideKickToHighSideKick()
    {
        isInSwingPhase = true;
        highSideKicks++;
        hitLanded = false;
        sideKickComboWindowOpen = false;
        isReady = false;
        anim.SetTrigger("SideKickToHighSideKick");
        CheckIfWouldHaveHit(sideKickToHighSideKickHitPoint.transform.position);
    }
    private void SideKickToHighSideKickParticles()
    {
        isInSwingPhase = false;
        DoHit(sideKickToHighSideKickHitPoint.transform.position, "SideKickToHighSideKick");
    }

    public void SideKickToElbow()
    {
        isInSwingPhase = true;
        elbows++;
        hitLanded = false;
        sideKickComboWindowOpen = false;
        isReady = false;
        anim.SetTrigger("SideKickToElbow");
        CheckIfWouldHaveHit(sideKickToElbowHitPoint.transform.position);
    }
    private void SideKickToElbowParticles()
    {
        isInSwingPhase = false;
        DoHit(sideKickToElbowHitPoint.transform.position, "SideKickToElbow");
    }

    public void PunchToKneeGrab()
    {
        isInSwingPhase = true;
        knees++;
        hitLanded = false;
        punchComboWindowOpen = false;
        isReady = false;
        anim.SetTrigger("PunchToKneeGrab");
        CheckIfWouldHaveHit(punchToKneeGrabHitPoint.transform.position);
    }
    private void PunchToKneeGrabParticles()
    {
        isInSwingPhase = false;
        if (WouldHaveHit(punchToKneeGrabHitPoint.transform.position))
        {
            if (opponent.IsDucking())
            {
                opponent.DuckSuccessful();
            }
            else if (opponent.IsBlocking() && config.blocksBlockHigh)
            {
                opponent.BlockSuccessful();
                block.Play();
            }
            else
            {
                timeSinceProperAttack = 0f;
                opponent.GetKneeGrabbed();
                anim.SetTrigger("PunchToKneeHit");
            }
        }
    }
    public void GetKneeGrabbed()
    {
        SetNotReady();

        SFXHurtGrunt();

        hasBeenHit = true;

        timeSinceProperAttack = 0f;

        isReady = false;
        ducking = false;
        blocking = false;

        roundKickComboWindowOpen = false;
        punchComboWindowOpen = false;
        sideKickComboWindowOpen = false;
        
        TranslatePixels(-movementOffset);
        movementOffset = 0f;
        TranslatePixelsY(-movementOffsetY);
        movementOffsetY = 0f;
    }
    private void PunchToKneeHitParticles()
    {
        kneeHits++;
        opponent.GetHit("Kneed");
        AddPoints(2);

        GameObject hit = Instantiate(hitEffect, punchToKneeHitHitPoint.transform.position, Quaternion.identity);
        Destroy(hit, 2f);
            
        hit.GetComponent<SpriteRenderer>().material.SetColor("_Colour", config.hitColour);
        hit.GetComponent<SpriteRenderer>().material.SetFloat("_GlowAmount", config.hitEffectGlowAmount);
        hitLanded = true;
    }

    public void SetReady()
    {
        isReady = true;

        ducking = false;
        blocking = false;
        movementOffset = 0f;
        movementOffsetY = 0f;

        roundKickComboWindowOpen = false;
        punchComboWindowOpen = false;
        sideKickComboWindowOpen = false;

        transform.position += new Vector3(0f, -transform.position.y, 0f);

        if (setReadyPlayAnimFlag)
        {
            anim.SetTrigger("Ready");
            setReadyPlayAnimFlag = false;
        }

        hitLanded = false;
    }
    public void SetNotReady()
    {
        isReady = false;
    }

    public void SFXWhoosh()
    {
        whoosh.Play();
    }
    private void SFXHitGrunt()
    {
        if (playerNum == 0)
        {
            int index = Random.Range(0, config.p1HitGruntNum);
            while (index == lastHitGruntIndex)
            {
                index = Random.Range(0, config.p1HitGruntNum);
            }
            lastHitGruntIndex = index;

            audioManager.Play("WilliamHit" + index);
        }
        else if (playerNum == 1)
        {
            int index = Random.Range(0, config.p2HitGruntNum);
            while (index == lastHitGruntIndex)
            {
                index = Random.Range(0, config.p2HitGruntNum);
            }
            lastHitGruntIndex = index;

            audioManager.Play("JoeHit" + index);
        }
    }
    private void SFXHurtGrunt()
    {
        if (playerNum == 0)
        {
            int index = Random.Range(0, config.p1HurtGruntNum);
            while (index == lastHurtGruntIndex)
            {
                index = Random.Range(0, config.p1HurtGruntNum);
            }
            lastHitGruntIndex = index;

            audioManager.Play("WilliamHurt" + index);
        }
        else if (playerNum == 1)
        {
            int index = Random.Range(0, config.p2HurtGruntNum);
            while (index == lastHurtGruntIndex)
            {
                index = Random.Range(0, config.p2HurtGruntNum);
            }
            lastHitGruntIndex = index;

            audioManager.Play("JoeHurt" + index);
        }
    }

    public void TranslatePixels(float pixels)
    {
        //transform.position += new Vector3(direction * pixels / 16f * transform.localScale.x, 0f, 0f);
        transform.position += new Vector3(pixels / 16f * transform.localScale.x, 0f, 0f);

        movementOffset += pixels;
    }
    public float PixelsToDistance(float pixels)
    {
        return pixels / 16f * transform.localScale.x;
    }
    public void TranslatePixelsY(float pixels)
    {
        transform.position += new Vector3(0f, pixels / 16f * transform.localScale.y, 0f);

        movementOffsetY += pixels;
    }

    private int Hit(Vector3 position, string hitType)
    {
        Collider[] collisions = Physics.OverlapSphere(position, config.hitRange);

        foreach(Collider col in collisions)
        {
            if (col.gameObject != gameObject && col.tag == "Player")
            {
                timeSinceProperAttack = 0f;

                KaratePlayerController opponent = col.GetComponent<KaratePlayerController>();
                //Debug.Log("Hit!");

                if (hitType == "RoundKick")
                {
                    if ((opponent.IsBlocking() && config.blocksBlockHigh))
                    {
                        opponent.BlockSuccessful();
                        //Stun();
                        block.Play();
                        return 2;
                    }
                    if (opponent.IsDucking())
                    {
                        opponent.DuckSuccessful();
                        return 0;
                    }
                    opponent.GetHit("Head");
                    roundKickHits++;
                    AddPoints(2);
                }
                if (hitType == "Punch")
                {
                    if (opponent.IsBlocking())
                    {
                        opponent.BlockSuccessful();
                        block.Play();
                        return 2;
                    }
                    opponent.GetHit("Head");
                    punchHits++;
                    AddPoints(1);
                }
                if (hitType == "SideKick")
                {
                    sideKickHits++;
                    opponent.GetHit("Head");
                    AddPoints(1);
                }
                if (hitType == "RoundKickToSpinFrontKick")
                {
                    if ((opponent.IsBlocking() && config.blocksBlockHigh))
                    {
                        opponent.BlockSuccessful();
                        block.Play();
                        return 2;
                    }
                    if (opponent.IsDucking())
                    {
                        opponent.DuckSuccessful();
                        return 0;
                    }
                    spinHookKickHits++;
                    opponent.GetHit("Head");
                    AddPoints(2);
                }
                if (hitType == "PunchToFrontKick")
                {
                    if (opponent.IsBlocking())
                    {
                        opponent.BlockSuccessful();
                        block.Play();
                        return 2;
                    }
                    frontKickHits++;
                    opponent.GetHit("Head");
                    AddPoints(1);
                }
                if (hitType == "SideKickToHighSideKick")
                {
                    if ((opponent.IsBlocking() && config.blocksBlockHigh))
                    {
                        opponent.BlockSuccessful();
                        block.Play();
                        return 2;
                    }
                    if (opponent.IsDucking())
                    {
                        opponent.DuckSuccessful();
                        return 0;
                    }
                    highSideKickHits++;
                    opponent.GetHit("Head");
                    AddPoints(2);
                }
                if (hitType == "SideKickToElbow")
                {
                    if (opponent.IsBlocking())
                    {
                        opponent.BlockSuccessful();
                        block.Play();
                        return 2;
                    }
                    elbowHits++;
                    opponent.GetHit("Head");
                    AddPoints(1);
                }
                return 1;
            }
        }
        return 0;
    }

    public bool IsBlocking()
    {
        return blocking;
    }
    public bool IsDucking()
    {
        return ducking;
    }

    public bool IsAttacking()
    {
        string animClipName = CurrentAnimClipName();
        return !isReady && !ducking && !blocking && animClipName != "JumpForward" && animClipName != "JumpBack" && animClipName != "Bow" && animClipName != "Ready";
    }
    public string CurrentAnimClipName()
    {
        return anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    private void DoHit(Vector3 position, string hitType)
    {
        int hitSuccessful = Hit(position, hitType);

        if (hitSuccessful == 0)
        {

        }
        else if (!isShadow)
        {
            GameObject hit = Instantiate(hitEffect, position, Quaternion.identity);
            Destroy(hit, 2f);

            if (hitSuccessful == 1)
            {
                //hit.GetComponent<SpriteRenderer>().color = config.hitColour;
                hit.GetComponent<SpriteRenderer>().material.SetColor("_Colour", config.hitColour);
                hit.GetComponent<SpriteRenderer>().material.SetFloat("_GlowAmount", config.hitEffectGlowAmount);
                hitLanded = true;

                if (isAI)
                {
                    ai.Landed();
                }
            }
            else if (hitSuccessful == 2)
            {
                //hit.GetComponent<SpriteRenderer>().color = config.blockColour;
                hit.GetComponent<SpriteRenderer>().material.SetColor("_Colour", config.blockColour);
                hit.GetComponent<SpriteRenderer>().material.SetFloat("_GlowAmount", config.hitEffectGlowAmount);

                if (isAI)
                {
                    ai.Blocked();
                }
            }
        }
    }

    private void Stun(float time)
    {
        anim.ResetTrigger("StunEnd");
        stunned = true;
        stunTimer = time;
        anim.SetTrigger("StunStart");
    }

    public void GetHit(string hitArea)
    {
        SFXHurtGrunt();

        hasBeenHit = true;
        referee.CountHit();

        timeSinceProperAttack = 0f;

        isReady = false;
        ducking = false;
        blocking = false;

        roundKickComboWindowOpen = false;
        punchComboWindowOpen = false;
        sideKickComboWindowOpen = false;

        if (hitArea == "Head")
        {
            anim.SetTrigger("HitHead");
        }
        else if (hitArea == "Body")
        {
            anim.SetTrigger("HitBody");
        }
        else if (hitArea == "Kneed")
        {
            anim.SetTrigger("HitKneed");
        }

        //Stun(config.stunTime);
        TranslatePixels(-movementOffset);
        movementOffset = 0f;
        TranslatePixelsY(-movementOffsetY);
        movementOffsetY = 0f;

        if (config.refIntervenes)
        {
            //referee.Intervene();
        }
    }

    public void AddPoints(int amount, bool refOverride = false)
    {
        if (!isShadow && (!referee.HasIntervened() || refOverride))
        {
            points += amount;

            if (points < 0)
            {
                points = 0;
            }

            pointsTextbox.text = ("Points: " + points).ToLower();
        }
    }

    public void AddPenalties(int amount)
    {
        if (!isShadow)
        {
            penalties += amount;

            if (penalties <= 0)
            {
                ResetPenaltyDisplay();
            }
            if (penalties >= 1)
            {
                penaltyBox1Off.SetActive(false);
                penaltyBox1On.SetActive(true);
            }
            if (penalties >= 2)
            {
                penaltyBox2Off.SetActive(false);
                penaltyBox2On.SetActive(true);

                referee.Intervene();
                referee.SFXPenalty();
                //AddPoints(-config.penaltyPointLoss);
                penalties = -1;
            }
            if (penalties >= 3)
            {
                penaltyBox3Off.SetActive(false);
                penaltyBox3On.SetActive(true);

                referee.Intervene();
                referee.SFXPenalty();
                //AddPoints(-config.penaltyPointLoss);
                penalties = -1;
            }
        }
    }
    public void ResetPenaltyDisplay()
    {
        penaltyBox1Off.SetActive(true);
        penaltyBox1On.SetActive(false);
        penaltyBox2Off.SetActive(true);
        penaltyBox2On.SetActive(false);
        //penaltyBox3Off.SetActive(true);
        penaltyBox3Off.SetActive(false);
        penaltyBox3On.SetActive(false);
    }
    public int GetPenalties()
    {
        return penalties;
    }

    public void WalkTo(float x)
    {
        if (Mathf.Abs(transform.position.x - walkTarget) < config.walkEndRadius)
        {
            direction = originalDirection;
            transform.localScale = new Vector3(3f * direction, 3f, 3f);
            walking = false;
            transform.position += new Vector3(walkTarget - transform.position.x, 0f, 0f);
            isReady = true;
            StopHoldMoves();
        }
        else
        {
            isReady = false;
            walking = true;
            walkTarget = x;
            anim.ResetTrigger("WalkEnd");
            anim.SetTrigger("WalkStart");
        }
    }

    public bool HasBowed()
    {
        return bowed;
    }
    public bool IsReady()
    {
        return isReady;
    }
    public float PositionWithoutExtraMovement()
    {
        return transform.position.x - movementOffset / 16f * transform.localScale.x;
    }

    private void CheckIfWouldHaveHit(Vector3 hitPoint)
    {
        Collider[] collisions = Physics.OverlapSphere(hitPoint, config.hitRange);

        foreach (Collider col in collisions)
        {
            if (col.gameObject != gameObject && col.tag == "Player")
            {
                timeSinceProperAttack = 0f;
                return;
            }
        }
    }
    private bool WouldHaveHit(Vector3 hitPoint)
    {
        Collider[] collisions = Physics.OverlapSphere(hitPoint, config.hitRange);

        foreach (Collider col in collisions)
        {
            if (col.gameObject != gameObject && col.tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    public float GetTimeSinceLastProperAttack()
    {
        return timeSinceProperAttack;
    }
    public void ResetTimeSinceLastProperAttack()
    {
        timeSinceProperAttack = 0f;
    }

    public void ClearQueuedButton()
    {
        queuedButton = "";
    }

    private void SetQueuedButton(string button)
    {
        if (queuedButton == button)
        {
            //queuedButton = "";
            queuedButton = button;
        }
        else
        {
            queuedButton = button;
        }
        queuedButtonTimer = config.queuedButtonTime;
    }

    public bool HasBeenHit()
    {
        return hasBeenHit;
    }
    public void ResetHasBeenHit()
    {
        hasBeenHit = false;
    }

    private void SetBeltColour(int gradeNum)
    {
        if (gradeNum < 0)
        {
            throw new System.ArgumentException("Grade cannot be negative. (gradeNum = " + gradeNum + ")");
        }
        else if (gradeNum >= config.gradeBeltColours.Length)
        {
            throw new System.ArgumentException("Grade is too high. (gradeNum = " + gradeNum + "; config.gradeBeltColours.Length = " + config.gradeBeltColours.Length + ")");
        }
        else
        {
            //Debug.Log("Setting belt to colour of grade " + gradeNum);
            sprRenderer.material.SetColor("_BeltColour", config.gradeBeltColours[gradeNum]);
        }
    }

    private void UpdateAppearance()
    {
        sprRenderer.material.SetColor("_GiColour", giColour);
        sprRenderer.material.SetColor("_SkinColour", skinColour);
        sprRenderer.material.SetColor("_HairColour", hairColour);
    }

    public void BlockSuccessful()
    {
        blocksSuccessful++;
    }
    public void DuckSuccessful()
    {
        ducksSuccessful++;
    }

    private void UpdateShadow()
    {
        shadow.transform.position = new Vector3(transform.position.x, -1.48f, 1.5f + transform.position.y);
        shadowSprRenderer.sprite = sprRenderer.sprite;
        shadowSprRenderer.color = new Color(0f, 0f, 0f, Mathf.Clamp(0.5f - Mathf.Abs(transform.position.x) / 15f, 0f, 1f));
    }

    public void StopHoldMoves()
    {
        if (true)
        {
            ducking = false;
            anim.SetTrigger("DuckEnd");
            anim.ResetTrigger("DuckStart");
            SetQueuedButton("");
        }
        if (true)
        {
            blocking = false;
            anim.SetTrigger("BlockEnd");
            anim.ResetTrigger("BlockStart");
            SetQueuedButton("");
        }
    }

    public int GetPoints()
    {
        return points;
    }

    private void OnDrawGizmosSelected()
    {
        if (!isShadow)
        {
            if (roundKickHitPoint != null)
            {
                Gizmos.DrawWireSphere(roundKickHitPoint.transform.position, configObj.GetComponent<KarateConfig>().hitRange);
            }

            if (punchHitPoint != null)
            {
                Gizmos.DrawWireSphere(punchHitPoint.transform.position, configObj.GetComponent<KarateConfig>().hitRange);
            }

            if (sideKickHitPoint != null)
            {
                Gizmos.DrawWireSphere(sideKickHitPoint.transform.position, configObj.GetComponent<KarateConfig>().hitRange);
            }
        }
    }

    public bool ReadyToBeIntervened()
    {
        return isReady || blocking || ducking;
    }

    public void SetRoundCircle(int roundNum, int state)
    {
        if (roundNum >= 1 && roundNum <= 3)
        {
            if (state == 0)
            {
                roundCircles[roundNum - 1].sprite = roundNotPlayed;
            }
            else if (state == 1)
            {
                roundCircles[roundNum - 1].sprite = roundLost;
            }
            else if (state == 2)
            {
                roundCircles[roundNum - 1].sprite = roundWon;
            }
            else
            {
                Debug.LogWarning("State not in valid range: state = " + state);
            }
        }
        else
        {
            Debug.LogWarning("Round num not in valid range: roundNum = " + roundNum);
        }
    }

    public void WinRound()
    {
        roundsWon++;
    }
    public int GetRoundWins()
    {
        return roundsWon;
    }

    public float MovementsToOpponent()
    {
        return 2f + Mathf.Floor(((opponent.transform.position.x - transform.position.x) * direction - 2.7f) / PixelsToDistance(direction * 7f));
    }

    public void SetDirection(float newDirection)
    {
        if (newDirection != direction)
        {
            direction = newDirection;
            transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        SetDirectionControls();
    }
    public void SetDirectionControls()
    {
        if (direction == originalDirection)
        {
            button1 = originalButton1;
            button2 = originalButton2;
        }
        else
        {
            button1 = originalButton2;
            button2 = originalButton1;
        }
    }

    public bool IsInSwingPhase()
    {
        return isInSwingPhase;
    }

    public int GetBasicAttacks()
    {
        return roundKicks + punches + sideKicks;
    }
    public int GetCombos()
    {
        return spinHookKicks + knees + frontKicks + highSideKicks + elbows;
    }
    public int GetAllAttacks()
    {
        return GetBasicAttacks() + GetCombos();
    }
    public int GetRoundKicks()
    {
        return roundKicks;
    }
    public int GetPunches()
    {
        return punches;
    }
    public int GetSideKicks()
    {
        return sideKicks;
    }
    public int GetSpinHooks()
    {
        return spinHookKicks;
    }
    public int GetKnees()
    {
        return knees;
    }
    public int GetFrontKicks()
    {
        return frontKicks;
    }
    public int GetHighSideKicks()
    {
        return highSideKicks;
    }
    public int GetElbows()
    {
        return elbows;
    }

    public void LoadPlayer(string name)
    {
        if (PlayerPrefs.GetInt("Karate Grade " + name, -1) == -1)
        {
            CreateNewPlayer(name);
        }

        playerName = name;

        grade = PlayerPrefs.GetInt("Karate Grade " + name);
        giColour = PlayerPrefsX.GetColor("Karate Gi " + name);
        skinColour = PlayerPrefsX.GetColor("Karate Skin " + name);
        hairColour = PlayerPrefsX.GetColor("Karate Hair " + name);

        nameTextbox.text = name;
        nameInputBox.SetTextWithoutNotify(name);

        UpdateColourPicker();

        SetBeltColour(grade);
        UpdateAppearance();
    }

    private void CreateNewPlayer(string name)
    {
        PlayerPrefsX.SetColor("Karate Gi " + name,  Color.white);
        PlayerPrefsX.SetColor("Karate Skin " + name, new Color(253f / 255f, 218f / 255f, 191f / 255f));
        PlayerPrefsX.SetColor("Karate Hair " + name, new Color(250f / 255f, 225f / 255f, 128f / 255f));
        PlayerPrefs.SetInt("Karate Grade " + name, 0);
    }

    public void SaveOutfit()
    {
        PlayerPrefsX.SetColor("Karate Gi " + playerName, giColour);
        PlayerPrefsX.SetColor("Karate Skin " + playerName, skinColour);
        PlayerPrefsX.SetColor("Karate Hair " + playerName, hairColour);
    }

    public void ColourPickerColourPicked()
    {
        if (currentlyEditing == "gi")
        {
            giColour = colourPicker.GetRGB();
        }
        else if (currentlyEditing == "skin")
        {
            skinColour = colourPicker.GetRGB();
        }
        else if (currentlyEditing == "hair")
        {
            hairColour = colourPicker.GetRGB();
        }
        UpdateAppearance();
    }

    private void UpdateColourPicker()
    {
        if (currentlyEditing == "gi")
        {
            colourPicker.SetColour(giColour);
        }
        else if (currentlyEditing == "skin")
        {
            colourPicker.SetColour(skinColour);
        }
        else if (currentlyEditing == "hair")
        {
            colourPicker.SetColour(hairColour);
        }
    }

    public void ChangeCurrentlyEditing(int newCurrentlyEditing)
    {
        switch (newCurrentlyEditing)
        {
            case 0:
                currentlyEditing = "gi";
                break;
            case 1:
                currentlyEditing = "hair";
                break;
            default:
                currentlyEditing = "skin";
                break;
        }
        UpdateColourPicker();
    }

    public void ShowCharacterCreation(bool on)
    {
        characterCreationCanvas.SetActive(on);
        characterCreationPhysicalCanvas.SetActive(on);
    }
}
