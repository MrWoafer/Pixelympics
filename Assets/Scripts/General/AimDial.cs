using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialType
{
    quarter,
    half
}

public class AimDial : MonoBehaviour
{
    [Header("Settings")]
    public float angleSpeed = 90f;
    public float startingAngle = 0f;
    public float maxAngle = 90f;
    private bool belowMaxLastFrame = false;
    public DialType dialType = DialType.quarter;
    [SerializeField]
    private bool IsVisible = true;
    public bool isVisible
    {
        get
        {
            return IsVisible;
        }
        set
        {
            SetVisible(value);
        }
    }

    [Header("Info")]
    [SerializeField]
    [Tooltip("The current angle of the dial. Read-only.")]
    private float Angle; /// Not to be actually used. This is just a variable that is updated every frame to the value of 'angle', so that you can see the angle in the inspector.
    public float angle
    {
        get
        {
            return GetAngle();
        }
        set
        {
            SetAngle(value);
        }
    }

    [Header("References")]
    [SerializeField]
    private GameObject needle;
    [SerializeField]
    private GameObject axisY;
    [SerializeField]
    private GameObject axisX;
    [SerializeField]
    private GameObject axisX2;

    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        angle = startingAngle;
        SetVisible(isVisible);
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
        {
            angle += angleSpeed * Time.deltaTime;
        }
        if (angle > maxAngle && belowMaxLastFrame)
        {
            SetAngle(maxAngle);
        }
        if (angle < maxAngle)
        {
            belowMaxLastFrame = true;
        }

        Angle = angle;
    }

    public void SetAngle(float _angle)
    {
        needle.transform.localEulerAngles = new Vector3(0f, 0f, -90f + _angle);
    }
    public float GetAngle()
    {
        return Functions.Mod(90f + needle.transform.localEulerAngles.z, 360f);
    }

    public void StartRotation()
    {
        started = true;
    }

    public float StopRotation()
    {
        started = false;

        return angle;
    }

    public void ResetRotation()
    {
        angle = startingAngle;
    }

    public void SetDialType(DialType _dialType)
    {
        dialType = _dialType;
        if (dialType == DialType.quarter)
        {
            axisX2.SetActive(false);
        }
        else if (dialType == DialType.half && isVisible)
        {
            axisX2.SetActive(true);
        }
    }

    public void SetVisible(bool visible)
    {
        IsVisible = visible;
        
        axisY.SetActive(isVisible);
        axisX.SetActive(isVisible);
        needle.SetActive(isVisible);

        axisX2.SetActive(isVisible);
        SetDialType(dialType);
    }
}
