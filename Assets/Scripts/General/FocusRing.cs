using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ThicknessMode
{
    constantThickness,
    proportionalThickness
}

public class FocusRing : MonoBehaviour
{
    [Header("Settings")]
    [Min(0)]
    public float thickness = 0.4f;
    public ThicknessMode thicknessMode;
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
    [Tooltip("The current radius of the ring. Read-only.")]
    private float Radius;
    public float radius
    {
        set
        {
            SetRadius(value);
        }
        get
        {
            return GetRadius();
        }
    }

    [Header("References")]
    public SpriteRenderer sprCircle;
    public SpriteMask maskCircle;

    private float shrinkDuration;
    private float t;
    private bool started = false;
    public float timeLeft
    {
        get
        {
            return t;
        }
    }
    public float timeElapsed
    {
        get
        {
            return shrinkDuration - t;
        }
    }
    private float startingRadius;
    private float endingRadius;

    private bool saidFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        SetVisible(isVisible);
    }

    // Update is called once per frame
    void Update()
    {
        Radius = radius;

        if (started)
        {
            t -= Time.deltaTime;
            SetRadius(Mathf.Lerp(startingRadius, endingRadius, timeElapsed / shrinkDuration));
        }
        /*if (HasFinished() && !saidFinished)
        {
            saidFinished = true;
            Debug.Log("Finished");
        }*/
    }

    public void SetRadius(float _radius)
    {
        if (_radius < 0f)
        {
            _radius = 0f;
        }

        sprCircle.transform.localScale = new Vector3(1f, 1f, 1f) * _radius * 2f;

        if (thicknessMode == ThicknessMode.constantThickness)
        {
            maskCircle.transform.localScale = new Vector3(1f, 1f, 1f) * Mathf.Max(0f, radius * 2f - thickness * 2f);
        }
        else if (thicknessMode == ThicknessMode.proportionalThickness)
        {
            maskCircle.transform.localScale = new Vector3(1f, 1f, 1f) * radius * 2f * thickness;
        }
    }

    public float GetRadius()
    {
        return sprCircle.transform.localScale.x / 2f;
    }

    public void StartShrink(float startRadius, float duration)
    {
        StartShrink(startRadius, 0f, duration);
    }
    public void StartShrink(float startRadius, float endRadius, float duration)
    {
        SetRadius(startRadius);
        startingRadius = startRadius;
        endingRadius = endRadius;
        shrinkDuration = duration;
        t = duration;
        started = true;

        saidFinished = false;
    }
    public void StopShrink()
    {
        started = false;
    }

    public void SetVisible(bool visible)
    {
        IsVisible = visible;

        sprCircle.gameObject.SetActive(isVisible);
        maskCircle.gameObject.SetActive(isVisible);
    }

    public bool HasFinished()
    {
        return radius <= endingRadius;
    }
}
