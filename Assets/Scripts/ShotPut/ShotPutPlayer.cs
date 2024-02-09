using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPutPlayer : MonoBehaviour
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
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";

    [Header("References")]
    private ShotPutConfig config;
    [SerializeField]
    private GameObject wrLine;
    [SerializeField]
    private GameObject pbLine;
    [SerializeField]
    private AimDial aimDial;
    [SerializeField]
    private SlideBar slideBar;
    [SerializeField]
    private GameObject shot;
    private OlympicsController olympicsController;

    private bool eligibleForRecord;

    private bool startedSlideBar = false;
    private int slideBarPresses = 0;
    private float powerScore = 0f;
    private float power;

    private bool startedAngle = false;
    private float angle;

    private bool released = false;
    private float speedU;
    private float speedR;

    private bool landed = false;
    private float t = 0f;
    private float distanceThrown;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<ShotPutConfig>();

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
        if (playerName == "Test")
        {
            eligibleForRecord = false;
        }

        aimDial.SetVisible(false);
        slideBar.SetVisible(true);

        Debug.Log("Shot Put WR: " + PlayerPrefs.GetFloat("Shot Put Record", OlympicsConfig.GetDefaultRecord("Shot Put")).ToString("n3") + "m. Held by: " + Functions.ArrayToString(Records.GetRecordOwners("Shot Put")));
        Debug.Log("Shot Put Worst Record: " + PlayerPrefs.GetFloat("Shot Put Worst Record", 100f).ToString("n3") + "m. Held by: " + PlayerPrefs.GetString("Shot Put Worst Record Holder", ""));
        Debug.Log(playerName + "'s PB: " + PlayerPrefs.GetFloat("Shot Put PB " + playerName, 0f).ToString("n3") + "m");

        wrLine.transform.position = new Vector3(config.throwMeasurementStart.position.x + PlayerPrefs.GetFloat("Shot Put Record", OlympicsConfig.GetDefaultRecord("Shot Put")), wrLine.transform.position.y, wrLine.transform.position.z);
        pbLine.transform.position = new Vector3(config.throwMeasurementStart.position.x + PlayerPrefs.GetFloat("Shot Put PB " + playerName, 0f), pbLine.transform.position.y, pbLine.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedSlideBar)
        {
            if (Input.GetKeyDown(button1))
            {
                StartSlideBar(DirectionLR.left);
            }
            else if (Input.GetKeyDown(button2))
            {
                StartSlideBar(DirectionLR.right);
            }
        }
        else if (!startedAngle)
        {
            if (Input.GetKeyDown(button1) || Input.GetKeyDown(button2))
            {
                ChangeSlideBarDirection();
            }
        }
        else if (!released)
        {
            if (Input.GetKeyDown(button5))
            {
                angle = aimDial.GetAngle();
                //angle = 45f;
                Debug.Log("Angle: " + angle.ToString("n3") + "°");

                speedU = Mathf.Sin(angle * Mathf.Deg2Rad) * power * config.speedUMultiplier;
                speedR = Mathf.Cos(angle * Mathf.Deg2Rad) * power * config.speedRMultiplier;

                released = true;
                aimDial.SetVisible(false);
            }
        }
        else
        {
            if (shot.transform.position.y > config.shotLandingY && !landed)
            {
                t += Time.deltaTime;
                shot.transform.position = config.releasePoint.position + new Vector3(speedR * t, speedU * t - 0.5f * config.gravity * t * t, 0f);
                shot.transform.eulerAngles = new Vector3(0f, 0f, Functions.CartesianToPolar(new Vector2(speedR, speedU - config.gravity * t)).y - 90f);
            }
            else if (!landed)
            {
                landed = true;
                shot.transform.position = new Vector2(shot.transform.position.x, config.shotLandingY);

                distanceThrown = shot.transform.position.x - config.throwMeasurementStart.position.x;
                Debug.Log("Distance Thrown: " + distanceThrown.ToString("n3") + "m");

                BroadcastScore(distanceThrown);

                if (distanceThrown > PlayerPrefs.GetFloat("Shot Put PB " + playerName, 0f) && eligibleForRecord)
                {
                    Debug.Log(playerName + " got a new PB!");
                    PlayerPrefs.SetFloat("Shot Put PB " + playerName, distanceThrown);
                }

                if (distanceThrown > PlayerPrefs.GetFloat("Shot Put Record", OlympicsConfig.GetDefaultRecord("Shot Put")) && eligibleForRecord)
                {
                    PlayerPrefs.SetFloat("Shot Put Record", distanceThrown);
                    Debug.Log("New Record!");
                }
                else if (distanceThrown < PlayerPrefs.GetFloat("Shot Put Worst Record", 100f) && distanceThrown > 0f && eligibleForRecord)
                {
                    PlayerPrefs.SetFloat("Shot Put Worst Record", distanceThrown);
                    PlayerPrefs.SetString("Shot Put Worst Record Holder", playerName);
                    Debug.Log("New Worst Record!");
                }
            }
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            slideBar.SetMoving(true);
        }*/
    }

    private void StartSlideBar(DirectionLR direction)
    {
        startedSlideBar = true;
        slideBar.SetDirection(direction);
        slideBar.SetMoving(true);
    }

    private void ChangeSlideBarDirection()
    {
        float value = slideBar.GetValue();
        Debug.Log((value * 100f).ToString("n3") + "%");
        powerScore += Functions.RoundToRange(Mathf.Abs(value), 0f, 1f);

        slideBar.SetWidth(slideBar.GetWidth() - config.slideBarShrinkAmount);
        slideBar.ChangeDirection();
        //slideBar.SetMoving(false);

        slideBarPresses++;

        if (slideBarPresses >= config.numOfSlideBarAttempts)
        {
            slideBar.SetVisible(false);
            powerScore /= config.numOfSlideBarAttempts;
            Debug.Log("Mean: " + (powerScore * 100f).ToString("n3") + "%");
            power = GetPower(powerScore);

            startedAngle = true;
            aimDial.SetVisible(true);
            aimDial.StartRotation();
        }
    }

    private float GetPower(float powerScore)
    {
        powerScore = Functions.RoundToRange(1f - powerScore, 0f, 1f);

        //return 5f;
        return Mathf.Max(Mathf.Sqrt(powerScore) * 5f, 0f);
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score);
        }
    }
}
