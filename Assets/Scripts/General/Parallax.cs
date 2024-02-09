using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private ParallaxCamera parallaxCam;
    private Vector3 originalPos;

    // Start is called before the first frame update
    void Awake()
    {
        originalPos = transform.position;
        parallaxCam = Camera.main.GetComponent<ParallaxCamera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float x;
        float y;

        if (parallaxCam.parallaxX)
        {
            x = parallaxCam.transform.position.x + (originalPos.x - parallaxCam.originalPos.x) - (parallaxCam.transform.position.x - parallaxCam.originalPos.x) * parallaxCam.parallaxAmountX / (transform.position.z - parallaxCam.transform.position.z);
        }
        else
        {
            x = originalPos.x;
        }

        if (parallaxCam.parallaxY)
        {
            y = parallaxCam.transform.position.y + (originalPos.y - parallaxCam.originalPos.y) - (parallaxCam.transform.position.y - parallaxCam.originalPos.y) * parallaxCam.parallaxAmountY / (transform.position.z - parallaxCam.transform.position.z);
        }
        else
        {
            y = originalPos.y;
        }

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
