using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachVolleyballConfig : MonoBehaviour
{
    [Header("Player Config")]
    public float speed = 0.1f;
    public float hitDuration = 0.2f;
    public float spikeDuration = 0.3f;
    public float playerGravity = 9.81f;
    public float playerJumpSpeed = 7f;

    [Header("Ball Config")]
    public float ballGravity = 9.81f;
    //public float ballServeSpeedX = 0.1f;
    public float ballServeSpeedX = 0.045f;
    //public float ballServeSpeedY = 0.1f;
    public float ballServeSpeedY = 10f;
    public float ballRotationSpeed = 5f;

    public float ballMinSpeedX = 0.04f;
    public float ballMaxSpeedX = 0.05f;
    public float ballMinSpeedY = 9f;
    public float ballMaxSpeedY = 11f;

    public float blockSpeedX = 0.03f;
    public float blockSpeedY = 2f;
    public float softShotMultiplier = 0.3f;

    [Header("General Settings")]
    public float groundY = -3f;
    public float outX = 11f;

    [Header("Gameplay Settings")]
    public int maxNumHitsPerTeam = 2;
    public bool increaseBallSpeed = false;
    public float ballSpeed = 1f;
    public float ballSpeedIncrease = 0.01f;
    public float diveDoubleTapWindow = 0.3f;
    public float diveSpeed = 1f;
    public float diveDuration = 0.5f;
    public float diveCoolDown = 0.5f;
    public float divePowerMultiplier = 0.3f;
    public float spikeSpeedX = 0.05f;
    public float maxSpikeSpeedY = -2f;
    public float minSpikeSpeedY = 2f;
    public float netTipSpeedX = 0.03f;
    public float netTipSpeedY = 8f;

    [Header("AI Settings")]
    public float aiLocationTolerance = 0.1f;
    public float midwayX = 5.5f;
    public float aiFrontX = 3.25f;
    public float aiBackX = 8f;
    public float aiDistanceToBall = 1f;
    public float aiServeCountdown = 3f;
    public float aiDiveHeight = -1f;
    public float aiMinDiveDistance = 2f;
    public float aiMaxDiveDistance = 4f;
    public float aiTeammateMinDiveDistance = 1f;
    public bool aiLetGoOut = true;
    public float aiLeaveToGoOutTolerance = 2f;
    public float aiSpikeHeight = 0.3f;
    public float aiSpikeMinHeight = -1f;
    public float aiSpikeLocationTolerance = 1f;
    public float aiBlockX = 1f;
    public float aiBallBlockTolerance = 1f;

    [Header("References")]
    public GameObject ballObj;
    private BallController ball;

    public void Start()
    {
        ball = ballObj.GetComponent<BallController>();
    }

    public void Update()
    {
        if (ball.IsServed() && increaseBallSpeed)
        {
            ballSpeed += ballSpeedIncrease;
        }
    }

    public void ResetConfig()
    {
        ball = ballObj.GetComponent<BallController>();
    }
}
