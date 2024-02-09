using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wrestler : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    public int playerNum = 0;
    [HideInInspector]
    public int playerID;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

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

    [Header("Shadow")]
    public GameObject shadow;
    private SpriteRenderer shadowSprRenderer;

    [Header("References")]
    public Wrestler opponent;
    private WrestlingConfig config;
    private SpriteRenderer spr;
    public Text nameTextbox;
    public InputField nameInputBox;
    public ColourPicker colourPicker;
    public GameObject characterCreationCanvas;
    public GameObject characterCreationPhysicalCanvas;
    [HideInInspector]
    public Animator anim;
    private WrestlingReferee referee;
    public Text pointsTextbox;
    public GameObject penaltyBox1Off;
    public GameObject penaltyBox1On;
    public GameObject penaltyBox2Off;
    public GameObject penaltyBox2On;
    public GameObject penaltyBox3Off;
    public GameObject penaltyBox3On;
    public Image[] roundCircles;
    public Sprite roundNotPlayed;
    public Sprite roundWon;
    public Sprite roundLost;
    public GameObject grabHitPoint;
    public WrestlingBall ball;

    private float movementOffset = 0f;
    private float movementOffsetY = 0f;

    private bool isReady = false;
    private string queuedButton = "";
    private float queuedButtonTimer = 0f;

    private float timeSinceProperAttack = 0f;

    private int roundsWon = 0;

    private bool ducking = false;
    private bool blocking = false;
    private bool bowed = false;

    private bool hasBeenHit = false;

    private int points = 0;
    private int penalties = 0;

    private bool walking = false;
    private float walkTarget = 0f;

    [HideInInspector]
    public bool inBall = false;

    private float aiT = 0f;
    private float aiDirection = 1f;

    private void Awake()
    {
        //Debug.Log(button1);
        originalButton1 = button1;
        originalButton2 = button2;
        originalDirection = direction;
    }

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        shadowSprRenderer = shadow.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        config = GameObject.Find("Config").GetComponent<WrestlingConfig>();
        referee = GameObject.Find("Referee").GetComponent<WrestlingReferee>();

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

        nameTextbox.text = playerName.ToLower();

        AddPenalties(0);
        for (int i = 1; i <= 3; i++)
        {
            SetRoundCircle(i, 0);
        }

        if (!config.developmentMode)
        {
            
        }
        else
        {
            bowed = true;
        }

        if (config.developmentMode)
        {
            ShowCharacterCreation(false);
        }
        else
        {
            ShowCharacterCreation(true);
        }
        LoadPlayer(playerName);
        SetBeltColour(grade);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAppearance();

        queuedButtonTimer -= Time.deltaTime;
        if (queuedButtonTimer <= 0f && queuedButton != "")
        {
            queuedButton = "";
        }

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
                transform.localScale = new Vector3(-3f, 3f, 3f);
            }
            else if (transform.position.x < walkTarget)
            {
                transform.localScale = new Vector3(3f, 3f, 3f);
            }
        }
        else if (referee.HasStartedMatch())
        {
            if (inBall)
            {
                if (!isAI)
                {
                    if (Input.GetKey(button1))
                    {
                        ball.Rotate((RotationDirection)(-direction));
                    }
                    if (Input.GetKey(button2))
                    {
                        ball.Rotate((RotationDirection)direction);
                    }
                }
                else
                {
                    aiT -= Time.deltaTime;
                    if (aiT <= 0f)
                    {
                        aiT = Random.Range(0.3f, 1f);

                        aiDirection = Functions.RandomBool() ? 1f : -1f;
                    }
                    ball.Rotate((RotationDirection)aiDirection);
                }
            }
            else if (isReady)
            {
                if (Input.GetKeyDown(button1) || queuedButton == button1)
                {
                    isReady = false;
                    anim.SetTrigger("JumpBack");
                }
                if (Input.GetKeyDown(button2) || queuedButton == button2)
                {
                    isReady = false;
                    anim.SetTrigger("JumpForward");
                }
                if (Input.GetKeyDown(button5) || queuedButton == button5)
                {
                    isReady = false;
                    anim.SetTrigger("Lunge");
                }
            }
            else
            {
                if (Input.GetKeyDown(button5))
                {
                    SetQueuedButton(button5);
                }
            }
        }

        UpdateShadow();
    }

    private void UpdateAppearance()
    {
        spr.material.SetColor("_GiColour", giColour);
        spr.material.SetColor("_SkinColour", skinColour);
        spr.material.SetColor("_HairColour", hairColour);
    }

    private void UpdateShadow()
    {
        shadow.transform.position = new Vector3(transform.position.x, -1.48f, 1.5f + transform.position.y);
        shadowSprRenderer.sprite = spr.sprite;
        shadowSprRenderer.color = new Color(0f, 0f, 0f, Mathf.Clamp(0.5f - Mathf.Abs(transform.position.x) / 15f, 0f, 1f));
    }

    public void LoadPlayer(string name)
    {
        if (PlayerPrefs.GetInt("Wrestling Grade " + name, -1) == -1)
        {
            CreateNewPlayer(name);
        }

        playerName = name;

        grade = PlayerPrefs.GetInt("Wrestling Grade " + name);
        giColour = PlayerPrefsX.GetColor("Wrestling Gi " + name);
        skinColour = PlayerPrefsX.GetColor("Wrestling Skin " + name);
        hairColour = PlayerPrefsX.GetColor("Wrestling Hair " + name);

        nameTextbox.text = name;
        nameInputBox.SetTextWithoutNotify(name);

        UpdateColourPicker();

        SetBeltColour(grade);
        UpdateAppearance();
    }

    private void CreateNewPlayer(string name)
    {
        PlayerPrefsX.SetColor("Wrestling Gi " + name, new Color(24f / 255f, 68f / 255f, 219f / 255f));
        PlayerPrefsX.SetColor("Wrestling Skin " + name, new Color(253f / 255f, 218f / 255f, 191f / 255f));
        PlayerPrefsX.SetColor("Wrestling Hair " + name, new Color(250f / 255f, 225f / 255f, 128f / 255f));
        PlayerPrefs.SetInt("Wrestling Grade " + name, 0);
    }

    public void SaveOutfit()
    {
        PlayerPrefsX.SetColor("Wrestling Gi " + playerName, giColour);
        PlayerPrefsX.SetColor("Wrestling Skin " + playerName, skinColour);
        PlayerPrefsX.SetColor("Wrestling Hair " + playerName, hairColour);
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
            spr.material.SetColor("_BeltColour", config.gradeBeltColours[gradeNum]);
        }
    }

    public void TranslatePixels(float pixels)
    {
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

    private void SetQueuedButton(string button)
    {
        if (queuedButton == button)
        {
            queuedButton = button;
        }
        else
        {
            queuedButton = button;
        }
        queuedButtonTimer = config.queuedButtonTime;
    }
    public void ClearQueuedButton()
    {
        queuedButton = "";
    }

    public void SetReady()
    {
        isReady = true;
        movementOffset = 0f;
        movementOffsetY = 0f;

        transform.position += new Vector3(0f, -transform.position.y, 0f);
    }
    public void SetNotReady()
    {
        isReady = false;
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

    public float GetTimeSinceLastProperAttack()
    {
        return timeSinceProperAttack;
    }
    public void ResetTimeSinceLastProperAttack()
    {
        timeSinceProperAttack = 0f;
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

    public bool HasBeenHit()
    {
        return hasBeenHit;
    }
    public void ResetHasBeenHit()
    {
        hasBeenHit = false;
    }

    public bool ReadyToBeIntervened()
    {
        return isReady || blocking || ducking;
    }

    public int GetPoints()
    {
        return points;
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

    public void ResetPenaltyDisplay()
    {
        penaltyBox1Off.SetActive(true);
        penaltyBox1On.SetActive(false);
        penaltyBox2Off.SetActive(true);
        penaltyBox2On.SetActive(false);
        penaltyBox3Off.SetActive(false);
        penaltyBox3On.SetActive(false);
    }
    public int GetPenalties()
    {
        return penalties;
    }

    public void AddPoints(int amount, bool refOverride = false)
    {
        if (!referee.HasIntervened() || refOverride)
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
            penalties = -1;
        }
        if (penalties >= 3)
        {
            penaltyBox3Off.SetActive(false);
            penaltyBox3On.SetActive(true);

            referee.Intervene();
            referee.SFXPenalty();
            penalties = -1;
        }
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

    public void Grab()
    {
        if (CheckIfHit(grabHitPoint.transform.position))
        {
            Debug.Log("Yoink!");
            anim.SetTrigger("Throw");
            opponent.Fall();
        }
    }

    private bool CheckIfHit(Vector3 hitPoint)
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

    public void Fall()
    {
        isReady = false;
        anim.SetTrigger("Fall");
    }

    public void MakeBall()
    {
        ball.MakeBall(this);
    }
}
