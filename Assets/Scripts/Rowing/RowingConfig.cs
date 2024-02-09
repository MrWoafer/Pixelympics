using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RowingConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Race Settings")]
    public float countdown = 3f;
    private float countdownTime;
    public float startX
    {
        get
        {
            return startLine.transform.position.x;
        }
    }
    public float finishX
    {
        get
        {
            return finishLine.transform.position.x;
        }
    }

    [Header("Boat Settings")]
    public float speedDecay = 1f;
    public float focusRingSize = 0.6f;
    public float frontOfBoatOffset = 2.025f;

    [Header("Rowing Settings")]
    /// <summary>
    /// The intended time between rowing strokes.
    /// </summary>
    public float rhythm = 1f;
    public float speedGain = 0.2f;
    public float timeBetweenStrokes = 1f;

    [Header("AI Settings")]
    public float aiHardMaxStrokeOffBy = 0.1f;
    public float aiHardMinStrokeOffBy = -0.1f;
    public float aiMediumMaxStrokeOffBy = 0.2f;
    public float aiMediumMinStrokeOffBy = -0.2f;
    public float aiEasyMaxStrokeOffBy = 0.3f;
    public float aiEasyMinStrokeOffBy = -0.3f;
    public float aiMaxStartTime = 0.2f;
    public float aiMinStartTime = 0.01f;

    [Header("Cloud Settings")]
    [Min(0)]
    public int cloudNum;
    public float cloudMaxX = 50f;
    public float cloudMinX = 0f;
    public float cloudMaxY = 2f;
    public float cloudMinY = -5f;
    public float cloudScale = 3f;

    [Header("References")]
    public Text centreText;
    public Text centreTextShadow;
    public Sprite[] cloudSprites;
    public RowingPlayer[] players;
    public GameObject startLine;
    public GameObject finishLine;
    public GameObject midwayLine;
    public Text distanceText;
    public Text timeText;
    public GameObject wrLine;

    private bool countdownStarted = false;
    private bool started = false;
    private float worldRecord;

    private float raceTime;
    public float raceTimeElapsed
    {
        get
        {
            return raceTime;
        }
    }

    public void SetCentreText(string text)
    {
        centreText.text = text;
        centreTextShadow.text = text;
    }

    private void Start()
    {
        countdownTime = countdown;
        SpawnClouds();

        worldRecord = PlayerPrefs.GetFloat("Rowing Record", 210f);
        Debug.Log("Current Record: " + worldRecord.ToString("n2") + "s. Held by: " + Functions.ArrayToString(Records.GetRecordOwners("Rowing")));
    }

    private void Update()
    {
        if (!countdownStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                countdownStarted = true;
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].GetReady();
                }
            }
        }
        else if (!started)
        {
            countdown -= Time.deltaTime;
            SetCentreText(Mathf.CeilToInt(countdown).ToString());

            if (countdown <= 0f)
            {
                raceTime = 0f;
                started = true;
                SetCentreText("GO!");
            }
        }
        else
        {
            countdown -= Time.deltaTime;
            raceTime += Time.deltaTime;

            if (countdown > -1f)
            {
                SetCentreText("GO!");
            }
            else
            {
                SetCentreText("");
            }

            wrLine.transform.position = new Vector3(Mathf.Lerp(startX, finishX, Functions.RoundToRange(raceTimeElapsed / worldRecord, 0f, 1f)),
                wrLine.transform.position.y, wrLine.transform.position.z);
        }

        float furthestFront = Enumerable.Max(from boat in players select boat.GetFrontOfBoat());
        distanceText.text = Mathf.Max(0f, Mathf.Ceil(finishX - furthestFront)).ToString() + "m";

        UpdateTimer();

        midwayLine.transform.position = new Vector3(0.5f * (finishX + startX), midwayLine.transform.position.y, midwayLine.transform.position.z);
    }

    private void SpawnClouds()
    {
        for (int i = 0; i < cloudNum; i++)
        {
            GameObject cloud = new GameObject("Cloud");
            SpriteRenderer sprCloud = cloud.AddComponent<SpriteRenderer>();
            sprCloud.sortingLayerName = "WaterReflection";
            sprCloud.sortingOrder = -20000;

            sprCloud.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            sprCloud.transform.position = new Vector3(Random.Range(cloudMinX, cloudMaxX), Random.Range(cloudMinY, cloudMaxY), 0f);
            sprCloud.transform.localScale = new Vector3(1f, 1f, 1f) * cloudScale;
        }
    }

    public bool RaceHasStarted()
    {
        return started;
    }

    public void FalseStart()
    {
        countdownStarted = false;
        countdown = countdownTime;
        started = false;
        SetCentreText("FALSE START");
    }

    public bool CountdownHasStarted()
    {
        return countdownStarted;
    }

    public void UpdateTimer()
    {
        bool allFinished = true;
        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].HasFinished())
            {
                allFinished = false;
            }
        }

        if (!allFinished)
        {
            timeText.text = raceTimeElapsed.ToString("n2");
        }
        else
        {
            timeText.text = Enumerable.Max(from p in players select p.GetFinishTime()).ToString("n2");
        }
    }
}
