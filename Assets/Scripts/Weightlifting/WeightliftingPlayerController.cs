using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeightliftingPlayerController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Settings")]
    public string playerName = "Test";
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public int weight = 0;
    private int presses = 0;
    private string lastPress;
    private float power = 0f;
    private float t = 5f;
    private float t2 = 5f;
    private bool powerLock = false;
    private bool powerLock2 = false;
    private bool angleLock = false;
    private float angle = 0f;
    private int mistakes = 0;

    [Header("References")]
    public GameObject configObj;
    private WeightliftingConfig config;
    public GameObject angleLine;
    public GameObject angleBar;
    public Text timeText;
    public Text secondsText;
    public Text angleText;
    public Animator anim;
    [SerializeField]
    private GameObject weightSelection;
    [SerializeField]
    private InputField weightInput;
    private OlympicsController olympicsController;

    private bool eligibleForRecord = false;

    private float tAI;
    private float aiAngle;

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

        weightInput.SetTextWithoutNotify(weight.ToString());

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Weightlifting Record", 200f).ToString() + "kg");
        Debug.Log(playerName + ", your current PB is: " + PlayerPrefs.GetFloat("Weightlifting PB " + playerName, 0f).ToString() + "kg");

        config = configObj.GetComponent<WeightliftingConfig>();
        angleLine.SetActive(false);
        angleBar.SetActive(false);

        angleText.text = "";
        secondsText.text = "";
        timeText.text = "";

        if (isAI)
        {
            tAI = 1f;
            aiAngle = Random.Range(config.aiMinAngle, config.aiMaxAngle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }

        //First end of countdown
        if (t <= 0f)
        {
            if (!powerLock)
            {
                timeText.text = "";
                secondsText.text = "";
                Debug.Log("You did " + power + " presses");
                Debug.Log("You made " + mistakes + " mistakes");
                powerLock = true;
                angleLine.SetActive(true);
                angleBar.SetActive(true);
            }
            if (!angleLock)
            {
                angle += Time.deltaTime * config.angleSpeed;
                angleLine.transform.position = new Vector3(angleBar.transform.position.x - angleLine.transform.localScale.x / 2f * Mathf.Cos(angle * Mathf.Deg2Rad), angleBar.transform.position.y + angleLine.transform.localScale.x / 2f * Mathf.Sin(angle * Mathf.Deg2Rad), angleBar.transform.position.z);
                angleLine.transform.eulerAngles = new Vector3(0f, 0f, -angle);
                //angleText.text = angle.ToString("n2") + "°";
                angleText.text = angle.ToString("n2") + "'";
                if ((!isAI && Input.GetKeyDown(button3)) || (isAI && angle >= aiAngle))
                {
                    angleLock = true;
                    Debug.Log(angle.ToString("n2") + "°");
                    power -= 2f * Mathf.Abs(90f - angle);
                    lastPress = "";
                    anim.SetTrigger("Lift");
                    tAI = Random.Range(config.aiMinPressTime, config.aiMaxPressTime);
                }
            }
            else
            {
                //Second countdown from 5
                t2 -= Time.deltaTime;
                timeText.text = Mathf.Clamp(t2, 0f, 10000f).ToString("n2");
                secondsText.text = "seconds";
                
                if ( t2 <= 3f)
                {
                    angleLine.SetActive(false);
                    angleBar.SetActive(false);
                    angleText.text = "";
                }

                if (!isAI)
                {
                    //Second tapping to gain power
                    if (Input.GetKeyDown(button1))
                    {
                        if (lastPress != button1)
                        {
                            lastPress = button1;
                            PowerGain();
                            presses++;
                        }
                        else
                        {
                            power -= config.powerLoss;
                            mistakes++;
                        }
                    }
                    if (Input.GetKeyDown(button2))
                    {
                        if (lastPress != button2)
                        {
                            lastPress = button2;
                            PowerGain();
                            presses++;
                        }
                        else
                        {
                            power -= config.powerLoss;
                            mistakes++;
                        }
                    }
                }
                else
                {
                    tAI -= Time.deltaTime;
                    if (tAI <= 0f)
                    {
                        PowerGain();
                        presses++;
                        tAI = Random.Range(config.aiMinPressTime, config.aiMaxPressTime);
                    }
                }
                if (t2 <= 0f)
                {
                    if (!powerLock2)
                    {
                        timeText.text = "";
                        secondsText.text = "";
                        power *= 2f;
                        Debug.Log("You did " + presses + " presses");
                        Debug.Log("You made " + mistakes + " mistakes");
                        Debug.Log("You have " + power + " total power");
                        powerLock2 = true;

                        if (power >= weight)
                        {
                            BroadcastScore(weight);

                            Debug.Log(playerName + " successfully lifted " + weight + "kg");

                            if (eligibleForRecord)
                            {
                                if (weight > PlayerPrefs.GetFloat("Weightlifting PB " + playerName, 0f))
                                {
                                    Debug.Log(playerName + ", you got a new PB!");
                                    PlayerPrefs.SetFloat("Weightlifting PB " + playerName, weight);
                                }

                                if (weight > PlayerPrefs.GetFloat("Weightlifting Record", 200f))
                                {
                                    PlayerPrefs.SetFloat("Weightlifting Record", weight);
                                    Debug.Log("New Record!");
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Foul!");

                            BroadcastScore(OlympicsConfig.FoulValue("Weightlifting"));
                        }
                    }
                }
            }
        }
        else
        {
            //Countdown starts on first press
            if (lastPress != button1 && lastPress != button2)
            {
                if (!isAI)
                {
                    if (Input.GetKeyDown(button1))
                    {
                        lastPress = button1;
                        PowerGain();

                        weightSelection.SetActive(false);
                        secondsText.text = "seconds";
                    }
                    if (Input.GetKeyDown(button2))
                    {
                        lastPress = button2;
                        PowerGain();

                        weightSelection.SetActive(false);
                        secondsText.text = "seconds";
                    }
                }
                else
                {
                    tAI -= Time.deltaTime;
                    //if (tAI <= 0f)
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        lastPress = button2;
                        PowerGain();
                        tAI = Random.Range(config.aiMinPressTime, config.aiMaxPressTime);

                        weightSelection.SetActive(false);
                        secondsText.text = "seconds";
                    }
                }   
            }
            else
            {
                //First Countdown from 5
                t -= Time.deltaTime;
                timeText.text = Mathf.Clamp(t, 0f, 10000f).ToString("n2");

                if (!isAI)
                {
                    //First tapping to gain power
                    if (Input.GetKeyDown(button1))
                    {
                        if (lastPress != button1)
                        {
                            lastPress = button1;
                            PowerGain();
                        }
                        else
                        {
                            power -= config.powerLoss;
                            mistakes++;
                        }
                    }
                    if (Input.GetKeyDown(button2))
                    {
                        if (lastPress != button2)
                        {
                            lastPress = button2;
                            PowerGain();
                        }
                        else
                        {
                            power -= config.powerLoss;
                            mistakes++;
                        }
                    }
                }
                else
                {
                    tAI -= Time.deltaTime;
                    if (tAI <= 0f)
                    {
                        PowerGain();
                        tAI = Random.Range(config.aiMinPressTime, config.aiMaxPressTime);
                    }
                }
            }
        }
    }
    
    private void PowerGain()
    {
        power += config.powerGain;
    }

    public void SetWeightFromTextBox(string inputWeight)
    {
        try
        {
            int newWeight = int.Parse(inputWeight);

            if (newWeight >= 0f)
            {
                Debug.Log("Set height to " + newWeight + "cm");
                SetWeight(newWeight);
            }
            else
            {
                Debug.Log("Invalid height");
                weightInput.text = weight.ToString();
            }
        }
        catch
        {
            Debug.Log("Invalid height");
            weightInput.text = weight.ToString();
        }
    }

    public void SetWeight(int newHeight)
    {
        weight = newHeight;
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
