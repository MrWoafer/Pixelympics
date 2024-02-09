using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField]
    private bool served = false;
    [SerializeField]
    private float direction = 1f;

    [Header("References")]
    public GameObject configObj;
    private BeachVolleyballConfig config;
    public GameObject serverObj;
    public GameObject ballSprite;

    private float t = 0f;
    private float speedX;
    private float speedY;
    private float lastHitY = -2.83f;
    private float lastHitX = -7.35f;
    private bool landed = false;

    private bool displayedInOrOut = false;
    private bool displayedTooManyHits = false;

    private string lastHitBy = "";
    private string lastHitByTeam = "";
    private int teamHitCount = 0;

    private float landingX = 0f;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<BeachVolleyballConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        if (served)
        {
            //t += Time.deltaTime;
            t += Time.deltaTime * config.ballSpeed;

            //transform.Rotate(new Vector3(0f, 0f, -direction * config.ballRotationSpeed));
            if (!landed)
            {
                ballSprite.transform.Rotate(new Vector3(0f, 0f, -direction * config.ballRotationSpeed));
            }

            if (transform.position.y > config.groundY && !landed)
            {
                //transform.Translate(new Vector3(direction * speedX, 0f, 0f));
                transform.Translate(new Vector3(direction * speedX * config.ballSpeed, 0f, 0f));
                transform.position = new Vector3(transform.position.x, lastHitY + speedY * t - 0.5f * config.ballGravity * t * t, transform.position.z);
            }
            else
            {
                landed = true;

                if (!displayedInOrOut)
                {
                    if (transform.position.x <= config.outX && transform.position.x >= -config.outX)
                    {
                        Debug.Log("IN!");
                    }
                    else
                    {
                        Debug.Log("OUT!");
                    }
                }

                displayedInOrOut = true;
            }
        }
        else
        {
            transform.position = new Vector3(serverObj.transform.position.x + 0.65f, serverObj.transform.position.y + 0.17f, 0f);
        }
    }

    public void Serve(string hitBy, string hitbyTeam)
    {
        served = true;

        t = 0f;
        //lastHitY = -2.83f;
        lastHitY = transform.position.y;
        lastHitX = transform.position.x;

        speedX = config.ballServeSpeedX;
        speedY = config.ballServeSpeedY;

        lastHitBy = hitBy;

        UpdateTeamHitCount(hitbyTeam);

        lastHitByTeam = hitbyTeam;

        CalculateLandingXCoord();

        //Debug.Log("The serve will land at x = " + GetLandingXCoord().ToString());
    }
    public bool IsServed()
    {
        return served;
    }

    public void Hit(float hitDirection, float hitTime, bool softShot, string hitBy, string hitbyTeam)
    {
        direction = hitDirection;
        //speedX = hitDirection * config.ballSpeedX;
        //speedX = config.ballSpeedX;
        //speedY = config.ballSpeedY;

        speedX = config.ballMaxSpeedX - (hitTime / config.hitDuration) * (config.ballMaxSpeedX - config.ballMinSpeedX);
        speedY = config.ballMaxSpeedY - (hitTime / config.hitDuration) * (config.ballMaxSpeedY - config.ballMinSpeedY);

        if (softShot)
        {
            speedX *= config.softShotMultiplier;
        }

        t = 0f;
        lastHitY = transform.position.y;
        lastHitX = transform.position.x;

        lastHitBy = hitBy;

        UpdateTeamHitCount(hitbyTeam);

        lastHitByTeam = hitbyTeam;

        CalculateLandingXCoord();
    }

    public void Block(float hitDirection, string hitBy, string hitbyTeam)
    {
        direction = hitDirection;

        speedX = config.blockSpeedX;
        speedY = config.blockSpeedY;

        t = 0f;
        lastHitY = transform.position.y;
        lastHitX = transform.position.x;

        lastHitBy = hitBy;

        UpdateTeamHitCount(hitbyTeam);

        lastHitByTeam = hitbyTeam;

        CalculateLandingXCoord();
    }

    public float GetDirection()
    {
        return direction;
    }
    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }
    public bool IsInAir()
    {
        return !landed;
    }

    public string GetLastHitBy()
    {
        return lastHitBy;
    }
    public string GetLastTeamHitBy()
    {
        return lastHitByTeam;
    }
    public int GetTeamHitCount()
    {
        return teamHitCount;
    }

    private void UpdateTeamHitCount(string hitByTeam)
    {
        if (hitByTeam == lastHitByTeam)
        {
            teamHitCount += 1;
        }
        else
        {
            teamHitCount = 1;
        }

        if (teamHitCount > config.maxNumHitsPerTeam && !displayedTooManyHits)
        {
            displayedTooManyHits = true;

            Debug.Log("TOO MANY HITS FROM " + hitByTeam + " TEAM!");
        }
    }

    public int GetSideOfNet()
    {
        if (transform.position.x >= 0f)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public float GetLandingXCoord()
    {
        //return lastHitX + speedX * 2 * speedY / config.ballGravity;
        //return lastHitX + speedX * 2 * speedY / config.ballGravity / Time.fixedDeltaTime;
        //return lastHitX + direction * config.ballSpeed * speedX * 2 * speedY / config.ballGravity / Time.fixedDeltaTime;
        //return lastHitX + direction * config.ballSpeed * speedX * 2f * speedY / config.ballGravity / Time.fixedDeltaTime;
        //return lastHitX + direction * config.ballSpeed * speedX * 2f * speedY / config.ballGravity / Time.deltaTime;

        //return landingX;

        /// Add Epsilon to make sure it doesn't mistake the position it was hit from as the landing point
        float tempT = t + float.Epsilon;
        float tempX = transform.position.x;

        while (lastHitY + speedY * tempT - 0.5f * config.ballGravity * tempT * tempT > -3f)
        {
            tempT += Time.deltaTime;
            tempX += direction * speedX;
        }

        return tempX;
    }

    public float GetFallingXCoordForY(float y)
    {
        /// Add Epsilon to make sure it doesn't mistake the position it was hit from as the landing point
        float tempT = t + float.Epsilon;
        float tempX = transform.position.x;

        while (lastHitY + speedY * tempT - 0.5f * config.ballGravity * tempT * tempT > y || tempT <= speedY / config.ballGravity)
        {
            tempT += Time.deltaTime;
            tempX += direction * speedX;
        }

        return tempX;
    }

    private void CalculateLandingXCoord()
    {
        /*landingX = 0f;

        for (int i = 0; i < 10; i++)
        {
            float tempT = t + float.Epsilon;
            float tempX = transform.position.x;

            while (lastHitY + speedY * tempT - 0.5f * config.ballGravity * tempT * tempT > -3f)
            {
                tempT += Time.deltaTime;
                tempX += direction * speedX;
            }

            landingX += tempX;
        }

        landingX /= 10f;*/
    }

    public bool IsFalling()
    {
        return t > speedY / config.ballGravity;
    }

    public bool IsGoingOut()
    {
        if (direction == 1)
        {
            return GetLandingXCoord() > config.outX;
        }
        else
        {
            return GetLandingXCoord() < -config.outX;
        }
    }

    public void Spike(float spikeDirection, float distanceToNet, bool isHardShot, string hitBy, string hitbyTeam)
    {
        direction = spikeDirection;

        /*if (!isHardShot)
        {
            speedX = config.spikeSpeedX;
            speedY = config.minSpikeSpeedY + (config.minSpikeSpeedY - config.minSpikeSpeedY) * (config.midwayX - distanceToNet) / config.midwayX;
        }
        else
        {
            speedX = config.netTipSpeedX;
            speedY = config.netTipSpeedY;
        }*/
        speedX = config.spikeSpeedX;
        speedY = config.minSpikeSpeedY + (config.minSpikeSpeedY - config.minSpikeSpeedY) * (config.midwayX - distanceToNet) / config.midwayX;

        t = 0f;
        lastHitY = transform.position.y;
        lastHitX = transform.position.x;

        lastHitBy = hitBy;

        UpdateTeamHitCount(hitbyTeam);

        lastHitByTeam = hitbyTeam;

        CalculateLandingXCoord();
    }

    public void UpdateConfigReference()
    {
        config = configObj.GetComponent<BeachVolleyballConfig>();
    }
}
