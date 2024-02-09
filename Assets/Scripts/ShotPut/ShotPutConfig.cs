using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPutConfig : MonoBehaviour
{
    [Header("Development Settings")]
    public bool disableRecordEligibility = false;

    [Header("Shot Settings")]
    public float speedUMultiplier = 1f;
    public float speedRMultiplier = 1f;
    public readonly float shotLandingY = -3.7f;

    [Header("Power Bar Settings")]
    public int numOfSlideBarAttempts = 6;
    public float slideBarEndingWidth = 3f;
    public float slideBarShrinkAmount
    {
        get
        {
            return (slideBarStartingWidth - slideBarEndingWidth) / (numOfSlideBarAttempts - 1);
        }
    }
    private float slideBarStartingWidth;

    [Header("Physics Settings")]
    public float gravity = 9.81f;

    [Header("References")]
    public Transform throwMeasurementStart;
    public Transform releasePoint;
    [SerializeField]
    private SlideBar slideBar;

    // Start is called before the first frame update
    void Start()
    {
        slideBarStartingWidth = slideBar.GetWidth();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
