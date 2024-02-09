using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Header("Appearance")]
    public Color waterTint = Color.white;
    public bool fadeGradient = false;
    public bool foam = true;

    [Header("Graphical Settings")]
    public float resolution = 1f;

    private float resolutionScaler = 100f;
    private int depth = 16;

    [Header("References")]
    public Camera renderCamera;
    private RenderTexture renderTexture;

    private SpriteRenderer sprRen;

    private bool visibleLock = false;

    // Start is called before the first frame update
    void Start()
    {
        sprRen = GetComponent<SpriteRenderer>();

        SetResolution(resolution);

        SetTint(waterTint);

        SetGradient(fadeGradient);

        AlignCamera();

        MakeInvisible();

        SetFoam(foam);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        /// Activate render camera if the water is visible
        if (sprRen.isVisible && !visibleLock)
        {
            visibleLock = true;
            renderCamera.enabled = true;
        }
        /// Deactivate render camera if the water is not visible
        else if (!sprRen.isVisible && visibleLock)
        {
            visibleLock = false;
            renderCamera.enabled = false;
        }
        */
    }

    private void OnBecameVisible()
    {
        MakeVisible();
    }
    private void OnBecameInvisible()
    {
        MakeInvisible();
    }

    private void MakeVisible()
    {
        renderCamera.enabled = true;
        //Debug.Log("Visible.");
    }
    private void MakeInvisible()
    {
        renderCamera.enabled = false;
        //Debug.Log("Not visible.");
    }

    private void SetResolution(float res)
    {
        renderTexture = new RenderTexture((int)(transform.lossyScale.x * res * resolutionScaler), (int)(transform.lossyScale.y * res * resolutionScaler), depth);
        renderTexture.name = "WaterRenderTexture";
        sprRen.material.SetTexture("RenderTexture", renderTexture);
    }

    private void SetTint(Color tint)
    {
        sprRen.material.SetColor("Tint", tint);
    }

    private void AlignCamera()
    {
        renderCamera.targetTexture = renderTexture;
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = transform.lossyScale.y / 2f;

        renderCamera.transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void SetGradient(bool useGradient)
    {
        sprRen.material.SetInt("GradientBool", useGradient ? 1 : 0);
    }

    private void SetFoam(bool useFoam)
    {
        sprRen.material.SetInt("Foam", useFoam ? 1 : 0);
    }
}
