using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryConfig : MonoBehaviour
{
    [Header("Wind Settings")]
    public float windAngle;
    public float windSpeed = 1f;
    public float minWindSpeed = 0.5f;
    public float maxWindSpeed = 2f;
    public float windRotationScalar = 10f;
    public float minWindChangeTime = 4f;
    public float maxWindChangeTime = 8f;
    private float windChangeTime;

    private float weatherVaneOffset = 0f;
    public float weatherVaneOffsetAdd = 0.4f;
    public float weatherVaneMaxOffset = 5f;

    private float oldWindAngle;
    private float newWindAngle;
    private float oldWindSpeed;
    private float newWindSpeed;

    private bool lerping = false;
    private float lerpTime = 0f;
    private float originalLerpTime = 0f;

    [Header("Player Settings")]
    public float keyboardHorizontalRotationSpeed = 30f;
    public float keyboardVerticalRotationSpeed = 30f;
    public float mouseHorizontalRotationSpeed = 30f;
    public float mouseVerticalRotationSpeed = 30f;
    public float zoomSpeed = 30f;
    public float minZoom = 10f;
    public float screenShake = 1f;
    public float countdownTime = 5f;

    [Header("Arrow Settings")]
    public float arrowSpeed = 5f;

    [Header("References")]
    public GameObject weatherVane;

    public void Start()
    {
        ChangeWind();
    }

    public void Update()
    {
        if (!lerping)
        {
            windChangeTime -= Time.deltaTime;
        }
        else
        {
            lerpTime -= Time.deltaTime;
            windAngle = oldWindAngle * lerpTime / originalLerpTime + (1f - lerpTime / originalLerpTime) * newWindAngle;
            windSpeed = oldWindSpeed * lerpTime / originalLerpTime + (1f - lerpTime / originalLerpTime) * newWindSpeed;

            if (lerpTime <= 0f)
            {
                windSpeed = newWindSpeed;
                windAngle = newWindAngle;
                lerping = false;
            }
        }
        if (windChangeTime <= 0f)
        {
            //LerpChangeWind(2f);
        }

        weatherVaneOffset += Random.Range(-weatherVaneOffsetAdd, weatherVaneOffsetAdd);
        weatherVaneOffset = Mathf.Clamp(weatherVaneOffset, -weatherVaneMaxOffset, weatherVaneMaxOffset);
        weatherVane.transform.eulerAngles = new Vector3(0f, 270f - windAngle + weatherVaneOffset * windSpeed / maxWindSpeed, 0f);
    }

    public void ChangeWind()
    {
        windAngle = Random.Range(0f, 360f);
        windSpeed = Random.Range(minWindSpeed, maxWindSpeed);

        windChangeTime = Random.Range(minWindChangeTime, maxWindChangeTime);
    }
    public void LerpChangeWind(float lerpDuration)
    {
        oldWindAngle = windAngle;
        oldWindSpeed = windSpeed;

        newWindAngle = Random.Range(0f, 360f);
        newWindSpeed = Random.Range(minWindSpeed, maxWindSpeed);

        originalLerpTime = lerpDuration;
        lerpTime = lerpDuration;
        lerping = true;

        windChangeTime = Random.Range(minWindChangeTime, maxWindChangeTime);
    }

    public Vector3 GetWindDirection()
    {
        return new Vector3(Mathf.Cos(Mathf.Deg2Rad * windAngle), 0f, Mathf.Sin(Mathf.Deg2Rad * windAngle));
    }
}
