using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ArcheryPlayerController : MonoBehaviour
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

    [Header("References")]
    public GameObject configObj;
    private ArcheryConfig config;
    public GameObject arrowPrefab;
    public ParticleSystem sparkParticles;
    public GameObject crosshair;
    public TextMeshProUGUI scoreTextbox;
    public TextMeshProUGUI timeLeftTextbox;
    public TextMeshProUGUI arrowsLeftTextbox;
    public Camera camera;
    public Camera secondaryCamera;
    private OlympicsController olympicsController;

    private float score = 0f;
    private int arrowsLeft = 10;

    private float timer = 5f;
    private bool aiming = false;

    private float xRot;

    private bool eligibleForRecord = false;

    private float zoom = 60f;

    private bool secondaryCameraCountdownOn = false;
    private float secondaryCameraCountdown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        config = configObj.GetComponent<ArcheryConfig>();

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

        timeLeftTextbox.text = "Time Left: " + timer.ToString("n2") + " s";

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

        secondaryCamera.enabled = false;

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Archery Record", 10f).ToString());
        Debug.Log(playerName + ", your current PB is: " + PlayerPrefs.GetFloat("Archery PB " + playerName, 0f).ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape") && playerName != "Test")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

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

        if (aiming)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0f, 10000f);
            timeLeftTextbox.text = "Time Left: " + timer.ToString("n2") + " s";

            SetZoom(zoom - config.zoomSpeed * Time.deltaTime);
            ScreenShake();

            if (Input.GetMouseButtonUp(0))
            {
                SetZoom(60f);
                aiming = false;
                Shoot();
                arrowsLeft--;
                secondaryCamera.enabled = true;
                StartSecondaryCameraCountdown();
                arrowsLeftTextbox.text = "Arrows Left: " + arrowsLeft;

                /*if (arrowsLeft == 0)
                {
                    Debug.Log("Your overall score: " + score);
                    if (eligibleForRecord)
                    {
                        if (score > PlayerPrefs.GetFloat("Archery PB " + playerName, 0f))
                        {
                            Debug.Log(playerName + ", you got a new PB!");
                            PlayerPrefs.SetFloat("Archery PB " + playerName, score);
                        }

                        if (score > PlayerPrefs.GetFloat("Archery Record", 8f))
                        {
                            PlayerPrefs.SetFloat("Archery Record", score);
                            Debug.Log("New Record!");
                        }
                    }
                }*/
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && arrowsLeft > 0)
            {
                aiming = true;
                timer = config.countdownTime;
                timeLeftTextbox.text = "Time Left: " + timer.ToString("n2") + " s";
            }
        }

        if (secondaryCameraCountdownOn)
        {
            secondaryCameraCountdown -= Time.deltaTime;
            if(secondaryCameraCountdown <= 0f)
            {
                secondaryCameraCountdownOn = false;
                secondaryCamera.enabled = false;
            }
        }
    }

    private void Shoot()
    {
        GameObject arrow = Instantiate(arrowPrefab, transform.position, transform.rotation);
        arrow.GetComponent<ArcheryArrowController>().Fire(config.arrowSpeed, gameObject, config);
    }

    private void SetZoom(float fovZoom)
    {
        zoom = fovZoom;
        if (zoom <= config.minZoom)
        {
            zoom = config.minZoom;
        }
        camera.fieldOfView = zoom;
    }

    public void ArrowHit(float distance)
    {
        if (distance == -1f)
        {
            Debug.Log("Miss!");
        }
        else if (distance <= 1f)
        {
            float points = Mathf.Ceil(10f * (1f - distance));
            score += points;
            scoreTextbox.text = "Score: " + score;
            Debug.Log(points);
        }
        else
        {
            Debug.Log("Miss!");
        }

        if (arrowsLeft == 0)
        {
            Debug.Log(playerName + ", your overall score: " + score);

            BroadcastScore(score);

            if (eligibleForRecord)
            {
                if (score > PlayerPrefs.GetFloat("Archery PB " + playerName, 0f))
                {
                    Debug.Log(playerName + ", you got a new PB!");
                    PlayerPrefs.SetFloat("Archery PB " + playerName, score);
                }

                if (score > PlayerPrefs.GetFloat("Archery Record", 10f))
                {
                    PlayerPrefs.SetFloat("Archery Record", score);
                    Debug.Log("New Record!");
                }
            }
        }
    }

    private void ScreenShake()
    {
        transform.eulerAngles += new Vector3(config.screenShake * Random.Range(-1f, 1f), config.screenShake * Random.Range(-1f, 1f), 0f);
    }

    private void StartSecondaryCameraCountdown()
    {
        secondaryCameraCountdownOn = true;
        secondaryCameraCountdown = 3f;
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
