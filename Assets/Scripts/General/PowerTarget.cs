using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTarget : MonoBehaviour
{
    [Header("Power Settings")]
    [Tooltip("How much power is gained with each press.")]
    public float powerGain = 0.05f;
    [Tooltip("How much power is lost over a second.")]
    public float powerDecay = 1f;
    [Tooltip("Set to 0 for no max power.")]
    public float maxPower = 1f;

    [Header("Zone Settings")]
    public float zoneUpperPower = 0.8f;
    public float zonePerfectPower = 0.7f;
    public float zoneLowerPower = 0.6f;

    [Header("Visual Settings")]
    [Min(0f)]
    public float boxWidth = 0.5f;
    [Min(0f)]
    public float boxHeight = 2f;
    [Min(0f)]
    public float borderSize = 0.05f;
    public bool fillPowerBar = true;
    public bool smoothMovement = true;
    [Tooltip("How much power the power bar can change by over a second.")]
    public float smoothMovementSpeed = 1f;
    [Tooltip("How close the power bar has to be to its current value before it snaps to the current value.")]
    public float smoothMovementSnapDistance = 0.05f;

    [Header("Info")]
    [SerializeField]
    [Tooltip("The current power. Read-only.")]
    private float Power;

    [Header("References")]
    [SerializeField]
    private GameObject background;
    [SerializeField]
    private GameObject border;
    [SerializeField]
    private GameObject powerBar;
    [SerializeField]
    private GameObject zoneUpper;
    [SerializeField]
    private GameObject zonePerfect;
    [SerializeField]
    private GameObject zoneLower;
    [SerializeField]
    private GameObject powerBarFill;

    //[SerializeField]
    private float p = 0f;
    public float power
    {
        set
        {
            SetPower(value);
        }
        get
        {
            return GetPower();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        p -= powerDecay * Time.deltaTime;

        if (p <= 0f)
        {
            p = 0;
        }
        if (p > maxPower && p > 0f)
        {
            p = maxPower;
        }

        UpdateDisplay();
        Power = p;
    }

    private void OnValidate()
    {
        UpdateBox(true);
    }

    private void UpdateBox()
    {
        UpdateBox(false);
    }
    private void UpdateBox(bool overrideSmoothMovement)
    {
        background.transform.localScale = new Vector3(boxWidth, boxHeight, 1f);
        border.transform.localScale = background.transform.localScale + new Vector3(borderSize * 2f, borderSize * 2f, 0f);

        powerBar.transform.localScale = new Vector3(boxWidth, powerBar.transform.localScale.y, 1f);

        zoneUpper.transform.localScale = new Vector3(boxWidth, zoneUpper.transform.localScale.y, 1f);
        zonePerfect.transform.localScale = new Vector3(boxWidth, zonePerfect.transform.localScale.y, 1f);
        zoneLower.transform.localScale = new Vector3(boxWidth, zoneLower.transform.localScale.y, 1f);

        powerBarFill.transform.localScale = new Vector3(boxWidth, powerBar.transform.localScale.y, 1f);

        UpdateDisplay(overrideSmoothMovement);
    }

    public void PowerGain()
    {
        //p += powerGain;
        p = power + powerGain;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        UpdateDisplay(false);
    }
    private void UpdateDisplay(bool overrideSmoothMovement)
    {
        if (smoothMovement && !overrideSmoothMovement)
        {
            if (Mathf.Abs(power - p) < smoothMovementSnapDistance)
            {
                powerBar.transform.localPosition = new Vector3(0f, (p / maxPower - 0.5f) * boxHeight, 0f);
            }
            else if (power < p)
            {
                powerBar.transform.localPosition += new Vector3(0f, smoothMovementSpeed * Time.deltaTime, 0f);
            }
            else
            {
                powerBar.transform.localPosition -= new Vector3(0f, smoothMovementSpeed * Time.deltaTime, 0f);
            }
        }
        else
        {
            powerBar.transform.localPosition = new Vector3(0f, (p / maxPower - 0.5f) * boxHeight, 0f);
        }

        zoneUpper.transform.localPosition = new Vector3(0f, (zoneUpperPower / maxPower - 0.5f) * boxHeight, 0f);
        zonePerfect.transform.localPosition = new Vector3(0f, (zonePerfectPower / maxPower - 0.5f) * boxHeight, 0f);
        zoneLower.transform.localPosition = new Vector3(0f, (zoneLowerPower / maxPower - 0.5f) * boxHeight, 0f);

        powerBarFill.transform.localPosition = new Vector3(0f, (powerBar.transform.localPosition.y - boxHeight / 2f) / 2f, 0f);
        powerBarFill.transform.localScale = new Vector3(powerBar.transform.localScale.x, (powerBar.transform.localPosition.y + boxHeight / 2f), 1f);
    }

    public float GetPower()
    {
        //return p;
        return (powerBar.transform.localPosition.y / boxHeight + 0.5f) * maxPower;
    }
    public void SetPower(float value, bool overrideSmoothMovement = true)
    {
        p = value;
        UpdateDisplay(overrideSmoothMovement);
    }

    public float GetPowerPercentage()
    {
        return GetPower() / maxPower;
    }
    public void SetPowerPercentage(float percentage, bool overrideSmoothMovement = true)
    {
        SetPower(maxPower * percentage, overrideSmoothMovement);
    }

    public bool InZone(bool inclusive = false)
    {
        if (inclusive)
        {
            return power <= zoneUpperPower && power >= zoneLowerPower;
        }
        else
        {
            return power < zoneUpperPower && power > zoneLowerPower;
        }
    }
}
