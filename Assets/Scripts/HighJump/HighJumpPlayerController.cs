using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighJumpPlayerController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";

    [Header("References")]
    public GameObject configObj;
    private HighJumpConfig config;
    public GameObject buttonLeft;
    public GameObject buttonRight;
    public Animator anim;
    private Rigidbody rb;
    public GameObject barObj;
    private HighJumpBarController bar;
    public AimDial dial;
    private Vector3 barPos;
    private OlympicsController olympicsController;

    private bool ready = false;

    private bool started = false;
    private float speed = 3f;
    private float t;
    private float oldT;

    private int lastButton = 0;
    private float buttonCountdown = 0f;
    private bool shownButton = false;

    private bool aiming = false;
    private bool jumping = false;
    private Vector3 releasePoint;
    private bool arched = false;
    private bool falling = false;

    private bool landed = false;
    private bool foul = false;

    private float lockSpeed;
    private float lockAngle;
    private float lockSpeedU;
    private float lockSpeedR;

    private bool eligibleForRecord;
    private bool goneOverBar = false;
    private bool gonePastBar = false;

    /// AI vars
    private float aiStartTime;
    private float aiReactTime;
    private float aiT;
    private float aiAngle;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(Mathf.Sqrt(0.02f) * 1f);
        //Debug.Log(Mathf.Sqrt(0.07f) * 1f);

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("High Jump Record", 200f).ToString() + " cm");

        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings != null)
        {
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

        config = configObj.GetComponent<HighJumpConfig>();

        rb = GetComponent<Rigidbody>();

        t = config.startOffset;
        transform.position = new Vector3(config.jumpX + config.curveCoefficient * t * t, 0.5f, -t);

        bar = barObj.GetComponent<HighJumpBarController>();

        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);

        dial.SetVisible(false);
        dial.angle = 0f;

        if (isAI)
        {
            aiStartTime = config.aiStartTime;
            GetAIReactionTime();

            if (difficulty == Difficulty.Hard)
            {
                aiT = Random.Range(config.aiMinTHard, config.aiMaxTHard);
                aiAngle = Random.Range(config.aiMinAngleHard, config.aiMaxAngleHard);
            }
            else if (difficulty == Difficulty.Medium)
            {
                aiT = Random.Range(config.aiMinTMedium, config.aiMaxTMedium);
                aiAngle = Random.Range(config.aiMinAngleMedium, config.aiMaxAngleMedium);
            }
            else if (difficulty == Difficulty.Easy)
            {
                aiT = Random.Range(config.aiMinTEasy, config.aiMaxTEasy);
                aiAngle = Random.Range(config.aiMinAngleEasy, config.aiMaxAngleEasy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (!landed)
        {
            /// Movement
            if (started && !aiming)
            {
                float distance = speed * Time.deltaTime;
                oldT = t;
                while (DistanceOnCurve(t, oldT) < distance)
                {
                    t -= 0.001f;
                }
                transform.position = new Vector3(config.jumpX + config.curveCoefficient * t * t, 0.5f, -t);
            }

            /// Ready
            if (!ready && Input.GetKeyDown(button3))
            {
                ready = true;
            }

            /// Start
            if (ready && !started && (Input.GetKeyDown(button1) || Input.GetKeyDown(button2)) && !isAI)
            {
                started = true;

                anim.SetTrigger("Run");

                if (Input.GetKeyDown(button1))
                {
                    lastButton = 1;
                }
                else if (Input.GetKeyDown(button2))
                {
                    lastButton = 2;
                }

                speed = config.startingSpeed;
                buttonCountdown = config.maxButtonCountdown;
                shownButton = false;
            }
            if (isAI && ready && !started)
            {
                aiStartTime -= Time.deltaTime;
                if (aiStartTime <= 0f)
                {
                    started = true;
                    anim.SetTrigger("Run");
                    speed = config.startingSpeed;
                    buttonCountdown = config.maxButtonCountdown;
                    shownButton = false;
                }
            }

            if (started && !aiming)
            {
                buttonCountdown -= Time.deltaTime;
                if (buttonCountdown <= 0f && !shownButton)
                {
                    shownButton = true;

                    if (!isAI)
                    {
                        if (lastButton == 1)
                        {
                            buttonRight.SetActive(true);
                        }
                        if (lastButton == 2)
                        {
                            buttonLeft.SetActive(true);
                        }
                    }
                }
                if (shownButton)
                {
                    if (!isAI)
                    {
                        if (Input.GetKeyDown(button1) && lastButton == 2)
                        {
                            Debug.Log(Mathf.Abs(buttonCountdown) + " s");

                            GainSpeed();

                            buttonLeft.SetActive(false);
                            lastButton = 1;
                            buttonCountdown = config.maxButtonCountdown;
                            shownButton = false;
                        }
                        else if (Input.GetKeyDown(button2) && lastButton == 1)
                        {
                            Debug.Log(Mathf.Abs(buttonCountdown) + " s");

                            GainSpeed();

                            buttonRight.SetActive(false);
                            lastButton = 2;
                            buttonCountdown = config.maxButtonCountdown;
                            shownButton = false;
                        }
                    }
                    else
                    {
                        if (Mathf.Abs(buttonCountdown) > aiReactTime)
                        {
                            Debug.Log(Mathf.Abs(buttonCountdown) + " s");

                            GetAIReactionTime();
                            GainSpeed();

                            buttonRight.SetActive(false);
                            buttonCountdown = config.maxButtonCountdown;
                            shownButton = false;
                        }
                    }
                }
            }

            if (!isAI)
            {
                if (started && !aiming && Input.GetKeyDown(button3))
                {
                    //Debug.Log("T: " + t);

                    lockSpeed = Mathf.Sqrt(speed);

                    Debug.Log("Speed: " + speed);
                    //jumping = true;
                    aiming = true;

                    anim.SetTrigger("Jump");

                    buttonLeft.SetActive(false);
                    buttonRight.SetActive(false);

                    dial.SetVisible(true);
                    dial.ResetRotation();
                    dial.StartRotation();
                }
                else if (aiming && !jumping)
                {
                    if (Input.GetKeyDown(button3))
                    {
                        t = 0f;
                        releasePoint = transform.position;

                        jumping = true;
                        lockAngle = dial.angle;

                        Debug.Log("Angle: " + lockAngle + "°");

                        lockSpeedR = lockSpeed * Mathf.Cos(lockAngle * Mathf.Deg2Rad) * config.verticalSpeedScalar;
                        lockSpeedU = lockSpeed * Mathf.Sin(lockAngle * Mathf.Deg2Rad) * config.horizontalSpeed;

                        dial.StopRotation();
                        dial.SetVisible(false);

                        barPos = bar.transform.position;
                    }
                }
                else if (jumping)
                {
                    t += Time.deltaTime / config.slowMo;

                    /*if (!arched)
                    {
                        transform.position = releasePoint + new Vector3(-t * config.horizontalSpeed, lockSpeed * config.verticalSpeedScalar * t - 0.5f * config.gravity * t * t, 0f);
                    }
                    else
                    {
                        transform.position = releasePoint + new Vector3(-t * config.horizontalSpeed, config.archHeightOffset + lockSpeed * config.verticalSpeedScalar * t - 0.5f * config.gravity * t * t, 0f);
                    }*/

                    transform.position = releasePoint + new Vector3(-t * lockSpeedR, lockSpeedU * t - 0.5f * config.gravity * t * t, 0f);
                    
                    if (Input.GetKey(button1))
                    {
                        transform.Rotate(new Vector3(0f, 0f, config.rotationSpeed * Time.deltaTime));
                    }
                    if (Input.GetKey(button2))
                    {
                        transform.Rotate(new Vector3(0f, 0f, -config.rotationSpeed * Time.deltaTime));
                    }

                    /*if (!arched && Input.GetKeyDown(button3))
                    {
                        arched = true;
                        anim.SetTrigger("Arch");
                    }
                    else if (arched && !falling && Input.GetKeyDown(button3))
                    {
                        falling = true;
                        anim.SetTrigger("Fall");
                    }*/
                }
            }
            else
            {
                if (started && !aiming && t < aiT)
                {
                    //Debug.Log("T: " + t);

                    lockSpeed = Mathf.Sqrt(speed);

                    Debug.Log("Speed: " + speed);
                    //jumping = true;
                    aiming = true;

                    anim.SetTrigger("Jump");

                    buttonLeft.SetActive(false);
                    buttonRight.SetActive(false);

                    dial.SetVisible(true);
                    dial.ResetRotation();
                    dial.StartRotation();
                }
                else if (aiming && !jumping)
                {
                    if (dial.angle > aiAngle)
                    {
                        t = 0f;
                        releasePoint = transform.position;

                        jumping = true;
                        lockAngle = dial.angle;

                        Debug.Log("Angle: " + lockAngle + "°");

                        lockSpeedR = lockSpeed * Mathf.Cos(lockAngle * Mathf.Deg2Rad) * config.verticalSpeedScalar;
                        lockSpeedU = lockSpeed * Mathf.Sin(lockAngle * Mathf.Deg2Rad) * config.horizontalSpeed;

                        dial.StopRotation();
                        dial.SetVisible(false);

                        barPos = bar.transform.position;
                    }
                }
                else if (jumping)
                {
                    t += Time.deltaTime / config.slowMo;

                    transform.position = releasePoint + new Vector3(-t * lockSpeedR, lockSpeedU * t - 0.5f * config.gravity * t * t, 0f);

                    if (((transform.position.y > barPos.y || transform.position.x < barPos.x) && Functions.Mod(-transform.eulerAngles.z, 360f) < 180f) ||
                        (transform.position.y < barPos.y && transform.position.x < barPos.x && Functions.Mod(-transform.eulerAngles.z, 360f) < 270f))
                    {
                        transform.Rotate(new Vector3(0f, 0f, -config.rotationSpeed * Time.deltaTime));
                    }
                }
            }
        }

        if (transform.position.y > bar.transform.position.y && transform.position.x <= bar.transform.position.x)
        {
            goneOverBar = true;
        }
        if (transform.position.x < bar.transform.position.x)
        {
            gonePastBar = true;
        }
    }

    private float DistanceOnCurve(float a, float b)
    {
        float aPrime = Functions.Arsinh(2f * config.curveCoefficient * a);
        float bPrime = Functions.Arsinh(2f * config.curveCoefficient * b);
        return (bPrime - aPrime + 0.5f * (Functions.Sinh(2f * bPrime) - Functions.Sinh(2f * aPrime))) / 4f / config.curveCoefficient;
    }

    private void GainSpeed()
    {
        float time = buttonCountdown + 1f;
        //Debug.Log("Countdown: " + buttonCountdown);
        if (time >= 0f)
        {
            //speed += Mathf.Sqrt(time) * config.maxSpeedGain;
            speed += time * config.maxSpeedGain;
            //Debug.Log(speed + " m/s");
        }
        else
        {
            speed += time * config.maxSpeedGain;
        }
    }

    public bool HasStarted()
    {
        return started;
    }
    public bool IsRunning()
    {
        return started && !jumping;
    }
    public bool IsJumping()
    {
        return jumping;
    }
    public bool IsReady()
    {
        return ready;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bar" && !foul)
        {
            Debug.Log("Foul!");

            BroadcastScore(float.MinValue);

            foul = true;
        }
        else if (collision.gameObject.tag == "Mat" && jumping && !landed)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            landed = true;

            //anim.SetTrigger("Arch");

            if (!foul && !goneOverBar)
            {
                BroadcastScore(float.MinValue);

                foul = true;

                Debug.Log("Foul! Went under bar!");
            }

            if (!foul && goneOverBar && gonePastBar)
            {
                Debug.Log("Successfully cleared " + bar.GetHeight() + "cm!");

                BroadcastScore(bar.GetHeight());

                if (eligibleForRecord)
                {
                    if (bar.GetHeight() > PlayerPrefs.GetFloat("High Jump PB " + playerName, 0f))
                    {
                        Debug.Log(playerName + " got a new PB!");
                        PlayerPrefs.SetFloat("High Jump PB " + playerName, bar.GetHeight());
                    }

                    if (bar.GetHeight() > PlayerPrefs.GetFloat("High Jump Record", 200f))
                    {
                        PlayerPrefs.SetFloat("High Jump Record", bar.GetHeight());
                        Debug.Log("New Record!");
                    }
                }
            }
        }
    }

    private void GetAIReactionTime()
    {
        if (difficulty == Difficulty.Hard)
        {
            aiReactTime = Random.Range(config.aiReactTimeMinHard, config.aiReactTimeMaxHard);
        }
        else if (difficulty == Difficulty.Medium)
        {
            aiReactTime = Random.Range(config.aiReactTimeMinMedium, config.aiReactTimeMaxMedium);
        }
        else if (difficulty == Difficulty.Easy)
        {
            aiReactTime = Random.Range(config.aiReactTimeMinEasy, config.aiReactTimeMaxEasy);
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
            olympicsController.RecordScore(score);
        }
    }
}
