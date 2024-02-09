using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighJumpCameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObj;
    private HighJumpPlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<HighJumpPlayerController>();

        //transform.eulerAngles = new Vector3(4f, 210f, 0f);
        //transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.HasStarted())
        {
            transform.position = new Vector3(35f, 2f, 1f);
            transform.eulerAngles = new Vector3(-4f, 233f, 0f);
        }
        if (player.IsRunning() || player.IsReady())
        {
            //transform.position = playerObj.transform.position + new Vector3(-1f, 1.5f, 8f);
            transform.position = playerObj.transform.position + new Vector3(0f, 1.5f, 6f);
            transform.eulerAngles = new Vector3(10f, 180f, 0f);
        }
    }
}
