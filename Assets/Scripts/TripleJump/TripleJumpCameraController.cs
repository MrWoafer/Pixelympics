using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleJumpCameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public GameObject configObj;
    private TripleJumpConfig config;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<TripleJumpConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Functions.RoundToRange(player.transform.position.x, config.cameraMinX, config.cameraMaxX), Functions.RoundToRange(player.transform.position.y, config.cameraMinY, config.cameraMaxY), -10f);
    }
}