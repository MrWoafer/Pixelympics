using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJumpPlayerController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Controls")]
    public string button1 = "e";

    private float speed;

    [Header("References")]
    public GameObject configObj;
    private TripleJumpConfig config;
    public GameObject needle;
    public GameObject controllerObj;
    private TripleJumpRaceController controller;
    public GameObject aimDial;
    public GameObject sandpile;
    public GameObject circle;
    public GameObject shadow;
    private OlympicsController olympicsController;

    private float speedMultiplier;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;
    
    private bool drawback = false;
    private bool released = false;

    public int bob = 0;

    [Header("AI Stuff")]
    private float aiBoardX;
    private float aiAngle;
    private float aiSkipHeight;
    private float aiJumpHeight;

    private int jumps = 0;

    private float lockSpeed;
    private float angle = 0f;
    private float t = 0f;
    private float speedU;
    private float speedR;
    private Vector3 releasePoint;
    private bool displayedDistance = false;
    private bool fouled = false;
    private bool allowedToGetRecord;

    private bool jumpLock = true;
    private float nextSpeedU = 0f;
    private float nextSpeedR = 0f;

    private float startTime;
    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
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

        config = configObj.GetComponent<TripleJumpConfig>();

        if (difficulty == Difficulty.Easy)
        {
            config.slowMo = 1.2f;
            allowedToGetRecord = false;
        }
        else if (difficulty == Difficulty.Medium)
        {
            config.slowMo = 1.1f;
            allowedToGetRecord = false;
        }
        else
        {
            allowedToGetRecord = true;
        }

        controller = controllerObj.GetComponent<TripleJumpRaceController>();

        sandpile.SetActive(false);
        circle.SetActive(false);

        speed = config.speed;

        if (isAI)
        {
            startTime = config.aiStartTime;
            aimDial.SetActive(false);

            if (difficulty == Difficulty.Hard)
            {
                aiBoardX = Random.Range(config.aiBoardXMinHard, config.aiBoardXMaxHard);
                aiAngle = Random.Range(config.aiAngleMinHard, config.aiAngleMaxHard);
                aiSkipHeight = Random.Range(config.aiJumpHeightMinHard, config.aiJumpHeightMaxHard);
                aiJumpHeight = Random.Range(config.aiJumpHeightMinHard, config.aiJumpHeightMaxHard);
            }
            else if (difficulty == Difficulty.Medium)
            {
                aiBoardX = Random.Range(config.aiBoardXMinMedium, config.aiBoardXMaxMedium);
                aiAngle = Random.Range(config.aiAngleMinMedium, config.aiAngleMaxMedium);
                aiSkipHeight = Random.Range(config.aiJumpHeightMinMedium, config.aiJumpHeightMaxMedium);
                aiJumpHeight = Random.Range(config.aiJumpHeightMinMedium, config.aiJumpHeightMaxMedium);
            }
            else if (difficulty == Difficulty.Easy)
            {
                aiBoardX = Random.Range(config.aiBoardXMinEasy, config.aiBoardXMaxEasy);
                aiAngle = Random.Range(config.aiAngleMinEasy, config.aiAngleMaxEasy);
                aiSkipHeight = Random.Range(config.aiJumpHeightMinEasy, config.aiJumpHeightMaxEasy);
                aiJumpHeight = Random.Range(config.aiJumpHeightMinEasy, config.aiJumpHeightMaxEasy);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if (isAI)
            {
                startTime -= Time.deltaTime;
                if (startTime <= 0f)
                {
                    started = true;
                    anim.SetTrigger("Start");
                }
            }
            else
            {
                if (Input.GetKeyDown(button1))
                {
                    started = true;
                    anim.SetTrigger("Start");
                }
            }
        }
        else
        {
            if (!displayedDistance)
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0f, 0f));
            }
            if (drawback)
            {
                if (!released && !isAI)
                {
                    needle.transform.Rotate(0f, 0f, config.needleRotationSpeed * Time.deltaTime);
                    if (needle.transform.eulerAngles.z > 0f && needle.transform.eulerAngles.z < 270f)
                    {
                        needle.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    angle = needle.transform.eulerAngles.z - 270f;
                }
                else
                {
                    t += Time.deltaTime / config.slowMo;
                }
            }

            if (!isAI)
            {
                if (Input.GetKeyDown(button1))
                {
                    if (!drawback)
                    {
                        lockSpeed = speed;
                        drawback = true;
                        speed = 0f;
                        anim.SetTrigger("Aim");

                        aimDial.SetActive(true);
                    }
                    else
                    {
                        if (jumps == 0)
                        {
                            anim.SetTrigger("Hop");

                            released = true;
                            t = 0f;

                            speed = lockSpeed;
                            Debug.Log("You had " + (config.lineX - transform.position.x).ToString() + " metres left to the board.");

                            releasePoint = transform.position;

                            speedU = lockSpeed * config.releaseSpeedMultiplierU * Mathf.Sin(angle * Mathf.Deg2Rad);
                            speedR = lockSpeed * config.releaseSpeedMultiplierR * Mathf.Cos(angle * Mathf.Deg2Rad);

                            Debug.Log(angle.ToString() + "°");

                            jumps++;

                            jumpLock = false;
                        }
                        else if (jumps == 1 && !jumpLock)
                        {
                            Jump();
                            Debug.Log(speed.ToString("n3") + " m/s");
                        }
                        else if (jumps == 2 && !jumpLock)
                        {
                            Jump();
                            Debug.Log(speed.ToString("n3") + " m/s");
                        }
                    }
                }
            }
            else
            {
                if (jumps == 0 && config.lineX - transform.position.x < aiBoardX)
                {
                    lockSpeed = speed;
                    drawback = true;
                    speed = 0f;

                    anim.SetTrigger("Hop");

                    released = true;
                    t = 0f;

                    speed = lockSpeed;
                    Debug.Log("You had " + (config.lineX - transform.position.x).ToString() + " metres left to the board.");

                    angle = aiAngle;

                    releasePoint = transform.position;

                    speedU = lockSpeed * config.releaseSpeedMultiplierU * Mathf.Sin(angle * Mathf.Deg2Rad);
                    speedR = lockSpeed * config.releaseSpeedMultiplierR * Mathf.Cos(angle * Mathf.Deg2Rad);

                    Debug.Log(angle.ToString() + "°");

                    jumps++;
                    jumpLock = false;
                }

                if (!jumpLock && t > speedU / config.gravity && ((jumps == 1 && transform.position.y < config.normalY + aiSkipHeight) || (jumps == 2 && transform.position.y < config.normalY + aiJumpHeight)))
                {
                    Jump();
                    Debug.Log(speed.ToString("n3") + " m/s");
                }
            }
            

            if (transform.position.x > config.lineX && !released && !fouled)
            {
                controller.Foul(playerName);
                BroadcastScore(float.MinValue);
                fouled = true;
            }

            if (released)
            {
                if (transform.position.y >= config.normalY && jumps <= 3)
                {
                    transform.position = new Vector3(releasePoint.x + speedR * t, releasePoint.y + speedU * t - config.gravity * t * t / 2f, 0f);
                }
                if (t > speedU / config.gravity && jumps <= 2 && !isAI)
                {
                    circle.SetActive(true);
                }
                else if (!displayedDistance && jumps > 3)
                {
                    displayedDistance = true;
                    var distanceJumped = Functions.RoundToRange(transform.position.x - config.lineX, 0f, 10000f);
                    Debug.Log("Distance jumped: " + distanceJumped.ToString("n2") + " m");

                    controller.SetWinnerDistance(distanceJumped);

                    if (!fouled)
                    {
                        BroadcastScore(distanceJumped);
                    }

                    if (distanceJumped > PlayerPrefs.GetFloat("Triple Jump PB " + playerName, 0f) && !fouled && allowedToGetRecord)
                    {
                        Debug.Log(playerName + " got a new PB!");
                        PlayerPrefs.SetFloat("Triple Jump PB " + playerName, distanceJumped);
                    }

                    if (distanceJumped > PlayerPrefs.GetFloat("Triple Jump Record", 9f) && !fouled && allowedToGetRecord)
                    {
                        PlayerPrefs.SetFloat("Triple Jump Record", distanceJumped);
                        Debug.Log("New Record!");
                        controller.SetRecordDistance("WR");
                    }
                    else if (distanceJumped < PlayerPrefs.GetFloat("Triple Jump Worst Record", 1f) && distanceJumped > 0f && !fouled)
                    {
                        PlayerPrefs.SetFloat("Triple Jump Worst Record", distanceJumped);
                        Debug.Log("New Worst Record!");
                        controller.SetRecordDistance("WR");
                    }

                    if (transform.position.x > config.sandX)
                    {
                        sandpile.SetActive(true);
                    }
                }
                if (transform.position.y < config.normalY)
                {
                    if (!jumpLock)
                    {
                        jumps = 3;
                        anim.SetTrigger("Jump");
                    }
                    if (jumps <= 3)
                    {
                        if (jumps == 1)
                        {
                            anim.SetTrigger("Skip");
                        }
                        else if (jumps == 2)
                        {
                            anim.SetTrigger("Jump");
                        }
                        jumps++;
                        t = 0f;
                        transform.position = new Vector3(transform.position.x, config.normalY, transform.position.z);
                        releasePoint = transform.position;

                        speedU = nextSpeedU;
                        speedR = nextSpeedR;

                        nextSpeedU = 0f;
                        nextSpeedR = 0f;

                        jumpLock = false;

                        circle.SetActive(false);
                    }
                }
            }

            /// Circle
            float scale = config.circleMaxScale * (transform.position.y - config.normalY) / (releasePoint.y + speedU * speedU / config.gravity - config.gravity * speedU / config.gravity * speedU / config.gravity / 2f - config.normalY);
            if (float.IsNaN(scale))
            {
                scale = 1f;
            }
            circle.transform.localScale = new Vector3(scale, scale, scale);

            /// Shadow;
            shadow.transform.position = new Vector3(transform.position.x, shadow.transform.position.y, shadow.transform.position.z);
            scale = 1f;
            shadow.transform.localScale = new Vector3(scale, scale / 2f, scale);
        }
    }

    public float GetSpeed()
    {
        if (speed == 0f)
        {
            return lockSpeed;
        }
        return speed;
    }
    public float GetAngle()
    {
        return angle;
    }

    private void Jump()
    {
        jumpLock = true;
        anim.SetTrigger("Aim");

        speed -= Mathf.Pow(transform.position.y - config.normalY, 2f);
        if (speed < 0f)
        {
            speed = 0f;
        }

        nextSpeedU = speed * config.releaseSpeedMultiplierU * Mathf.Sin(angle * Mathf.Deg2Rad);
        nextSpeedR = speed * config.releaseSpeedMultiplierR * Mathf.Cos(angle * Mathf.Deg2Rad);

        circle.SetActive(false);
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
