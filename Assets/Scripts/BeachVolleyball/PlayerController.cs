using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerName;
    public string team;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Medium;
    public float direction = 1f;
    public bool isServing = false;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "e";
    public string button4 = "w";
    public string button5 = "s";
    public string button6 = "q";

    private int lastButton = 0;

    private float speed = 0f;

    [Header("References")]
    public GameObject configObj;
    private BeachVolleyballConfig config;
    public GameObject ballObj;
    private BallController ball;
    public GameObject teammateObj;
    private PlayerController teammate;

    [Header("Animator")]
    [SerializeField]
    public Animator anim;

    private bool hitting = false;
    private float hitTime = 0f;
    private float hitDirection = 1f;

    private bool softShot = false;

    private bool served = false;

    private bool jumping = false;
    private float jumpTime = 0f;

    private float doubleTapLeftCooldown = 0f;
    private float doubleTapRightCooldown = 0f;

    private bool diving = false;
    public bool divingHitWindow = false;
    private float diveCountdown = 0f;
    private float diveDirection = 1f;

    private int lastKey = 0;

    private float aiServeCountdown = 0f;

    private bool spiking = false;
    private float spikeTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<BeachVolleyballConfig>();
        ball = ballObj.GetComponent<BallController>();
        teammate = teammateObj.GetComponent<PlayerController>();

        if (isAI && isServing)
        {
            aiServeCountdown = config.aiServeCountdown;
        }
    }

    // Update is called once per frame
    void Update()
    {
        /// Controls
        if (!isAI)
        {
            if (Input.GetKey(button5))
            {
                softShot = true;
            }
            else
            {
                softShot = false;
            }

            if (Input.GetKeyDown(button1) && !diving && !jumping)
            {
                if (doubleTapLeftCooldown <= 0f)
                {
                    doubleTapLeftCooldown = config.diveDoubleTapWindow;
                    lastKey = 1;
                }
                else if (lastKey == 1)
                {
                    Dive(-1f);
                }
                else
                {
                    lastKey = 1;
                }
            }
            if (Input.GetKeyDown(button2) && !diving && !jumping)
            {
                if (doubleTapRightCooldown <= 0f)
                {
                    doubleTapRightCooldown = config.diveDoubleTapWindow;
                    lastKey = 2;
                }
                else if (lastKey == 2)
                {
                    Dive(1f);
                }
                else
                {
                    lastKey = 2;
                }
            }

            if (Input.GetKey(button1) && !jumping && !diving)
            {
                transform.Translate(new Vector3(-config.speed, 0f, 0f));
            }
            if (Input.GetKey(button2) && !jumping && !diving)
            {
                transform.Translate(new Vector3(config.speed, 0f, 0f));
            }

            if (Input.GetKeyDown(button3))
            {
                if (ball.IsServed() || !isServing)
                {
                    if (!jumping)
                    {
                        if (!hitting)
                        {
                            hitDirection = 1f;
                            Hit();
                        }
                    }
                    else if (ball.GetLastTeamHitBy() == team)
                    {
                        hitDirection = 1f;
                        Spike();
                    }
                    else if (false)
                    //else if (true)
                    {
                        hitDirection = 1f;
                        Spike();
                    }
                }
                else if (isServing && !served)
                {
                    served = true;
                    Hit();
                    ball.Serve(name, team);
                }
            }

            if (Input.GetKeyDown(button6))
            {
                if (ball.IsServed() || !isServing)
                {
                    if (!jumping)
                    {
                        if (!hitting)
                        {
                            hitDirection = -1f;
                            Hit();
                        }
                    }
                }
            }

            if (Input.GetKeyDown(button4))
            {
                if (!jumping)
                {
                    Jump();
                }
            }
        }
        /// AI Brain
        else if (ball.IsInAir())
        {
            if (!diving && !jumping && !spiking)
            {
                if (!ball.IsServed() && isServing)
                {
                    aiServeCountdown -= Time.deltaTime;

                    if (aiServeCountdown <= 0f)
                    {
                        served = true;
                        Hit();
                        ball.Serve(name, team);
                    }
                }
                if (ball.GetSideOfNet() == direction || !ball.IsServed())
                {
                    if (ball.GetLastTeamHitBy() != team && ball.GetTeamHitCount() == 2 && Mathf.Sign(ball.GetLandingXCoord()) == direction && Mathf.Abs(transform.position.x) < Mathf.Abs(teammateObj.transform.position.x))
                    {
                        MoveTo(-direction * config.aiBlockX);
                    }
                    else if (ball.GetLastTeamHitBy() != team && ball.GetTeamHitCount() == 3 && Mathf.Abs(transform.position.x) < Mathf.Abs(teammateObj.transform.position.x))
                    {
                        MoveTo(-direction * config.aiBlockX);

                        if (Mathf.Abs(ball.transform.position.x) < config.aiBallBlockTolerance && Mathf.Abs(transform.position.x + direction * config.aiBlockX) < config.aiLocationTolerance)
                        {
                            Jump();
                        }
                    }
                    else
                    {
                        AIFrontAndBack();
                    }
                }
                else
                {
                    if (ball.GetLastTeamHitBy() != team)
                    {
                        if (!((ball.IsGoingOut() && config.aiLetGoOut) && Mathf.Abs(transform.position.x - ball.GetLandingXCoord()) < config.aiLeaveToGoOutTolerance))
                        {
                            if (direction == 1)
                            {
                                if (Mathf.Sign(ball.GetLandingXCoord() + config.midwayX) == Mathf.Sign(transform.position.x + config.midwayX))
                                {
                                    MoveTo(ball.GetLandingXCoord());
                                    //MoveTo(ball.transform.position.x);
                                }
                            }
                            else
                            {
                                if (Mathf.Sign(ball.GetLandingXCoord() - config.midwayX) == Mathf.Sign(transform.position.x - config.midwayX))
                                {
                                    MoveTo(ball.GetLandingXCoord());
                                    //MoveTo(ball.transform.position.x);
                                }
                            }

                            AIReadyToDive();
                            AIReadyToHit(true, true);
                        }
                    }
                    else if (ball.GetTeamHitCount() == 1)
                    {
                        if (ball.GetLastHitBy() == name)
                        {
                            AIFrontAndBack();
                        }
                        else
                        {
                            MoveTo(ball.GetLandingXCoord());

                            AIReadyToDive();
                            AIReadyToHit(true, true);
                        }
                    }
                    else if (ball.GetTeamHitCount() == 2)
                    {
                        if (ball.GetLastHitBy() == name)
                        {
                            //MoveTo(-direction * config.midwayX);
                            AIFrontAndBack();
                        }
                        else
                        {
                            //if (Mathf.Abs())
                            //MoveTo(ball.GetLandingXCoord());

                            /*if (ball.GetLastHitBy() != name && Mathf.Abs(transform.position.x) < config.midwayX)
                            {
                                if (Mathf.Abs(ball.GetLandingXCoord()) < config.midwayX - 0.5f)
                                {
                                    if (Mathf.Abs(ball.transform.position.x) < config.midwayX - 0.5f)
                                    {
                                        if (Mathf.Sign(ball.GetLandingXCoord() - transform.position.x) == -direction)
                                        {
                                            MoveTo(ball.transform.position.x);
                                        }
                                        else if (Mathf.Sign(ball.transform.position.x - transform.position.x) == direction)
                                        {
                                            MoveTo(ball.transform.position.x);
                                        }
                                    }
                                    else
                                    {
                                        MoveTo(direction * 0.75f * config.midwayX);
                                    }

                                    AIReadyToSpike();
                                }
                                else
                                {
                                    MoveTo(ball.GetLandingXCoord());
                                }
                            }
                            else
                            {
                                MoveTo(ball.GetLandingXCoord());
                            }*/

                            MoveTo(ball.GetFallingXCoordForY(config.aiSpikeHeight));

                            AIReadyToSpike();

                            AIReadyToDive();
                            AIReadyToHit(false, false);
                        }
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (hitting)
        {
            hitTime -= Time.fixedDeltaTime;

            if (hitTime <= 0f)
            {
                hitting = false;
            }
        }
        if (spiking)
        {
            spikeTime -= Time.fixedDeltaTime;

            if (spikeTime <= 0f)
            {
                spiking = false;
                anim.SetTrigger("SpikeEnd");
            }
        }

        if (jumping)
        {
            jumpTime += Time.fixedDeltaTime;

            transform.position = new Vector3(transform.position.x, -3f + config.playerJumpSpeed * jumpTime - 0.5f * config.playerGravity * jumpTime * jumpTime, 0f);

            if (transform.position.y <= -3)
            {
                jumping = false;
                transform.position = new Vector3(transform.position.x, -3f, transform.position.z);
                anim.SetTrigger("BlockEnd");
            }
        }

        if (doubleTapRightCooldown >= 0f)
        {
            doubleTapRightCooldown -= Time.fixedDeltaTime;
        }
        if (doubleTapLeftCooldown >= 0f)
        {
            doubleTapLeftCooldown -= Time.fixedDeltaTime;
        }

        if (diving)
        {
            if (divingHitWindow)
            {
                transform.Translate(new Vector3(diveDirection * config.diveSpeed, 0f, 0f));
            }

            diveCountdown -= Time.fixedDeltaTime;

            if (diveCountdown <= 0f)
            {
                divingHitWindow = false;
                anim.SetTrigger("DiveEnd");
            }

            if (diveCountdown <= -config.diveCoolDown)
            {
                diving = false;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D col)
    //public void OnTriggerEnter2D(Collider2D col)
    {
        //if (ball.IsServed() && ball.GetDirection() != direction && ball.IsInAir())
        if (ball.IsServed() && ball.GetLastHitBy() != name && ball.IsInAir())
        {
            if (jumping)
            {
                //ball.Hit(direction, config.hitDuration);
                if (spiking)
                {
                    bool hardSpike = true;
                    if (isAI)
                    {
                        hardSpike = Random.Range(0f, 1f) < 0.5f;

                        if (hardSpike)
                        {
                            ball.Spike(direction, Mathf.Abs(transform.position.x), hardSpike, name, team);
                        }
                        else
                        {
                            ball.Block(direction, name, team);
                        }
                    }
                    else
                    {
                        ball.Spike(direction, Mathf.Abs(transform.position.x), true, name, team);
                    }                  
                }
                else
                {
                    ball.Block(direction, name, team);
                }
            }
            else
            {
                //if (hitting && ball.IsServed() && ball.GetDirection() != direction && ball.IsInAir())
                if (isAI)
                {
                    if (diving)
                    {
                        ball.Hit(direction, config.divePowerMultiplier, softShot, name, team);
                    }
                    else
                    {
                        if (hitting || AIDifficultyProbabilityOfHit())
                        {
                            AIHitBall();
                        }
                    }
                }
                else
                {
                    if (hitting)
                    {
                        ball.Hit(direction * hitDirection, hitTime, softShot, name, team);
                    }
                    else
                    {
                        if (diving)
                        {
                            //ball.Hit(direction, 0.5f * (config.ballMaxSpeedX + config.ballMinSpeedX), false, name, team);
                            //ball.Hit(direction, config.divePowerMultiplier * config.ballMaxSpeedX + (1 - config.divePowerMultiplier) * config.ballMinSpeedX, false, name, team);
                            //ball.Hit(direction, config.divePowerMultiplier, false, name, team);
                            ball.Hit(direction, config.divePowerMultiplier, softShot, name, team);
                        }
                    }
                }
            }
        }
    }

    private void Hit()
    {
        hitting = true;
        hitTime = config.hitDuration;

        anim.SetTrigger("Hit");
    }

    private void Spike()
    {
        spiking = true;
        spikeTime = config.spikeDuration;

        anim.SetTrigger("SpikeStart");
    }

    private void Jump()
    {
        jumping = true;
        jumpTime = 0f;

        anim.SetTrigger("BlockStart");
    }

    private void Dive(float diveDirectionParameter)
    {
        //transform.Translate(new Vector3(diveDirection * config.diveSpeed, 0f, 0f));
        hitting = false;
        hitTime = 0f;

        diveDirection = diveDirectionParameter;

        diving = true;
        divingHitWindow = true;
        diveCountdown = config.diveDuration;

        if (diveDirection == direction)
        {
            anim.SetTrigger("DiveStart");
        }
        else
        {
            anim.SetTrigger("DiveBackStart");
        }
    }

    private void MoveTo(float xCoord)
    {
        if (!diving && !jumping && !spiking)
        {
            if ((direction == 1 && xCoord < 0f) || (direction == -1 && xCoord > 0f))
            {
                if (transform.position.x < xCoord - config.aiLocationTolerance)
                {
                    transform.Translate(new Vector3(config.speed, 0f, 0f));
                }
                else if (transform.position.x > xCoord + config.aiLocationTolerance)
                {
                    transform.Translate(new Vector3(-config.speed, 0f, 0f));
                }
            }
        }
    }

    private float DistanceToBall()
    {
        return Mathf.Sqrt((ball.transform.position.x - transform.position.x) * (ball.transform.position.y - transform.position.y) + (ball.transform.position.y - transform.position.y));
    }

    private void AIReadyToHit(bool isPass, bool doSoftShot)
    {
        if (!diving && !hitting && !(ball.GetLastTeamHitBy() != team && ball.IsGoingOut() && config.aiLetGoOut) && !jumping && !spiking)
        {
            softShot = doSoftShot;

            if (isPass)
            {
                if (DistanceToBall() < config.aiDistanceToBall)
                {
                    if (direction == 1)
                    {
                        if (transform.position.x <= -config.midwayX)
                        {
                            hitDirection = 1f;
                        }
                        else
                        {
                            hitDirection = -1f;
                        }
                    }
                    else
                    {
                        if (transform.position.x >= config.midwayX)
                        {
                            hitDirection = 1f;
                        }
                        else
                        {
                            hitDirection = -1f;
                        }
                    }
                    Hit();
                }
            }
            else
            {
                hitDirection = 1f;
                Hit();
            }
        }
    }

    private void AIHitBall()
    {
        if (!hitting)
        {
            if (ball.GetLastTeamHitBy() == team && ball.GetTeamHitCount() == 3)
            {
                hitDirection = 1f;
            }
            else
            {
                hitDirection = -direction * Mathf.Sign(transform.position.x - teammateObj.transform.position.x);
            }

            Hit();
        }
        if (Mathf.Sign(transform.position.x + direction * config.midwayX) == -direction)
        {
            if (ball.GetLastTeamHitBy() != team)
            {
                ball.Hit(direction * hitDirection, Random.Range(0.4f, 0.8f) * config.hitDuration, softShot, name, team);
            }
            else if (ball.GetTeamHitCount() == 1 || ball.GetTeamHitCount() == 2)
            {
                ball.Hit(direction * hitDirection, Random.Range(0.4f, 0.8f) * config.hitDuration, softShot, name, team);
            }
            else if (ball.GetTeamHitCount() == 3)
            {
                ball.Hit(direction * hitDirection, Random.Range(0f, 0.5f) * config.hitDuration, softShot, name, team);
            }
        }
        else
        {
            if (ball.GetLastTeamHitBy() != team)
            {
                ball.Hit(direction * hitDirection, Random.Range(0.5f, 1f) * config.hitDuration, softShot, name, team);
            }
            else if (ball.GetTeamHitCount() == 1 || ball.GetTeamHitCount() == 2)
            {
                ball.Hit(direction * hitDirection, Random.Range(0.5f, 1f) * config.hitDuration, softShot, name, team);
            }
            else if (ball.GetTeamHitCount() == 3)
            {
                ball.Hit(direction * hitDirection, Random.Range(0f, 1f) * config.hitDuration, softShot, name, team);
            }
        }
    }

    private void DiveToBall()
    {
        /*if (Mathf.Sign(transform.position.x - ball.transform.position.x) == -direction)
        {
            Dive(1f);
        }
        else
        {
            Dive(-1f);
        }*/

        Dive((float)Mathf.Sign(ball.transform.position.x - transform.position.x));
    }

    private void AIReadyToDive()
    {
        //if (ball.IsFalling() && ball.transform.position.y < config.aiDiveHeight && Mathf.Abs(transform.position.x - ball.transform.position.x) > config.aiMinDiveDistance && ball.GetLastHitBy() != name && !diving && !jumping)
        if (ball.IsFalling() && ball.transform.position.y < config.aiDiveHeight && Mathf.Abs(transform.position.x - ball.GetLandingXCoord()) > config.aiMinDiveDistance && ball.GetLastHitBy() != name && !diving && !jumping)
        {
            //if ((Mathf.Abs(teammateObj.transform.position.x - ball.transform.position.x) > config.aiTeammateMinDiveDistance || !teammate.IsHitting()) && Mathf.Abs(transform.position.x - ball.transform.position.x) < config.aiMaxDiveDistance)
            //if (Mathf.Abs(teammateObj.transform.position.x - ball.transform.position.x) > config.aiTeammateMinDiveDistance && Mathf.Abs(transform.position.x - ball.transform.position.x) < config.aiMaxDiveDistance)
            if (Mathf.Abs(teammateObj.transform.position.x - ball.GetLandingXCoord()) > config.aiTeammateMinDiveDistance && Mathf.Abs(transform.position.x - ball.GetLandingXCoord()) < config.aiMaxDiveDistance)
            {
                Debug.Log("Dive!");
                DiveToBall();
            }
        }
    }

    public bool IsHitting()
    {
        return hitting;
    }

    private void AIFrontAndBack()
    {
        if (direction == 1)
        {
            if (teammateObj.transform.position.x >= -config.midwayX)
            {
                MoveTo(-config.aiBackX);
            }
            else
            {
                MoveTo(-config.aiFrontX);
            }
        }
        else
        {
            if (teammateObj.transform.position.x < config.midwayX)
            {
                MoveTo(config.aiBackX);
            }
            else
            {
                MoveTo(config.aiFrontX);
            }
        }
    }

    private void AIReadyToSpike()
    {
        if (!diving && !hitting && !jumping && !spiking)
        {
            if (Mathf.Abs(ball.GetLandingXCoord()) < config.midwayX && Mathf.Abs(transform.position.x - ball.transform.position.x) < config.aiSpikeLocationTolerance && ball.IsFalling() && ball.transform.position.y < config.aiSpikeHeight && ball.transform.position.y > config.aiSpikeMinHeight)
            {
                hitDirection = 1f;
                Jump();
                Spike();
            }
        }
    }

    private bool AIDifficultyProbabilityOfHit()
    {
        if (difficulty == Difficulty.Easy)
        {
            //return Random.Range(0f, 1f) < 0.2f;
            return Random.Range(0f, 1f) < 0.5f;
        }
        else if (difficulty == Difficulty.Medium)
        {
            //return Random.Range(0f, 1f) < 0.6f;
            return Random.Range(0f, 1f) < 0.9f;
        }
        else if (difficulty == Difficulty.Hard)
        {
            //return Random.Range(0f, 1f) < 0.9f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetPlayer()
    {
        ball = ballObj.GetComponent<BallController>();

        served = false;
        if (isAI && isServing)
        {
            aiServeCountdown = config.aiServeCountdown;
        }
    }
}
