using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("The object the camera will follow.")]
    public GameObject target;
    [Tooltip("Whether the camera will follow the target or not.")]
    public bool followTarget = true;
    [Tooltip("Whether the camera will follow the target on the X-axis.")]
    public bool followX = true;
    [Tooltip("Whether the camera will follow the target on the Y-axis.")]
    public bool followY = true;
    [Tooltip("Whether the camera will smoothly follow the target or whether it will immediately jump to the next camera position.")]
    public bool smoothFollow = true;
    [Tooltip("The distance the camera can move over a second")]
    public float speed = 5f;
    [Tooltip("How close the camera needs to be to its next location before it will snap to that position.")]
    public float snapRange = 0.2f;

    [Header("Camera Settings")]
    [Tooltip("The maximum X-coord the camera can move to.")]
    public float maxX = float.PositiveInfinity;
    [Tooltip("The minimum X-coord the camera can move to.")]
    public float minX = float.NegativeInfinity;
    [Tooltip("The maximum Y-coord the camera can move to.")]
    public float maxY = float.PositiveInfinity;
    [Tooltip("The minimum Y-coord the camera can move to.")]
    public float minY = float.NegativeInfinity;
    public bool useSoftMinY = false;
    public float softMinYStart = 0f;
    public float softMinYEnd = 0f;
    [Tooltip("The offset the camera will have from the target's position.")]
    public Vector2 cameraOffset = new Vector2(0f, 0f);

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCamera();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (followTarget)
        {
            float x = target.transform.position.x + cameraOffset.x; ;
            if (followX)
            {
                x = Mathf.Clamp(x, minX, maxX);
            }
            else
            {
                x = transform.position.x;
            }
            
            float y = target.transform.position.y + cameraOffset.y;
            if (followY)
            {
                if (useSoftMinY)
                {
                    //y = Mathf.Clamp(y, minY - cam.orthographicSize, maxY);
                    y = Mathf.Clamp(y, softMinYEnd, maxY);

                    if (y < softMinYStart)
                    {
                        //y = minY + (1f - Mathf.Pow((softMinYStart - y) / (softMinYStart - minY), 2f)) * (softMinYStart - minY);

                        //y += Mathf.Pow((softMinYStart - y) / (softMinYStart - (minY - cam.orthographicSize)), 1f) * cam.orthographicSize;
                        y += Mathf.Pow((softMinYStart - y) / (softMinYStart - softMinYEnd), 1f) * (minY - softMinYEnd);
                    }
                }
                else
                {
                    y = Mathf.Clamp(y, minY, maxY);
                }
            }
            else
            {
                y = transform.position.y;
            }

            if (smoothFollow)
            {
                if (Vector2.Distance(new Vector2(x, y), new Vector2(transform.position.x, transform.position.y)) < snapRange)
                {
                    transform.position = new Vector3(x, y, transform.position.z);
                }
                else
                {
                    transform.position += (new Vector3(x, y, transform.position.z) - transform.position).normalized * speed * Time.deltaTime;
                }
            }
            else
            {
                transform.position = new Vector3(x, y, transform.position.z);
            }
        }
    }
}
