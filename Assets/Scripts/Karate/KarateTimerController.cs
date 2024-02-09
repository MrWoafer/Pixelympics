using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KarateTimerController : MonoBehaviour
{
    [Header("Settings")]
    public float countdownTime = 0f;
    public bool started = false;

    [Header("References")]
    public Text timerTextbox;

    // Start is called before the first frame update
    void Start()
    {
        SetTimeText();
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime <= 0f)
            {
                countdownTime = 0f;
                started = false;
            }
            SetTimeText();
        }
    }

    private void SetTimeText()
    {
        int mins = Mathf.FloorToInt(Mathf.CeilToInt(countdownTime) / 60f);
        int seconds = Mathf.CeilToInt(Mathf.CeilToInt(countdownTime) % 60f);

        if (seconds < 10)
        {
            timerTextbox.text = mins + ":" + "0" + seconds;
        }
        else
        {
            timerTextbox.text = mins + ":" + seconds;
        }
    }

    public void SetTimer(float time)
    {
        countdownTime = time;
        SetTimeText();
    }

    public void StartTimer(float time)
    {
        countdownTime = time;
        started = true;
    }

    public float GetTime()
    {
        return countdownTime;
    }
}
