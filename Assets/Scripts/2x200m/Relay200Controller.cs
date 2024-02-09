using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relay200Controller : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName;
    public bool isAI = false;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";

    private int lastButton = 0;

    private float speed = Relay200Config.minSpeed;

    [Header("References")]
    public GameObject raceControllerObj;
    private Relay200RaceController raceController;

    private bool running = false;
    private bool finished = false;
    private bool usedDip = false;
    private float dipCountdown = -1f;
    private bool dipFinished = false;

    private float willStartAt;
    private float speedMultiplier;
    private float dipX;
    private float maxSpeed = 0f;
    private float startDist;

    private int penalties = 0;
    private int presses = 0;
    private float finishTime = 0f;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;

    private bool alreadySet = false;
    private bool doneBend2 = false;

    [Header("Relay")]
    public bool isSecond = false;
    public bool tagged = false;
    private float tagCountdown = 0f;
    private bool disqualified = false;
    public GameObject partner;

    private bool startedHomeStraight = false;

    // Start is called before the first frame update
    void Start()
    {
        raceController = raceControllerObj.GetComponent<Relay200RaceController>();

        if (isAI)
        {
            willStartAt = Random.Range(0.2f, 0.5f);
            speedMultiplier = Random.Range(Relay200Config.aiMinSpeedMultiplier, Relay200Config.aiMaxSpeedMultiplier);
            dipX = Random.Range(Relay200Config.finishX - 0.9f, Relay200Config.finishX - 0.5f);
            maxSpeed = Random.Range(Relay200Config.aiMinMaxSpeed, Relay200Config.aiMaxMaxSpeed);
            startDist = Random.Range(Relay200Config.aiMinStartDist, Relay200Config.aiMaxStartDist);
        }
        else
        {
            maxSpeed = Relay200Config.maxSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (raceController.HasStarted() && !alreadySet)
        {
            anim.SetTrigger("GetSet");
            alreadySet = true;
        }

        if (!isAI)
        {
            if ((Input.GetKeyDown(button1) || Input.GetKeyDown(button2)) && lastButton == 0)
            {
                if (raceController.GetCountdown() > 0f)
                {
                    raceController.FalseStart(playerName);
                }
                else
                {
                    running = true;
                    speed = CalculateStartingSpeed(Mathf.Abs(raceController.GetCountdown()));

                    anim.SetTrigger("Start");

                    Debug.Log(playerName + ", your reaction time: " + Mathf.Abs(raceController.GetCountdown()).ToString());
                    presses += 1;

                    if (isSecond)
                    {
                        tagCountdown = Relay200Config.tagTime;
                    }
                }
            }

            if (running)
            {
                if (Input.GetKeyDown(button1))
                {
                    if (lastButton == 1)
                    {
                        speed -= Relay200Config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else if (!(!isSecond && tagged) && !disqualified)
                    {
                        speed += Relay200Config.speedChange;
                        lastButton = 1;
                        presses += 1;
                    }
                }
                else if (Input.GetKeyDown(button2))
                {
                    if (lastButton == 2)
                    {
                        speed -= Relay200Config.speedPenalty;
                        penalties += 1;
                        presses += 1;
                    }
                    else if (!(!isSecond && tagged) && !disqualified)
                    {
                        speed += Relay200Config.speedChange;
                        lastButton = 2;
                        presses += 1;
                    }
                }
                else if (Input.GetKeyDown(button3) && !usedDip)
                {
                    usedDip = true;
                    dipCountdown = Relay200Config.dipDuration;
                    speed = 0.04f;
                    presses += 1;
                    anim.SetTrigger("Dip");
                }
            }
        }
        else
        {
            if (running)
            {
                if (transform.position.x > dipX && !usedDip && doneBend2)
                {
                    usedDip = true;
                    dipCountdown = Relay200Config.dipDuration;
                    speed = 0.04f;
                    anim.SetTrigger("Dip");
                }
            }
            else if ((raceController.GetCountdown() < -willStartAt && !isSecond) || (isSecond && Mathf.Sqrt(Mathf.Pow(transform.position.x - partner.transform.position.x, 2) + Mathf.Pow(transform.position.y - partner.transform.position.y, 2)) < startDist))
            {
                running = true;
                speed = CalculateStartingSpeed(willStartAt);
                anim.SetTrigger("Start");
                Debug.Log(playerName + ", your reaction time: " + willStartAt.ToString());

                if (isSecond)
                {
                    tagCountdown = Relay200Config.tagTime;
                }
            }
        }

        speed -= Relay200Config.speedDecay;

        if (!usedDip || dipFinished)
        {
            if (speed < Relay200Config.minSpeed)
            {
                speed = Relay200Config.minSpeed;
            }
            else if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
        }
        if (dipCountdown < 0 && !dipFinished && usedDip)
        {
            speed = 0.005f;
            dipFinished = true;
            anim.SetTrigger("DipEnded");
        }

        anim.SetBool("IsMoving", speed > 0);
    }

    public void FixedUpdate()
    {
        if (isAI && running)
        {
            if (!usedDip && !(!isSecond && tagged) && !disqualified)
            {
                speed += Relay200Config.speedChange * (speedMultiplier + Random.Range(-0.001f, 0.001f));
            }
            if (!isSecond && tagged)
            {
                speed -= Relay200Config.speedDecay;
            }
        }

        //speed = 0.02f;
        if (Relay200Config.straightX1 <= transform.position.x && transform.position.x < 0 && transform.position.y < 0 && !startedHomeStraight)
        {
            if (isAI)
            {
                ChangeHomeStraightSpeed();
            }
            else
            {
                maxSpeed = Relay200Config.homeStraightMaxSpeed;
            }
            startedHomeStraight = true;
        }
        if (transform.position.x >= Relay200Config.straightX2)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Relay200Config.anglePlus + Mathf.Rad2Deg * (Mathf.PI / 2 + Mathf.Atan(transform.position.y / (transform.position.x - Relay200Config.straightX2))));
        }
        else if (transform.position.x <= Relay200Config.straightX1)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Relay200Config.anglePlus + Mathf.Rad2Deg * (3 * Mathf.PI / 2 - Mathf.Atan(transform.position.y / (Relay200Config.straightX1 - transform.position.x))));
            doneBend2 = true;
        }

        transform.Translate(new Vector3(speed, 0f, 0f));

        if (usedDip)
        {
            dipCountdown -= Time.fixedDeltaTime;
        }
        if (running)
        {
            tagCountdown -= Time.fixedDeltaTime;
            
            if (tagCountdown < 0 && isSecond && !tagged && !disqualified)
            {
                Debug.Log(playerName + " has been disqualified!");
                disqualified = true;
            }
        }

        if (transform.position.x >= Relay200Config.finishX - 0.5 && !finished && doneBend2)
        {
            finishTime = raceController.GetTime();

            raceController.Finish(playerName, finishTime);

            finished = true;
            Debug.Log(playerName + ", your time: " + finishTime.ToString());

            if (!isAI)
            {
                Debug.Log(playerName + ", you got " + penalties.ToString() + " penalties out of " + presses.ToString() + " presses.");
                Debug.Log(playerName + ", you did an average of " + (presses / finishTime).ToString() + " presses per second.");
            }
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float CalculateStartingSpeed(float time)
    {
        return Relay200Config.maxSpeed / 2.5f;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.GetComponent<Relay200Controller>().playerName == playerName)
        {
            tagged = true;

            if (isSecond)
            {
                Debug.Log(playerName + " has passed it on!");
                Debug.Log(playerName + " had " + tagCountdown.ToString() + " seconds left to do so.");
            }
        }
    }

    private void ChangeHomeStraightSpeed()
    {
        maxSpeed = Random.Range(Relay200Config.aiMinHomeStraightMaxSpeed, Relay200Config.aiMaxHomeStraightMaxSpeed);
    }
}
