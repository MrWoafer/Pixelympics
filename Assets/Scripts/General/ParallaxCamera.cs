using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamera : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float parallaxAmountX = 1f;
    public float parallaxAmountY = 1f;
    public bool parallaxX = true;
    public bool parallaxY = false;
    
    private Vector3 _originalPos;
    public Vector3 originalPos
    {
        get
        {
            return _originalPos;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        ResetParallax();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetParallax()
    {
        _originalPos = transform.position;
    }
}
