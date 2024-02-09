using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SkeetPlayerController : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";
    public bool aimWithKeyboard = false;

    [Header("Settings")]
    public int shotsLeft = 5;
    public int score = 0;
    public bool fullGame = false;

    [Header("References")]
    public GameObject configObj;
    private SkeetConfig config;
    public GameObject camera;
    public ParticleSystem puffParticles;
    public ParticleSystem sparkParticles;
    public GameObject crosshair;
    public TextMeshProUGUI hitsTextbox;
    public TextMeshProUGUI shotsLeftTextbox;
    private OlympicsController olympicsController;

    private float xRot = 0f;

    private bool atStation = false;

    private bool eligibleForRecord = false;
    private bool finished = false;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<SkeetConfig>();

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

        crosshair.SetActive(false);

        shotsLeftTextbox.text = "Shots Left: " + shotsLeft;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && playerName != "Test")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

        if (atStation)
        {
            /// Aim with keyboard
            if (aimWithKeyboard)
            {
                if (Input.GetKey(button1))
                {
                    transform.eulerAngles += new Vector3(0f, -config.keyboardHorizontalRotationSpeed * Time.deltaTime, 0f);
                }
                if (Input.GetKey(button2))
                {
                    transform.eulerAngles += new Vector3(0f, config.keyboardHorizontalRotationSpeed * Time.deltaTime, 0f);
                }
                if (Input.GetKey(button3))
                {
                    transform.eulerAngles += new Vector3(-config.keyboardVerticalRotationSpeed * Time.deltaTime, 0f, 0f);
                }
                if (Input.GetKey(button4))
                {
                    transform.eulerAngles += new Vector3(config.keyboardVerticalRotationSpeed * Time.deltaTime, 0f, 0f);
                }
            }
            /// Aim with mouse
            else
            {
                float mouseX = Input.GetAxis("Mouse X") * config.mouseHorizontalRotationSpeed * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * config.mouseVerticalRotationSpeed * Time.deltaTime;

                xRot -= mouseY;
                xRot = Functions.RoundToRange(xRot, -90f, 80f);

                transform.eulerAngles = new Vector3(xRot, transform.eulerAngles.y + mouseX, transform.eulerAngles.z);
            }

            if ((Input.GetKeyDown(button5) && aimWithKeyboard) || (Input.GetMouseButtonDown(0) && !aimWithKeyboard))
            {
                if (shotsLeft > 0)
                {
                    Shoot();
                }
            }



            if (Input.GetKeyDown("space") && !config.pulled)
            {
                //Debug.Log("Pull!");
                config.pulled = true;
            }

            if (Input.GetKeyDown(KeyCode.Return) && fullGame)
            {
                if (config.stationNum == 8)
                {
                    if (!finished)
                    {
                        finished = true;
                        fullGame = false;

                        Debug.Log(playerName + ", your score is: " + score + " / 25 hits");

                        BroadcastScore(score);

                        if (eligibleForRecord)
                        {
                            if (score > PlayerPrefs.GetFloat("Skeet PB " + playerName, 0f))
                            {
                                Debug.Log(playerName + ", you got a new PB!");
                                PlayerPrefs.SetFloat("Skeet PB " + playerName, score);
                            }

                            if (score > PlayerPrefs.GetFloat("Skeet Record", 8f))
                            {
                                PlayerPrefs.SetFloat("Skeet Record", score);
                                Debug.Log("New Record!");
                            }
                        }
                    }
                }
                else
                {
                    GoToStation(config.stationNum + 1);
                    config.pulled = false;
                }
            }
        }
    }

    private void Shoot()
    {
        shotsLeft--;
        //Debug.Log("BANG!");
        //Debug.Log(shotsLeft + " shots left.");

        shotsLeftTextbox.text = "Shots Left: " + shotsLeft;

        ParticleSystem puff = Instantiate(puffParticles, puffParticles.transform.position, puffParticles.transform.rotation);
        puff.Play();
        Destroy(puff.gameObject, 4f);

        RaycastHit ray;
        bool hit = Physics.Raycast(camera.transform.position, transform.forward, out ray);

        if (hit)
        {
            ParticleSystem spark = Instantiate(sparkParticles, ray.point, Quaternion.LookRotation(ray.normal));
            spark.Play();
            Destroy(spark.gameObject, 1f);

            if (ray.transform.tag == "Skeet" && !ray.transform.GetComponent<SkeetController>().IsBroken())
            {
                //Debug.Log("Hit!");
                ray.transform.GetComponent<SkeetController>().Break();

                score++;
                hitsTextbox.text = "Hits: " + score;
            }            
        }
    }

    public void GoToStation(int stationNum)
    {
        if (stationNum >= 1 && stationNum <= config.stationLocations.Length)
        {
            atStation = true;

            transform.position = config.stationLocations[stationNum - 1];
            config.stationNum = stationNum;

            if (!aimWithKeyboard)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            crosshair.SetActive(true);

            transform.LookAt(new Vector3(0f, 1.5f, 70f));

            shotsLeft = config.shotsPerStation[stationNum - 1];
            shotsLeftTextbox.text = "Shots Left: " + shotsLeft;
        }
        else
        {
            throw new System.ArgumentException("No station " + stationNum + ".");
        }
    }

    public void DoFullGame()
    {
        fullGame = true;
        score = 0;

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Skeet Record", 8f).ToString() + " / 25 hits");
        Debug.Log(playerName + ", your current PB is: " + PlayerPrefs.GetFloat("Skeet PB "  + playerName, 0f).ToString() + " / 25 hits");

        GoToStation(1);
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
