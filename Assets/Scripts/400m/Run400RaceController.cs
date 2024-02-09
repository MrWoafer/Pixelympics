using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Run400RaceController : MonoBehaviour
{
    private bool started = false;
    public const float startingCountdown = 5f;
    private float countdown = startingCountdown;
    private string countdownStage = "Marks";

    public GameObject countdownTextObj;
    private Text countdownText;
    public GameObject countdownTextBackObj;
    private Text countdownTextBack;

    public GameObject recordTextObj;
    private Text recordText;

    public GameObject winnerTimeTextObj;
    private Text winnerTimeText;

    private bool timeOn = false;
    private float time = 0f;

    private bool winner = false;
    private int finishers = 0;

    // Start is called before the first frame update
    void Start()
    {
        countdownText = countdownTextObj.GetComponent<Text>();
        countdownTextBack = countdownTextBackObj.GetComponent<Text>();

        recordText = recordTextObj.GetComponent<Text>();
        recordText.text = PlayerPrefs.GetFloat("400m Record", 50f).ToString("n2");
        Debug.Log("Current record is: " + PlayerPrefs.GetFloat("400m Record", 50f).ToString());
        
        winnerTimeText = winnerTimeTextObj.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            started = true;
            StartRace();
        }
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (started)
        {
            countdown -= Time.deltaTime;

            if (countdown <= 2f && countdownStage == "Marks")
            {
                countdownText.text = "SET";
                countdownTextBack.text = "SET";
                countdownStage = "Set";
            }
            else if (countdown <= 0f && countdownStage == "Set")
            {
                countdownText.text = "GO!";
                countdownTextBack.text = "GO!";
                countdownStage = "Go";
                StartTimer();
            }
            else if (countdown <= -1f && countdownStage == "Go")
            {
                countdownText.text = "";
                countdownTextBack.text = "";
                countdownStage = "Going";
            }
        }
    }

    public void FixedUpdate()
    {
        if (timeOn)
        {
            time += Time.fixedDeltaTime;
        }
    }

    public void StartRace()
    {
        countdownText.text = "MARKS";
        countdownTextBack.text = "MARKS";
        countdownStage = "Marks";
    }

    public float GetCountdown()
    {
        return countdown;
    }

    public void FalseStart(string playerName)
    {
        if (started)
        {
            Debug.Log(playerName + " got a false start! There were " + GetCountdown().ToString() + " seconds left.");
        }

        countdown = startingCountdown;
        started = false;
        countdownText.text = "FALSE START";
        countdownTextBack.text = "FALSE START";
    }

    public void StartTimer()
    {
        timeOn = true;
        time = 0f;
    }
    public void StopTimer()
    {
        timeOn = false;
    }
    public float GetTime()
    {
        return time;
    }

    public void Finish(string playerName, float time, bool eligibleForRecord)
    {
        finishers += 1;

        if (time < PlayerPrefs.GetFloat("400m PB " + playerName, 1000f) && eligibleForRecord)
        {
            Debug.Log(playerName + " got a new PB!");
            PlayerPrefs.SetFloat("400m PB " + playerName, time);
        }

        if (!winner)
        {
            winner = true;

            winnerTimeText.text = time.ToString("n2");

            if (time < PlayerPrefs.GetFloat("400m Record", 50f) && eligibleForRecord)
            {
                Debug.Log(playerName + " got a new record!");
                PlayerPrefs.SetFloat("400m Record", time);
                recordText.text = "WR";
            }
        }

        if (finishers >= 4)
        {
            StopTimer();
        }
    }

    public bool HasStarted()
    {
        return started;
    }
}
