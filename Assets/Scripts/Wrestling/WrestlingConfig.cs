using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrestlingConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool developmentMode = true;

    [Header("Player Settings")]
    public float damage = 1f;
    public float playerSpeed = 5f;
    public float hitRange = 0.2f;
    public float stunTime = 1f;
    public float queuedButtonTime = 0.2f;
    public float minDistanceBetweenPlayers = 2.7f;

    [Header("Ball Settings")]
    public float rotationSpeed = 90f;

    [Header("Referee Settings")]
    public float refDefaultZ = 6.4f;
    public float refCloseZ = 1f;
    public float refTimeWarningZ = 3.4f;
    public float refRetreatSpeed = 5f;
    public bool refIntervenes = true;
    public float timeWarningTime = 10f;
    public float timePenaltyTime = 20f;

    [Header("Mat Settings")]
    public float oneFootOffX = 5.8f;
    public float bothFeetOffX = 7f;

    [Header("Round Settings")]
    public float roundDuration = 120f;

    [Header("Movement Settings")]
    public bool turnWhenPastOpponent = true;
    public float walkEndRadius = 0.3f;

    [Header("Attack Settings")]
    public int penaltyPointLoss = 3;
    public bool blocksBlockHigh = false;

    [Header("Grade Settings")]
    public Color[] gradeBeltColours = new Color[] { Color.white, Color.black };

    [Header("Sound Settings")]
    public int p1HitGruntNum = 1;
    public int p2HitGruntNum = 1;
    public int p1HurtGruntNum = 6;
    public int p2HurtGruntNum = 6;
    public int shockNum = 9;
    public int bigShockNum = 8;
    public int cheerNum = 6;
    public int refGoNum = 1;
    public int refQuickGoNum = 1;
    public int refPointNum = 1;
    public int refPenaltyNum = 2;
    public int refTimeWarningNum = 2;
    public int refGoldenPointNum = 1;
    public int refStopNum = 1;
}
