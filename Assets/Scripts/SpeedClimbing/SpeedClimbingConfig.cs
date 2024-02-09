using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class Course
{
    public string name;
    public GameObject course;
    public SpeedClimbingHold[] holds
    {
        get
        {
            return GetHolds();
        }
    }

    public SpeedClimbingHold[] GetHolds()
    {
        return GetHolds(course);
    }
    public static SpeedClimbingHold[] GetHolds(GameObject obj)
    {
        SpeedClimbingHold[] holds = obj.GetComponentsInChildren<SpeedClimbingHold>();

        holds = holds.OrderBy(x => x.transform.position.y).ToArray();

        return holds;
    }
}

public class SpeedClimbingConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Race Settings")]
    public float countdown = 3f;
    public float t = 0f;

    [Header("Course Settings")]
    public string currentCourseName = "Test";
    public bool randomiseButtons = false;
    public Course[] courses;
    private SpeedClimbingHold[] holds1;
    private SpeedClimbingHold[] holds2;
    public SpeedClimber p1;
    public SpeedClimber p2;
    private Course course1;
    private Course course2;

    [Header("AI Settings")]
    public float aiOlympicMaxWait = 0.1f;
    public float aiOlympicMinWait = 0.1f;
    public float aiHardMaxWait = 0.1f;
    public float aiHardMinWait = 0.1f;
    public float aiMediumMaxWait = 0.1f;
    public float aiMediumMinWait = 0.1f;
    public float aiEasyMaxWait = 0.1f;
    public float aiEasyMinWait = 0.1f;
    public float aiMaxStartTime = 0.15f;
    public float aiMinStartTime = 0.05f;

    [Header("Hold Settings")]
    public float snapDistance = 0.1f;
    public float minSwingXDifference = 0.4f;
    public float wrongButtonPenalty = 0.3f;

    [Header("Wall Settings")]
    public float paddingTop = 4f;

    [Header("Camera Settings")]
    public float parallaxScalar = 0.9f;

    [Header("References")]
    public GameObject wall;
    public Sprite buttonSprite;
    public Text centreText;
    public Text centreTextDropShadow;
    public GameObject crowd1;
    public GameObject crowd2;
    public GameObject camera1;
    public GameObject camera2;
    private float cameraStartY;
    private float crowdStartY;

    private bool countdownStarted = false;
    private bool started = false;
    private float raceTime = 0f;
    public float raceTimeElapsed
    {
        get
        {
            return raceTime;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        crowdStartY = crowd1.transform.position.y;

        for (int i = 0; i < courses.Length; i++)
        {
            courses[i].course.SetActive(false);
            
            if (courses[i].name == currentCourseName)
            {
                course1 = courses[i];
            }
        }

        course1.course.SetActive(true);
        holds1 = course1.holds;
        holds1[holds1.Length - 1].SetSprite(buttonSprite);

        GameObject course = Instantiate(course1.course, course1.course.transform.position + new Vector3(45f, 0f, 0f), Quaternion.identity);

        course2 = new Course();
        course2.name = course1.name;
        course2.course = course;

        holds2 = course2.holds;

        p1.SetHolds(holds1);
        p2.SetHolds(holds2);

        for (int i = 0; i < holds1.Length; i++)
        {
            //holds1[i].button = p1.GetButton(holds1[i].button);
            if (randomiseButtons)
            {
                holds1[i].RandomiseButton();
            }
            holds1[i].UpdateText();
            holds1[i].UpdateArrow();
        }
        for (int i = 0; i < holds2.Length; i++)
        {
            //holds2[i].button = p2.GetButton(holds2[i].button);
            if (randomiseButtons)
            {
                holds2[i].button = holds1[i].button;
            }
            holds2[i].UpdateText();
            holds2[i].UpdateArrow();
        }

        wall.transform.localScale = new Vector3(wall.transform.localScale.x, (holds1[holds1.Length - 1].transform.position.y + paddingTop), wall.transform.localScale.z);
        wall.transform.position = new Vector3(wall.transform.position.x, wall.transform.localScale.y / 2f, wall.transform.position.z);

        Debug.Log("Current Record For Course " + currentCourseName + ": " + PlayerPrefs.GetFloat("Speed Climbing Course " + currentCourseName + " Record", 100f).ToString("n3") + "s");
    }

    private void Start()
    {
        cameraStartY = camera1.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!countdownStarted)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                countdownStarted = true;
                t = countdown;
            }
        }
        else if (!started)
        {
            t -= Time.deltaTime;
            SetCentreText(Mathf.CeilToInt(t).ToString());

            if (t <= 0f)
            {
                started = true;
                SetCentreText("GO!");
            }
        }
        else
        {
            raceTime += Time.deltaTime;

            t -= Time.deltaTime;
            if (t <= -1f)
            {
                SetCentreText("");
            }
        }

        UpdateCrowds();
    }

    private void SetCentreText(string text)
    {
        centreText.text = text;
        centreTextDropShadow.text = text;
    }

    public bool CountdownHasStarted()
    {
        return countdownStarted;
    }
    public float GetCountdown()
    {
        return t;
    }
    public bool RaceHasStarted()
    {
        return started;
    }

    public void FalseStart()
    {
        SetCentreText("FALSE START");
        countdownStarted = false;
        started = false;
    }

    private void UpdateCrowds()
    {
        crowd1.transform.position = new Vector3(crowd1.transform.position.x, crowdStartY - (camera1.transform.position.y - cameraStartY) * parallaxScalar * 2f, crowd1.transform.position.z);
        crowd2.transform.position = new Vector3(crowd2.transform.position.x, crowdStartY - (camera2.transform.position.y - cameraStartY) * parallaxScalar * 2f, crowd2.transform.position.z);
    }
}
