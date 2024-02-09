using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongJumpCameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = new Vector3(Functions.RoundToRange(player.transform.position.x, 0, 1000f), Functions.RoundToRange(player.transform.position.y, 0f, 1000f), -10f);
        //transform.position = new Vector3(Functions.RoundToRange(player.transform.position.x, -10f, 1000f), Functions.RoundToRange(player.transform.position.y, -2f, 1000f), -10f);
        //transform.position = new Vector3(Functions.RoundToRange(player.transform.position.x, -10f, 1000f), Functions.RoundToRange(player.transform.position.y, -1f, 1000f), -10f);
        transform.position = new Vector3(Functions.RoundToRange(player.transform.position.x, -5f, 1000f), Functions.RoundToRange(player.transform.position.y, -1f, 1000f), -10f);
    }
}
