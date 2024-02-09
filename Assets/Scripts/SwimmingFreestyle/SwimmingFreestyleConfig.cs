using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwimmingFreestyleConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;
    public bool disableCountdown = false;

    [Header("Race Settings")]
    [Min(1)]
    public int startCountdown = 3;

    [Header("Swimming Settings")]
    [Min(0f)]
    public float timeBetweenStrokes = 0.4f;
    [Min(0f)]
    public float incorrectStrokePenalty = 0.3f;
    [Min(0f)]
    public float startingSpeed = 5f;
    [Min(0f)]
    public float speedLossPerSecond = 1f;
    [Min(0f)]
    public float speedMaxIncrease = 2f;
    [Min(0f)]
    public float speedMinIncrease = 0.4f;
    [Min(1f)]
    [Tooltip("The number of strokes BEFORE having to take a breath.")]
    public int strokesBeforeBreath = 4;
    [Min(0f)]
    public float glideStartSpeed = 4f;
    [Min(0f)]
    public float glideSpeedLossPerSecond = 1f;
    [Min(0f)]
    public float minGlideSpeed = 1f;
    [Min(0f)]
    public float pushOffSpeed = 3f;
    [Min(0f)]
    public float pushOffTimeUntilSwim = 1f;

    [Header("Dive Settings")]
    public float diveHeight = 0.2f;
    public float diveHorizontalSpeed = 1f;
    public float diveVerticalSpeed = 1f;
    public float gravity = 9.81f;

    [Header("Pool Settings")]
    public float poolEndX = 8.9f;

    [Header("GUI Settings")]
    [Min(0f)]
    public float arrowSideSwapDuration = 1f;
    public Color32 defaultBoxColour = new Color32(188, 188, 188, 255);
    public Color32 falseStartBoxColour = new Color32(255, 71, 71, 255);
    public bool hideAIArrows = true;
    public Vector2 pbTimeTextOffset = new Vector2(0f, 0f);

    [Header("AI Settings")]
    [Min(0f)]
    public float aiMinMultiplier = 0.98f;
    [Min(0f)]
    public float aiMaxMultiplier = 1.2f;
    [Min(0f)]
    public float aiMinStartTime = 0.01f;
    [Min(0f)]
    public float aiMaxStartTime = 0.4f;
    [Min(0f)]
    public float aiMinGlideDistance = 1f;
    [Min(0f)]
    public float aiMaxGlideDistance = 2f;
    [Min(0f)]
    public float aiEasyMinPressTime = 0.2f;
    [Min(0f)]
    public float aiEasyMaxPressTime = 0.4f;
    [Min(0f)]
    public float aiMediumMinPressTime = 0.2f;
    [Min(0f)]
    public float aiMediumMaxPressTime = 0.4f;
    [Min(0f)]
    public float aiHardMinPressTime = 0.2f;
    [Min(0f)]
    public float aiHardMaxPressTime = 0.4f;
    [Min(0f)]
    public float aiOlympicMinPressTime = 0.2f;
    [Min(0f)]
    public float aiOlympicMaxPressTime = 0.4f;

    [Header("References")]
    public CentreText centreText;
    public Text timerText;

    [HideInInspector]
    public bool startedCountdown = false;
    [HideInInspector]
    public bool started = false;

    private float t = 0f;
    public float raceTimeElapsed
    {
        get
        {
            return t - startCountdown;
        }
    }

    private List<float> times = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        if (disableCountdown)
        {
            startedCountdown = true;
            t = startCountdown;
        }

        timerText.text = PlayerPrefs.GetFloat("Swimming Freestyle Record", OlympicsConfig.GetDefaultRecord("100m Freestyle")).ToString("n2");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !startedCountdown)
        {
            startedCountdown = true;
            t = 0f;
            timerText.text = "00.00";
        }

        if (startedCountdown)
        {
            t += Time.deltaTime;

            if (!started)
            {
                if (t >= startCountdown)
                {
                    started = true;
                }
                else
                {
                    centreText.SetText((startCountdown - Mathf.FloorToInt(t)).ToString());
                }
            }
            else
            {
                if (raceTimeElapsed <= 1f)
                {
                    centreText.SetText("GO!");
                }
                else
                {
                    centreText.SetText("");
                }

                if (float.Parse(raceTimeElapsed.ToString("n2")) < 10f)
                {
                    timerText.text = "0" + raceTimeElapsed.ToString("n2");
                }
                else
                {
                    timerText.text = raceTimeElapsed.ToString("n2");
                }
            }
        }
    }

    public void FalseStart(string playerName)
    {
        Debug.Log(playerName + " had a false start! They had " + (startCountdown - t).ToString() + "s left.");

        t = 0f;
        started = false;
        startedCountdown = false;
        centreText.SetText("FALSE START");
    }

    /// <summary>
    /// Adds the time to the list of finishing times. Returns the position that places you in in the race.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public int RecordTime(float time)
    {
        if (times.Count == 0 || times[times.Count - 1] != time)
        {
            times.Add(time);
        }
        return times.IndexOf(time) + 1;
    }
}
