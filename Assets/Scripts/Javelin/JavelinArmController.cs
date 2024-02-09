using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinArmController : MonoBehaviour
{
    [Header("Settings)")]
    public float armRadius = 0.35f;
    public float bobAmount = 0.1f;

    [Header("References")]
    public GameObject player;
    public JavelinPlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<JavelinPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            -armRadius * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad) + player.transform.position.x,
            //armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2.19f + 2.5f - armRadius + player.transform.position.y + ((float)playerController.bob) * bobAmount,
            //armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2.19f + 2.5f - armRadius + player.transform.position.y,
            //armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2f + 2.5f - armRadius + player.transform.position.y,
            //armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2.065f + 2.5f - armRadius + player.transform.position.y,
            armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2.03f + 2.5f - armRadius + player.transform.position.y,
            //armRadius * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - 2.03f - player.transform.position.y - armRadius + player.transform.position.y,
            transform.position.z);
    }
}
