using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkiDownhillConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;
    public bool disableCountdown = false;

    [Header("Race Settings")]
    [Min(1)]
    public int startCountdownLength = 3;

    [Header("Course Settings")]
    public string courseName = "Mount Pix";
    [Min(0f)]
    public float pisteEdgeLineThickness = 0.3f;

    [Header("Turning Settings")]
    [Min(0f)]
    public float maxAngle = 90f;
    [Min(0f)]
    public float rotationSpeedCrouch = 90f;
    [Min(0f)]
    public float rotationSpeedNormal = 180f;
    [Min(0f)]
    public float turningSpeedExchangeRate = 1f;
    [Min(0f)]
    public float turningSpeedExchangeAngleTolerance = 2f;
    [Min(0f)]
    public float turningDownSpeedLossScalar = 0.3f;
    [Min(0f)]
    public float turningSideSpeedLossScalar = 0.9f;

    [Header("Speed Settings")]
    [Min(0f)]
    public float maxDownSpeed = 10f;
    [Min(0f)]
    public float minDownSpeed = 1f;
    [Min(0f)]
    public float maxSideSpeed = 10f;
    [Min(0f)]
    public float speedGainNormal = 1f;
    [Min(0f)]
    public float speedGainCrouch = 1f;

    [Header("Skidding Settings")]
    [Min(0f)]
    public float skidAngleTolerance = 5f;
    [Min(0f)]
    public float maxSkidDeceleration = 5f;

    [Header("Breaking Settings")]
    [Min(0f)]
    public float breakAngleTolerance = 5f;
    [Min(0f)]
    public float breakDeceleration = 5f;

    [Header("References")]
    [SerializeField]
    public CentreText centreText;
    [SerializeField]
    public Text timerText;
    [SerializeField]
    private GameObject gatePrefab;
    [SerializeField]
    private GameObject courses;

    [HideInInspector]
    public bool startedCountdown = false;
    [HideInInspector]
    public bool started = false;

    private float t = 0f;
    public float raceTimeElapsed
    {
        get
        {
            return t - startCountdownLength;
        }
    }

    private SkiDownhillCourse course;

    // Start is called before the first frame update
    void Start()
    {
        course = courses.transform.Find(courseName).GetComponent<SkiDownhillCourse>();
        if (course == null)
        {
            Debug.LogError("Unable to find course: " + courseName);
        }
        else
        {
            course.CreatePisteEdgeLine(pisteEdgeLineThickness);
        }

        if (disableCountdown)
        {
            startedCountdown = true;
            t = startCountdownLength;
        }

        timerText.text = PlayerPrefs.GetFloat("Ski Downhill Record", OlympicsConfig.GetDefaultRecord("Alpine Skiing")).ToString("n2");

        for (int i = 200; i >= -700; i -= 5)
        {
            Instantiate(gatePrefab, new Vector3(0f, i, 0f), Quaternion.identity);
        }

        maxAngle = 90f - Mathf.Rad2Deg * Mathf.Atan2(minDownSpeed, maxSideSpeed);
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
                if (t >= startCountdownLength)
                {
                    started = true;
                }
                else
                {
                    centreText.SetText((startCountdownLength - Mathf.FloorToInt(t)).ToString());
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

    public float SkidDeceleration(float angle, float speedAngle)
    {
        float diff = Mathf.Abs(angle - speedAngle);

        if (diff <= skidAngleTolerance)
        {
            return 0f;
        }

        return Functions.RoundToRange((diff / 140f) * (diff / 140f) * maxSkidDeceleration, 0f, maxSkidDeceleration);
    }
}
