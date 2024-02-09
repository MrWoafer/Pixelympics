using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Player Settings")]
    public float startingRotationSpeed = 1f;
    public float rotationGain = 50f;
    public float moveSpeed = 3f;
    public float foulAngleDeceleration = 400f;
    public float foulMovementDeceleration = 5f;

    [Header("AI Settings")]
    public float aiStartTime = 1f;
    [Header("AI Wait Time")]
    public float aiOlympicMaxWaitTime = -45f;
    public float aiOlympicMinWaitTime = -45f;
    public float aiHardMaxWaitTime = -45f;
    public float aiHardMinWaitTime = -45f;
    public float aiMediumMaxWaitTime = -45f;
    public float aiMediumMinWaitTime = -45f;
    public float aiEasyMaxWaitTime = -45f;
    public float aiEasyMinWaitTime = -45f;
    [Header("AI Horizontal Angle")]
    public float aiOlympicMaxHAngle = -45f;
    public float aiOlympicMinHAngle = -45f;
    public float aiHardMaxHAngle = -45f;
    public float aiHardMinHAngle = -45f;
    public float aiMediumMaxHAngle = -45f;
    public float aiMediumMinHAngle = -45f;
    public float aiEasyMaxHAngle = -45f;
    public float aiEasyMinHAngle = -45f;
    [Header("AI Vertical Angle Angle")]
    public float aiOlympicMaxVAngle = -45f;
    public float aiOlympicMinVAngle = -45f;
    public float aiHardMaxVAngle = -45f;
    public float aiHardMinVAngle = -45f;
    public float aiMediumMaxVAngle = -45f;
    public float aiMediumMinVAngle = -45f;
    public float aiEasyMaxVAngle = -45f;
    public float aiEasyMinVAngle = -45f;

    [Header("Spin Settings")]
    public float numOfSpins = 5;
    public float speedPenalty = 100f;
    public float horizontalAngleBound = 50f;

    [Header("Hammer Settings")]
    public float hammerMass = 100f;
    public float speedUMultiplier = 1f;
    public float speedRMultiplier = 1f;

    [Header("Physics Settings")]
    public float gravity = 9.81f;

    [Header("References")]
    public Camera cam;
    private CameraFollow camFollow;
    private ParallaxCamera parallaxCam;
    public Transform throwPoint;
    public float throwPointX
    {
        get
        {
            return throwPoint.position.x;
        }
    }
    public Transform sideOnArea;
    public Rigidbody2D sideOnHammer;
    public Transform outOfRing;
    public float outOfRingX
    {
        get
        {
            return outOfRing.position.x;
        }
    }
    public Transform throwMeasurementStart;

    private void Start()
    {
        //cam = Camera.main;
        camFollow = cam.GetComponent<CameraFollow>();
        parallaxCam = cam.GetComponent<ParallaxCamera>();
    }

    public void MoveCameraToSideOn()
    {
        cam.transform.position = new Vector3(sideOnArea.position.x, sideOnArea.position.y, transform.position.z);
        parallaxCam.ResetParallax();
    }

    public void UnfreezeHammer()
    {
        //sideOnHammer.isKinematic = false;
        //sideOnHammer.mass = hammerMass;

        camFollow.followTarget = true;

        foreach (Rigidbody2D chain in sideOnHammer.GetComponentsInChildren<Rigidbody2D>())
        {
            if (chain.tag == "Chain")
            {
                chain.isKinematic = false;
            }
        }
    }
}
