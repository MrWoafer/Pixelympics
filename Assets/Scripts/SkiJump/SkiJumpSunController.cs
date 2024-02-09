using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiJumpSunController : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(cam.orthographicSize * Screen.width / Screen.height, transform.localPosition.y, transform.localPosition.z);
    }
}
