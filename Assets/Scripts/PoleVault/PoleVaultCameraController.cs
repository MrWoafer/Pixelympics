using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleVaultCameraController : MonoBehaviour
{
    [Header("Settings")]
    public float lerpSpeed = 1f;

    [Header("References")]
    public GameObject playerObj;
    private PoleVaultPlayerController player;
    public GameObject bar;

    private float lerp;
    private bool lerping = false;
    private Vector3 lerpStartPoint;
    private Vector3 lerpEndPoint;
    private bool lerpingToPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<PoleVaultPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!lerping)
        {
            if (!player.HasStarted())
            {
                transform.position = new Vector3(34f, 2f, 5f);
                transform.eulerAngles = new Vector3(-3f, 230f, 0f);
            }
            if (player.IsRunning() || player.IsReady() && !player.IsSticking())
            {
                transform.position = playerObj.transform.position + new Vector3(-4f, 1.5f, 6f);

                /*if (transform.position.x < bar.transform.position.x + 0.3f)
                {
                    transform.position -= new Vector3(transform.position.x - bar.transform.position.x - 0.3f, 0f, 0f);
                }*/

                transform.eulerAngles = new Vector3(10f, 180f, 0f);
            }
            if (player.IsSticking())
            {
                transform.position = playerObj.transform.position + new Vector3(0f, 1.5f, 6f);
            }
        }
        else
        {
            if (lerp < 1f)
            {
                lerp += lerpSpeed * Time.deltaTime;
            }
            if (lerpingToPlayer)
            {
                lerpEndPoint = player.transform.position + new Vector3(0f, 1.5f, 6f);
            }
            transform.position = lerp * lerpEndPoint + (1f - lerp) * lerpStartPoint;
        }   
    }

    public void LerpTo(Vector3 position)
    {
        lerpStartPoint = transform.position;
        lerpEndPoint = position;
        lerping = true;
        lerp = 0f;
    }
    public void LerpToPlayer()
    {
        lerpingToPlayer = true;
        lerpStartPoint = transform.position;
        lerping = true;
        lerp = 0f;
    }
    public void EndLerp()
    {
        lerping = false;
        lerpingToPlayer = false;
    }
}
