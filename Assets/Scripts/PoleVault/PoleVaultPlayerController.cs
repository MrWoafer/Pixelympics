using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoleVaultPlayerController : MonoBehaviour
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
    private PoleVaultConfig config;
    public GameObject buttonLeft;
    public GameObject buttonRight;
    public Animator anim;
    private Rigidbody rb;
    public GameObject barObj;
    private PoleVaultBarController bar;
    public GameObject armPivot;
    public GameObject arm;
    public GameObject pole;
    public GameObject camera;
    private PoleVaultCameraController cameraController;
    public GameObject aimDial;
    public GameObject needle;
    private OlympicsController olympicsController;

    private bool ready = false;

    private bool started = false;
    private float speed = 3f;
    private float t;
    private float oldT;

    private int lastButton = 0;
    private float buttonCountdown = 0f;
    private bool shownButton = false;

    private bool sticking = false;
    private Vector3 stickPoint;
    private bool arched = false;
    private bool falling = false;

    private bool landed = false;
    private bool foul = false;

    private float lockSpeed;
    private bool vaulting = false;
    private bool released = false;
    private bool aiming = false;

    private bool eligibleForRecord;
    private bool goneOverBar = false;
    private bool gonePastBar = false;
    private bool goneOffGround = false;

    private bool aimed = false;
    private float angle = 0f;
    private Vector3 releasePoint;

    private float speedU;
    private float speedR;

    private float armPivotArmDistance;

    /// AI vars
    private float aiStartTime;
    private float aiReactTime;
    private float aiStickDistance;
    private float aiVaultAngle;
    private float aiReleaseAngle;
    private Vector3 barPos;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Pole Vault Record", 500f).ToString() + " cm");

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
        if (playerName == "Test")
        {
            eligibleForRecord = false;
        }

        config = configObj.GetComponent<PoleVaultConfig>();

        rb = GetComponent<Rigidbody>();

        bar = barObj.GetComponent<PoleVaultBarController>();

        cameraController = camera.GetComponent<PoleVaultCameraController>();

        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);
        aimDial.SetActive(false);

        SetArmAngle(0f);

        armPivotArmDistance = Mathf.Sqrt((arm.transform.position.x - armPivot.transform.position.x) * (arm.transform.position.x - armPivot.transform.position.x) +
            (arm.transform.position.y - armPivot.transform.position.y) * (arm.transform.position.y - armPivot.transform.position.y));

        if (isAI)
        {
            aiStartTime = config.aiStartTime;
            GetAIReactionTime();

            if (difficulty == Difficulty.Hard)
            {
                aiStickDistance = Random.Range(config.aiMinStickDistanceHard, config.aiMaxStickDistanceHard);
                aiVaultAngle = Random.Range(config.aiMinVaultAngleHard, config.aiMaxVaultAngleHard);
                aiReleaseAngle = Random.Range(config.aiMinReleaseAngleHard, config.aiMaxReleaseAngleHard);
            }
            else if (difficulty == Difficulty.Medium)
            {
                aiStickDistance = Random.Range(config.aiMinStickDistanceMedium, config.aiMaxStickDistanceMedium);
                aiVaultAngle = Random.Range(config.aiMinVaultAngleMedium, config.aiMaxVaultAngleMedium);
                aiReleaseAngle = Random.Range(config.aiMinReleaseAngleMedium, config.aiMaxReleaseAngleMedium);
            }
            else if (difficulty == Difficulty.Easy)
            {
                aiStickDistance = Random.Range(config.aiMinStickDistanceEasy, config.aiMaxStickDistanceEasy);
                aiVaultAngle = Random.Range(config.aiMinVaultAngleEasy, config.aiMaxVaultAngleEasy);
                aiReleaseAngle = Random.Range(config.aiMinReleaseAngleEasy, config.aiMaxReleaseAngleEasy);
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
            if (started && !vaulting)
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));
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

            if (started && !sticking)
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
            
            if (started && !sticking && Input.GetKeyDown(button3) && !isAI)
            {
                lockSpeed = speed;

                Debug.Log("Speed: " + speed);
                sticking = true;

                cameraController.LerpToPlayer();

                buttonLeft.SetActive(false);
                buttonRight.SetActive(false);
            }
            else if (started && !sticking && isAI && transform.position.x - bar.transform.position.x < aiStickDistance)
            {
                lockSpeed = speed;

                Debug.Log("Speed: " + speed);
                sticking = true;

                cameraController.LerpToPlayer();
            }
            else if (sticking && !vaulting)
            {
                SetArmAngle(armPivot.transform.eulerAngles.z + config.stickingSpeed * Time.deltaTime);
                if (GetPoleTip().y <= 0f)
                {
                    cameraController.EndLerp();

                    vaulting = true;
                    t = 0f;

                    stickPoint = GetPoleTip();

                    anim.SetTrigger("Vault");

                    config.vaultSpeed = lockSpeed * 15f + 30f;

                    if (stickPoint.x <= bar.transform.position.x && !foul)
                    {
                        BroadcastScore(float.MinValue);
                        Debug.Log("Foul! Put pole too far!");
                        foul = true;
                    }

                    //camera.GetComponent<PoleVaultCameraController>().LerpTo(bar.transform.position + new Vector3(0.4f, 0.5f, 6f));
                }
            }
            else if (vaulting && !aiming)
            {
                if (Input.GetKeyDown(button3) && !aimed && !isAI)
                {
                    aimed = true;
                    aiming = true;
                    aimDial.SetActive(true);
                    Debug.Log(armPivot.transform.eulerAngles.z + "°");
                }
                else if (isAI && armPivot.transform.eulerAngles.z > aiVaultAngle)
                {
                    aimed = true;
                    aiming = true;
                    Debug.Log(armPivot.transform.eulerAngles.z + "°");

                    pole.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    released = true;
                    releasePoint = transform.position;
                    arm.SetActive(false);
                    anim.SetTrigger("Push");

                    angle = aiReleaseAngle;

                    Debug.Log(angle + "°");

                    speed = Mathf.Sqrt(speed);

                    speedU = speed * Mathf.Sin(angle * Mathf.Deg2Rad) * config.verticalSpeedScalar;
                    speedR = speed * Mathf.Cos(angle * Mathf.Deg2Rad) * config.horizontalSpeedScalar;

                    barPos = bar.transform.position;

                    t = 0f;
                }
                else
                {
                    if (armPivot.transform.eulerAngles.z < 90f)
                    {
                        config.vaultSpeed -= config.vaultSpeedLoss * Time.deltaTime / config.slowMo;
                    }
                    else
                    {
                        config.vaultSpeed += config.vaultSpeedLoss * Time.deltaTime / config.slowMo;
                    }

                    SetArmAngle(armPivot.transform.eulerAngles.z + config.vaultSpeed * Time.deltaTime / config.slowMo);
                    MoveToPole();
                }
            }
            else if (aiming && !released)
            {
                needle.transform.Rotate(0f, 0f, config.needleRotationSpeed * Time.deltaTime);
                if (needle.transform.eulerAngles.z > 0f && needle.transform.eulerAngles.z < 270f)
                {
                    needle.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    aiming = false;

                    aimDial.SetActive(false);
                }
                angle = needle.transform.eulerAngles.z - 270f;
                if (angle < 0f)
                {
                    angle = 0f;
                }
                if (angle > 90f)
                {
                    angle = 90f;
                }

                if (Input.GetKeyDown(button3))
                {
                    pole.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                    released = true;
                    releasePoint = transform.position;
                    arm.SetActive(false);
                    aimDial.SetActive(false);
                    anim.SetTrigger("Push");

                    Debug.Log(angle + "°");

                    speed = Mathf.Sqrt(speed);

                    speedU = speed * Mathf.Sin(angle * Mathf.Deg2Rad) * config.verticalSpeedScalar;
                    speedR = speed * Mathf.Cos(angle * Mathf.Deg2Rad) * config.horizontalSpeedScalar;

                    t = 0f;
                }
            }
            else if (released)
            {
                t += Time.deltaTime;

                if (!isAI)
                {
                    if (Input.GetKey(button1))
                    {
                        transform.Rotate(new Vector3(0f, 0f, config.rotationSpeed * Time.deltaTime));
                    }
                    if (Input.GetKey(button2))
                    {
                        transform.Rotate(new Vector3(0f, 0f, -config.rotationSpeed * Time.deltaTime));
                    }
                }
                else
                {
                    /*float angleBetweenAIAndBar = 0f;
                    if (transform.position.y > bar.transform.position.y)
                    {
                        if (transform.position.y == bar.transform.position.y)
                        {
                            if (transform.position.x > bar.transform.position.x)
                            {
                                angleBetweenAIAndBar = 0f;
                            }
                            else
                            {
                                angleBetweenAIAndBar = 180f;
                            }
                        }
                        else
                        {
                            if (transform.position.x > bar.transform.position.x)
                            {
                                angleBetweenAIAndBar = 90f - Functions.Mod(Mathf.Rad2Deg * Mathf.Atan((transform.position.x - bar.transform.position.x) / (transform.position.y - bar.transform.position.y)), 180f);
                            }
                            else
                            {
                                angleBetweenAIAndBar = 90f + Functions.Mod(Mathf.Rad2Deg * Mathf.Atan((bar.transform.position.x - transform.position.x) / (transform.position.y - bar.transform.position.y)), 180f);
                            }
                        }
                        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, -angleBetweenAIAndBar);
                    }
                    else if (transform.position.x > bar.transform.position.x)
                    {
                        angleBetweenAIAndBar = 0f;
                    }
                    else
                    {
                        angleBetweenAIAndBar = 180f;
                    }

                    if (Functions.Mod(-transform.eulerAngles.z, 360f) < angleBetweenAIAndBar)
                    {
                        transform.Rotate(new Vector3(0f, 0f, -config.rotationSpeed * Time.deltaTime));
                    }*/

                    if (((transform.position.y > barPos.y || transform.position.x < barPos.x) && Functions.Mod(-transform.eulerAngles.z, 360f) < 180f) ||
                        (transform.position.y < barPos.y && transform.position.x < barPos.x && Functions.Mod(-transform.eulerAngles.z, 360f) < 270f))
                    {
                        transform.Rotate(new Vector3(0f, 0f, -config.rotationSpeed * Time.deltaTime));
                    }
                }

                transform.position = releasePoint + new Vector3(-t * speedR, speedU * t - 0.5f * config.gravity * t * t, 0f);
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
        if (transform.position.y > 0.6f)
        {
            goneOffGround = true;
        }

        if (!released)
        {
            MovePole();
        }
    }

    private void GainSpeed()
    {
        float time = buttonCountdown + 1f;
        if (time >= 0f)
        {
            speed += time * config.maxSpeedGain;
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
        return started && !sticking;
    }
    public bool IsJumping()
    {
        return sticking;
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
        else if (collision.gameObject.tag == "Mat" && goneOffGround && !landed)
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

            //if (!foul && goneOverBar && gonePastBar && collision.gameObject.name == "CrashMat")
            if (!foul && goneOverBar && gonePastBar)
            {
                Debug.Log("Successfully cleared " + bar.GetHeight() + "cm!");

                BroadcastScore(bar.GetHeight());

                if (eligibleForRecord)
                {
                    if (bar.GetHeight() > PlayerPrefs.GetFloat("Pole Vault PB " + playerName, 0f))
                    {
                        Debug.Log(playerName + " got a new PB!");
                        PlayerPrefs.SetFloat("Pole Vault PB " + playerName, bar.GetHeight());
                    }

                    if (bar.GetHeight() > PlayerPrefs.GetFloat("Pole Vault Record", 500f))
                    {
                        PlayerPrefs.SetFloat("Pole Vault Record", bar.GetHeight());
                        Debug.Log("New Record!");
                    }
                }
            }
            /*if (collision.gameObject.name != "CrashMat")
            {
                Debug.Log("Did not land on the crash mat!");
            }*/
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

    private void SetArmAngle(float armAngle)
    {
        armPivot.transform.eulerAngles = new Vector3(0f, 0f, armAngle);
    }
    private void MovePole()
    {
        pole.transform.position = new Vector3(
            -(config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x / 2f - config.gripX),
            (config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x / 2f - config.gripX),
            transform.position.z) + armPivot.transform.position;

        pole.transform.eulerAngles = new Vector3(0f, 0f, armPivot.transform.eulerAngles.z);
    }
    private Vector3 GetPoleTip()
    {
        return new Vector3(
            -(config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x - config.gripX),
            (config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x - config.gripX),
            transform.position.z) + armPivot.transform.position;
    }
    private void MoveToPole()
    {
        transform.position = stickPoint - new Vector3(
            -(config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x - config.gripX),
            (config.armLength / 2f + armPivotArmDistance + config.gripY) * Mathf.Cos(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) - Mathf.Sin(armPivot.transform.eulerAngles.z * Mathf.Deg2Rad) * (pole.transform.localScale.x - config.gripX),
            transform.position.z) - armPivot.transform.position + transform.position;
    }

    public bool IsSticking()
    {
        return sticking;
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
