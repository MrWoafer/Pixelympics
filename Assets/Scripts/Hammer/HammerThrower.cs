using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Direction
{
    right,
    up,
    left,
    down
}

public class HammerThrower : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string buttonUp = "w";
    public string buttonDown = "s";
    public string buttonLeft = "a";
    public string buttonRight = "d";
    public string buttonRelease = "e";
    public RotationDirection rotationDirection = RotationDirection.anticlockwise;

    [Header("Info")]
    public float RotationSpeed = 0f;

    [Header("References")]
    public Rigidbody2D sideOnHammer;
    private HammerConfig config;
    private Rigidbody2D rb;
    public AimDial aimDial;
    public Transform feet;
    public GameObject topDownHammer;
    public GameObject wrLine;
    public GameObject pbLine;
    private OlympicsController olympicsController;

    private bool eligibleForRecord;

    private float rotationSpeed = 0f;
    private float rotationLeft;
    private float rotationAmount;
    private float moveSpeed;

    private bool started = false;
    private bool aiming = false;
    private bool released = false;

    private float t = 0f;
    private Direction lastButtonDirection;

    private Vector3 startPoint;

    private Vector3 releasePoint;

    private float angle = 45f;
    private float speedU;
    private float speedR;

    private float distanceThrown;
    private bool landed = false;
    private bool foul = false;
    private bool thrownFoul = false;

    private int mistakes = 0;
    private int presses = 0;
    
    private float aiT = 0f;
    private float aiVAngle;
    private float aiHAngle;

    private float maxRotationSpeed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<HammerConfig>();
        rb = GetComponent<Rigidbody2D>();

        //rb.constraints = RigidbodyConstraints2D.FreezeAll;

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

        rotationLeft = config.numOfSpins * 360f;
        rotationAmount = rotationLeft;
        startPoint = transform.position;

        Debug.Log("Hammer WR: " + PlayerPrefs.GetFloat("Hammer Record", 10f).ToString("n3") + "m");
        Debug.Log("Hammer Worst Record: " + PlayerPrefs.GetFloat("Hammer Worst Record", 100f).ToString("n3") + "m. Held by: " + PlayerPrefs.GetString("Hammer Worst Record Holder", ""));
        Debug.Log(playerName + "'s PB: " + PlayerPrefs.GetFloat("Hammer PB " + playerName, 0f).ToString("n3") + "m");

        wrLine.transform.position = new Vector3(config.throwMeasurementStart.position.x + PlayerPrefs.GetFloat("Hammer Record", 10f), wrLine.transform.position.y, wrLine.transform.position.z);
        pbLine.transform.position = new Vector3(config.throwMeasurementStart.position.x + PlayerPrefs.GetFloat("Hammer PB " + playerName, 0f), pbLine.transform.position.y, pbLine.transform.position.z);

        if (isAI)
        {
            aiT = config.aiStartTime;
            aiHAngle = GetAIHAngle();
            aiVAngle = GetAIVAngle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            //sideOnHammer.MovePosition(sideOnHammer.position + new Vector2(Time.deltaTime, 0f));
        }

        if (!started)
        {
            if (!isAI)
            {
                if (Input.GetKeyDown(buttonUp))
                {
                    lastButtonDirection = Direction.up;
                    started = true;
                }
                else if (Input.GetKeyDown(buttonDown))
                {
                    lastButtonDirection = Direction.down;
                    started = true;
                }
                else if (Input.GetKeyDown(buttonRight))
                {
                    lastButtonDirection = Direction.left;
                    started = true;
                }
                else if (Input.GetKeyDown(buttonLeft))
                {
                    lastButtonDirection = Direction.right;
                    started = true;
                }
            }
            else
            {
                aiT -= Time.deltaTime;
                if (aiT <= 0f)
                {
                    started = true;
                    aiT = GetAIWaitTime();
                }
            }

            if (started)
            {
                rotationSpeed = config.startingRotationSpeed;
                t = 0f;
                presses++;
            }
        }
        else if (!aiming)
        {
            t += Time.deltaTime;

            if (foul)
            {
                if (rotationSpeed > 0f)
                {
                    rotationSpeed -= config.foulAngleDeceleration * Time.deltaTime;
                }
                else if (rotationSpeed < 0f)
                {
                    rotationSpeed = 0f;
                }
            }

            transform.eulerAngles += new Vector3(0f, 0f, -rotationSpeed * Time.deltaTime * (int)rotationDirection);
            rotationLeft -= rotationSpeed * Time.deltaTime;

            //transform.position += new Vector3(config.moveSpeed * Mathf.Clamp(rotationSpeed / 300f, 0f, 1f) * Time.deltaTime, 0f, 0f);

            if (!foul && transform.position.x < config.throwPointX && rotationLeft > 0f)
            {
                moveSpeed = (config.throwPointX - transform.position.x) / (rotationLeft / rotationSpeed);
                //transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
                //transform.position = new Vector3(config.throwPointX - (config.throwPointX - startPoint.x) * rotationLeft / rotationAmount, transform.position.y, transform.position.z);
            }
            else if (foul)
            {
                if (moveSpeed > 0f)
                {
                    moveSpeed -= config.foulMovementDeceleration * Time.deltaTime;
                }
                else if (moveSpeed < 0f)
                {
                    moveSpeed = 0f;
                }

                //transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);
            }
            transform.position += new Vector3(moveSpeed * Time.deltaTime, 0f, 0f);

            if (!isAI)
            {
                if (feet.position.x > config.outOfRingX && !foul)
                {
                    Foul();
                }
                if (PressedIncorrectButton() && !foul)
                {
                    presses++;
                    mistakes++;
                    rotationSpeed = Mathf.Max(0f, rotationSpeed - config.speedPenalty);
                }
                if (Input.GetKeyDown(GetDirectionKey(GetNextDirection(lastButtonDirection))) && !foul)
                {
                    presses++;

                    lastButtonDirection = GetNextDirection(lastButtonDirection);

                    rotationSpeed += config.rotationGain * Mathf.Max(1f - t, 0f);

                    t = 0f;
                }

                if (Input.GetKeyDown(buttonRelease) && !foul)
                {
                    ReleaseH(Functions.ModAngle(transform.eulerAngles.z - 90f));
                }
            }
            else
            {
                if (feet.position.x > config.outOfRingX && !foul)
                {
                    Foul();
                }
                if (!foul)
                {
                    aiT -= Time.deltaTime;
                    if (aiT <= 0f)
                    {
                        aiT = GetAIWaitTime();

                        rotationSpeed += config.rotationGain * Mathf.Max(1f - t, 0f);

                        t = 0f;
                    }
                    if (rotationLeft <= aiHAngle * (int)rotationDirection)
                    {
                        //ReleaseH(aiHAngle);
                        ReleaseH(Functions.ModAngle(transform.eulerAngles.z - 90f));
                        if (!foul)
                        {
                            ReleaseV(aiVAngle);
                        }
                    }
                }
            }

            if (thrownFoul)
            {
                topDownHammer.transform.position += new Vector3(speedR * Time.deltaTime, speedU * Time.deltaTime, 0f);
            }
        }
        else if (!released)
        {
            if (!isAI)
            {
                if (Input.GetKeyDown(buttonRelease))
                {
                    ReleaseV(aimDial.angle);
                }
            }
            /*else
            {
                if (aimDial.angle)
            }*/
        }
        else if (released)
        {
            if (sideOnHammer.position.y > -3.7f && !landed)
            {
                t += Time.deltaTime;
                sideOnHammer.MovePosition(releasePoint + new Vector3(speedR * t, speedU * t - 0.5f * config.gravity * t * t, 0f));
                sideOnHammer.MoveRotation(Functions.CartesianToPolar(new Vector2(speedR, speedU - config.gravity * t)).y - 90f);
            }
            else if (!landed)
            {
                landed = true;
                sideOnHammer.MovePosition(new Vector2(sideOnHammer.position.x, -3.7f));

                distanceThrown = sideOnHammer.position.x - config.throwMeasurementStart.position.x;
                Debug.Log("Distance Thrown: " + distanceThrown.ToString("n3") + "m");

                BroadcastScore(distanceThrown);

                if (distanceThrown > PlayerPrefs.GetFloat("Hammer PB " + playerName, 0f) && eligibleForRecord)
                {
                    Debug.Log(playerName + " got a new PB!");
                    PlayerPrefs.SetFloat("Hammer PB " + playerName, distanceThrown);
                }

                if (distanceThrown > PlayerPrefs.GetFloat("Hammer Record", 10f) && eligibleForRecord)
                {
                    PlayerPrefs.SetFloat("Hammer Record", distanceThrown);
                    Debug.Log("New Record!");
                }
                else if (distanceThrown < PlayerPrefs.GetFloat("Hammer Worst Record", 100f) && distanceThrown > 0f && eligibleForRecord)
                {
                    PlayerPrefs.SetFloat("Hammer Worst Record", distanceThrown);
                    PlayerPrefs.SetString("Hammer Worst Record Holder", playerName);
                    Debug.Log("New Worst Record!");
                }
            }
        }

        RotationSpeed = rotationSpeed;
        maxRotationSpeed = Mathf.Max(maxRotationSpeed, rotationSpeed);
    }

    private string GetDirectionKey(Direction direction)
    {
        if (direction == Direction.up)
        {
            return buttonUp;
        }
        else if (direction == Direction.down)
        {
            return buttonDown;
        }
        else if (direction == Direction.left)
        {
            return buttonLeft;
        }
        else if (direction == Direction.right)
        {
            return buttonRight;
        }
        else
        {
            throw new System.ArgumentException("Unknown direction: " + direction);
        }
    }

    private Direction GetNextDirection(Direction direction)
    {
        return (Direction)(Functions.Mod((int)direction - (int)rotationDirection, 4));
    }

    private bool PressedIncorrectButton()
    {
        if (GetNextDirection(lastButtonDirection) == Direction.up)
        {
            return Input.GetKeyDown(buttonDown) || Input.GetKeyDown(buttonLeft) || Input.GetKeyDown(buttonRight);
        }
        else if (GetNextDirection(lastButtonDirection) == Direction.down)
        {
            return Input.GetKeyDown(buttonUp) || Input.GetKeyDown(buttonLeft) || Input.GetKeyDown(buttonRight);
        }
        else if (GetNextDirection(lastButtonDirection) == Direction.left)
        {
            return Input.GetKeyDown(buttonDown) || Input.GetKeyDown(buttonUp) || Input.GetKeyDown(buttonRight);
        }
        else if (GetNextDirection(lastButtonDirection) == Direction.right)
        {
            return Input.GetKeyDown(buttonDown) || Input.GetKeyDown(buttonUp) || Input.GetKeyDown(buttonLeft);
        }
        else
        {
            return false;
        }
    }

    private float GetAIWaitTime()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinWaitTime, config.aiOlympicMaxWaitTime);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinWaitTime, config.aiHardMaxWaitTime);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinWaitTime, config.aiMediumMaxWaitTime);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinWaitTime, config.aiEasyMaxWaitTime);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    private float GetAIHAngle()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinHAngle, config.aiOlympicMaxHAngle);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinHAngle, config.aiHardMaxHAngle);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinHAngle, config.aiMediumMaxHAngle);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinHAngle, config.aiEasyMaxHAngle);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    private float GetAIVAngle()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinVAngle, config.aiOlympicMaxVAngle);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinVAngle, config.aiHardMaxVAngle);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinVAngle, config.aiMediumMaxVAngle);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinVAngle, config.aiEasyMaxVAngle);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    private void ReleaseH(float horizontalAngle)
    {
        //Debug.Log(horizontalAngle + " " + aiHAngle);
        float originalAngle = horizontalAngle;
        if (isAI)
        {
            horizontalAngle = aiHAngle;
        }

        Debug.Log("Mistakes: " + mistakes);
        Debug.Log("Valid Presses: " + (presses - mistakes));
        Debug.Log("Release Rotation Speed: " + rotationSpeed.ToString("n3") + "°/s");
        Debug.Log("Highest Rotation Speed: " + maxRotationSpeed.ToString("n3") + "°/s");
        Debug.Log("Horizontal Angle: " + horizontalAngle.ToString("n3") + "°");

        if (horizontalAngle > -config.horizontalAngleBound && horizontalAngle < config.horizontalAngleBound)
        {
            config.MoveCameraToSideOn();

            //sideOnHammer.velocity = new Vector2(1f, 1f) * rotationSpeed / 10f;
            //sideOnHammer.AddForce(new Vector2(1f, 1f) * rotationSpeed, ForceMode2D.Impulse);

            aiming = true;

            //aimDial.ResetRotation();
            aimDial.StartRotation();
        }
        else
        {
            horizontalAngle = originalAngle;

            Debug.Log("Foul! Throw out of bounds.");

            foul = true;
            thrownFoul = true;

            BroadcastScore(float.MinValue);

            topDownHammer.transform.parent = null;

            float speed = Mathf.Sqrt(rotationSpeed / 100f) * 2.8f;
            speedU = Mathf.Sin(horizontalAngle * Mathf.Deg2Rad) * speed * config.speedRMultiplier;
            speedR = Mathf.Cos(horizontalAngle * Mathf.Deg2Rad) * speed * config.speedRMultiplier;
        }
    }

    private void Foul()
    {
        Debug.Log("Foul! Out of ring.");
        foul = true;
        eligibleForRecord = false;

        BroadcastScore(float.MinValue);

        Debug.Log("Mistakes: " + mistakes);
        Debug.Log("Valid Presses: " + (presses - mistakes));
        Debug.Log("Rotation Speed: " + rotationSpeed.ToString("n3") + "°/s");
    }

    private void ReleaseV(float angleV)
    {
        t = 0f;
        released = true;
        releasePoint = sideOnHammer.position;

        //rotationSpeed = 1000f;

        angle = angleV;

        //float speed = rotationSpeed / 100f;
        float speed = Mathf.Sqrt(rotationSpeed / 100f) * 2.8f;
        speedU = Mathf.Sin(angle * Mathf.Deg2Rad) * speed * config.speedUMultiplier;
        speedR = Mathf.Cos(angle * Mathf.Deg2Rad) * speed * config.speedRMultiplier;

        aimDial.SetVisible(false);
        config.UnfreezeHammer();

        Debug.Log("Vertical Angle: " + angle.ToString("n3") + "°");
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
