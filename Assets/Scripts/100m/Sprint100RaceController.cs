using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sprint100RaceController : MonoBehaviour
{
    private bool started = false;
    //public const float startingCountdown = 6f;
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

    public Sprint100Controller[] players;

    public ParticleSystem gunSmoke;

    // Start is called before the first frame update
    void Start()
    {
        countdownText = countdownTextObj.GetComponent<Text>();
        countdownTextBack = countdownTextBackObj.GetComponent<Text>();

        recordText = recordTextObj.GetComponent<Text>();
        //recordText.text = PlayerPrefs.GetFloat("100m Record", 8.983368f).ToString();
        //recordText.text = PlayerPrefs.GetFloat("100m Record", 10f).ToString("n2") + " s";
        recordText.text = PlayerPrefs.GetFloat("100m Record", 10f).ToString("n2");
        Debug.Log("Current record is: " + PlayerPrefs.GetFloat("100m Record", 10f).ToString());

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

            //if (countdown <= 3f && countdownStage == "Marks")
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
                gunSmoke.Play();
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
        //Debug.Log("False start!");
        if (started) {
            Debug.Log(playerName + " got a false start! There were " + GetCountdown().ToString() + " seconds left.");
        }

        countdown = startingCountdown;
        started = false;
        countdownText.text = "FALSE START";
        countdownTextBack.text = "FALSE START";

        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetStarting();
        }
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

        if (time < PlayerPrefs.GetFloat("100m PB " + playerName, 1000f) && eligibleForRecord)
        {
            Debug.Log(playerName + " got a new PB!");
            PlayerPrefs.SetFloat("100m PB " + playerName, time);
        }

        if (!winner)
        {
            winner = true;
            //Debug.Log(PlayerPrefs.GetFloat("100m Record"));

            winnerTimeText.text = time.ToString("n2");

            if (time < PlayerPrefs.GetFloat("100m Record", 10f) && eligibleForRecord)
            {
                Debug.Log(playerName + " got a new record!");
                PlayerPrefs.SetFloat("100m Record", time);
                //recordText.text = time.ToString("n2") + " s";
                //recordText.text = time.ToString("n2");
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
