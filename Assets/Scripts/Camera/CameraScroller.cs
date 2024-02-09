using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [Header("Settings")]
    public bool canScroll = true;
    [Space(15)]
    public bool horizontalScrolling = true;
    public float horizontalSpeed = 5f;
    [Space(5)]
    public bool useHorizontalMax = false;
    public float horizontalMax = 5f;
    public bool useHorizontalMin = false;
    public float horizontalMin = -5f;
    [Space(15)]
    public bool verticalScrolling = true;
    public float verticalSpeed = 5f;
    [Space(5)]
    public bool useVerticalMax = false;
    public float verticalMax = 5f;
    public bool useVerticalMin = false;
    public float verticalMin = -5f;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canScroll)
        {
            if (horizontalScrolling)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    cam.transform.position += new Vector3(-horizontalSpeed * Time.deltaTime, 0f, 0f);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    cam.transform.position += new Vector3(horizontalSpeed * Time.deltaTime, 0f, 0f);
                }
            }
            if (verticalScrolling)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    cam.transform.position += new Vector3(0f, verticalSpeed * Time.deltaTime, 0f);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    cam.transform.position += new Vector3(0f, -verticalSpeed * Time.deltaTime, 0f);
                }
            }
        }

        if (useHorizontalMax && cam.transform.position.x > horizontalMax)
        {
            cam.transform.position = new Vector3(horizontalMax, cam.transform.position.y, cam.transform.position.z);
        }
        if (useHorizontalMin && cam.transform.position.x < horizontalMin)
        {
            cam.transform.position = new Vector3(horizontalMin, cam.transform.position.y, cam.transform.position.z);
        }

        if (useVerticalMax && cam.transform.position.y > verticalMax)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, verticalMax, cam.transform.position.z);
        }
        if (useVerticalMin && cam.transform.position.y < verticalMin)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, verticalMin, cam.transform.position.z);
        }
    }
}
