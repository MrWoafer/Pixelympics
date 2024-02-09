using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeirinPlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Controls")]
    public string button1 = "w";
    public string button2 = "s";
    public string button3 = "e";

    [Header("References")]
    public GameObject configObj;
    private KeirinConfig config;
    public GameObject controllerObj;
    private KeirinGameController controller;
    public TextMeshProUGUI rankingTextBox;

    [Header("Hidden Variables")]
    [SerializeField]
    private float speed = 0f;
    [SerializeField]
    private int lane = 0;
    [SerializeField]
    private int laps = -1;
    private bool lapLock = true;

    /// AI Variables
    private float aiLaneChangeTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<KeirinConfig>();
        controller = controllerObj.GetComponent<KeirinGameController>();

        if (isAI)
        {
            aiLaneChangeTimer = Random.Range(config.aiLaneChangeTimerMin, config.aiLaneChangeTimerMax);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /// Movement - going round bends / along straights, staying in correct lane, etc.
        float r = config.GetLaneRadius(lane);
        float theta = config.speedMultiplier * speed * Time.deltaTime / r;
        float h = r * Mathf.Sqrt(2 - 2 * Mathf.Cos(theta));
        if (transform.position.x < config.leftBendX)
        {
            if (transform.position.y >= config.centreY)
            {
                float mu = Mathf.Atan((config.leftBendX - transform.position.x) / (transform.position.y - config.centreY));
                float thetaPrime = Mathf.PI / 2f - mu - theta / 2f;

                transform.position += new Vector3(-h * Mathf.Sin(thetaPrime), -h * Mathf.Cos(thetaPrime), 0f);
                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * (Mathf.PI + mu));
            }
            else
            {
                float mu = Mathf.Atan((config.leftBendX - transform.position.x) / (config.centreY - transform.position.y));
                float thetaPrime = Mathf.PI / 2f - mu + theta / 2f;

                transform.position += new Vector3(h * Mathf.Sin(thetaPrime), -h * Mathf.Cos(thetaPrime), 0f);
                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * (3f * Mathf.PI / 2f + thetaPrime - theta / 2f));
            }
        }
        else if (transform.position.x > config.rightBendX)
        {
            if (transform.position.y >= config.centreY)
            {
                float mu = Mathf.Atan((transform.position.x - config.rightBendX) / (transform.position.y - config.centreY));
                float thetaPrime = mu - theta / 2f;

                transform.position += new Vector3(-h * Mathf.Cos(thetaPrime), h * Mathf.Sin(thetaPrime), 0f);
                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * (Mathf.PI - mu));
            }
            else
            {
                float mu = Mathf.Atan((transform.position.x - config.rightBendX) / (config.centreY - transform.position.y));
                float thetaPrime = mu + theta / 2f;

                transform.position += new Vector3(h * Mathf.Cos(thetaPrime), h * Mathf.Sin(thetaPrime), 0f);
                transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * (thetaPrime - theta / 2f));
            }
        }
        else
        {
            if (transform.position.y > config.centreY)
            {
                transform.position -= (new Vector3(config.speedMultiplier * speed * Time.deltaTime, transform.position.y - r, 0f));
            }
            else
            {
                transform.position += (new Vector3(config.speedMultiplier * speed * Time.deltaTime, - r - transform.position.y, 0f));
            }
        }

        /// Lap Count
        if (GetDistanceOnTrack() > config.TrackLength(0) / 2f && lapLock)
        {
            lapLock = false;
        }
        if (GetDistanceOnTrack() < 10f && !lapLock)
        {
            lapLock = true;
            laps++;
        }

        /// Input
        if (!isAI)
        {
            if (Input.GetKeyDown(button3))
            {
                float speedGain = controller.GetSpeedGain(playerNum);
                if (speedGain == -1f)
                {
                    speed -= config.speedPenalty;
                }
                else
                {
                    speed += speedGain;
                }
                
                SpeedCheck();
            }
            else if (Input.GetKeyDown(button1))
            {
                SetLane(lane + 1);
            }
            else if (Input.GetKeyDown(button2))
            {
                SetLane(lane - 1);
            }
        }
        /// AI
        else
        {
            aiLaneChangeTimer -= Time.deltaTime;
            if (aiLaneChangeTimer <= 0f)
            {
                aiLaneChangeTimer = Random.Range(config.aiLaneChangeTimerMin, config.aiLaneChangeTimerMax);

                AILaneChangeCheck();
            }
        }
    }

    private void SpeedCheck()
    {
        if (speed < 0f)
        {
            speed = 0f;
        }
    }

    private void LaneCheck()
    {
        if (lane < 0)
        {
            lane = 0;
        }
        else if (lane > 3)
        {
            lane = 3;
        }
    }

    private void SetLane(int newLane)
    {
        int oldLane = lane;
        lane = newLane;
        LaneCheck();

        if (transform.position.x < config.leftBendX)
        {
            transform.position = new Vector3(
                config.leftBendX - (config.leftBendX - transform.position.x) * config.GetLaneRadius(lane) / config.GetLaneRadius(oldLane),
                config.centreY + (transform.position.y - config.centreY) * config.GetLaneRadius(lane) / config.GetLaneRadius(oldLane),
                0f);
        }
        else if (transform.position.x > config.rightBendX)
        {
            transform.position = new Vector3(
                config.rightBendX + (transform.position.x - config.rightBendX) * config.GetLaneRadius(lane) / config.GetLaneRadius(oldLane),
                config.centreY + (transform.position.y - config.centreY) * config.GetLaneRadius(lane) / config.GetLaneRadius(oldLane),
                0f);
        }
    }

    public void LoseSpeed()
    {
        speed -= config.maxSpeedGain - config.maxSpeedGain * config.speedLossCoefficient / (speed + config.speedLossCoefficient);

        //speed += config.behindSpeedGain / ((float)controller.GetPlayers().Length - controller.Ranking(playerNum) + 1);
        speed += config.behindSpeedGain * (controller.Ranking(playerNum) - controller.GetPlayers().Length / 2f) / (controller.GetPlayers().Length / 2f) * Mathf.Abs((controller.Ranking(playerNum) - controller.GetPlayers().Length / 2f) / (controller.GetPlayers().Length / 2f));

        rankingTextBox.text = controller.Ranking(playerNum).ToString();

        SpeedCheck();
    }

    public bool IsAI()
    {
        return isAI;
    }

    public void AIGainSpeed()
    {
        if (difficulty == Difficulty.Hard)
        {
            speed += config.maxSpeedGain * Random.Range(config.aiHardMinMultiplier, config.aiHardMaxMultiplier);
        }
        else if (difficulty == Difficulty.Medium)
        {
            speed += config.maxSpeedGain * Random.Range(config.aiMediumMinMultiplier, config.aiMediumMaxMultiplier);
        }
        else if (difficulty == Difficulty.Easy)
        {
            speed += config.maxSpeedGain * Random.Range(config.aiEasyMinMultiplier, config.aiEasyMaxMultiplier);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision!");
        speed *= config.crashSpeedLossMultiplier;
    }

    public float GetDistanceOnTrack()
    {
        if (transform.position.y < config.centreY && config.leftBendX <= transform.position.x && transform.position.x <= config.rightBendX)
        {
            return config.startLineX - config.leftBendX + config.GetLaneRadius(lane) * Mathf.PI + transform.position.x - config.leftBendX;
        }
        if (transform.position.x <= config.startLineX)
        {
            if (transform.position.x >= config.leftBendX)
            {
                if (transform.position.y > config.centreY)
                {
                    return config.startLineX - transform.position.x;
                }
                else
                {
                    return -1f;
                }
            }
            else
            {
                return config.startLineX - config.leftBendX + (Mathf.PI / 2f - Mathf.Atan((transform.position.y - config.centreY) / (config.leftBendX - transform.position.x)))* config.GetLaneRadius(lane);
            }
        }
        else
        {
            if (transform.position.x > config.rightBendX)
            {
                return config.startLineX - config.leftBendX + config.GetLaneRadius(lane) * Mathf.PI + config.rightBendX - config.leftBendX + (Mathf.PI / 2f + Mathf.Atan((transform.position.y - config.centreY) / (transform.position.x - config.rightBendX))) * config.GetLaneRadius(lane);
            }
            else
            {
                return config.startLineX - config.leftBendX + config.GetLaneRadius(lane) * 2f * Mathf.PI + config.rightBendX - config.leftBendX + config.rightBendX - transform.position.x;
            }
        }
    }
    public float GetDistanceOnTrack(int laneNum)
    {
        int oldLane = lane;
        lane = laneNum;
        float distance = GetDistanceOnTrack();
        lane = oldLane;
        return distance;
    }

    public int GetLane()
    {
        return lane;
    }

    private void AILaneChangeCheck()
    {
        bool laneChanged = false;
        for (int i = 0; i < controller.GetPlayers().Length; i++)
        {
            if (controller.GetPlayers()[i].playerNum != playerNum && !laneChanged)
            {
                if (controller.GetPlayers()[i].GetLane() == lane)
                {
                    float difference = Mathf.Min(controller.GetPlayers()[i].GetDistanceOnTrack() - GetDistanceOnTrack(), controller.GetPlayers()[i].GetDistanceOnTrack() - GetDistanceOnTrack() + config.TrackLength(lane));
                    if (difference >= 0f && difference < config.aiMaxLaneChangeDistance)
                    {
                        if (lane > 0 && controller.IsLaneFree(lane - 1, GetDistanceOnTrack(lane - 1)))
                        {
                            SetLane(lane - 1);
                            laneChanged = true;
                        }
                        else
                        {
                            if (lane < 3 && controller.IsLaneFree(lane + 1, GetDistanceOnTrack(lane + 1)))
                            {
                                SetLane(lane + 1);
                                laneChanged = true;
                            }
                        }
                    }
                }
            }
        }

        if (!laneChanged)
        {
            if (lane > 0 && controller.IsLaneFree(lane - 1, GetDistanceOnTrack(lane - 1)))
            {
                SetLane(lane - 1);
            }
        }
    }

    public int GetLaps()
    {
        return laps;
    }
}
