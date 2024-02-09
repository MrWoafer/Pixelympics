using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinCameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject javelin;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (Functions.RoundToRange(javelin.transform.position.x, 0, 1000f), Functions.RoundToRange(javelin.transform.position.y, 0f, 1000f), -10f);
    }
}
