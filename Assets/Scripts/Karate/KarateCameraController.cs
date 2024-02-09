using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarateCameraController : MonoBehaviour
{
    [Header("Settings")]
    public bool followPlayers = false;
    public float speed = 1f;
    public float snapRange = 0.4f;

    [Header("References")]
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (followPlayers)
        {
            float x = (player1.transform.position.x + player2.transform.position.x) / 2f;
            if (Mathf.Abs(transform.position.x - x) < snapRange)
            {
                transform.position += new Vector3(x - transform.position.x, 0f, 0f);
            }
            else
            {
                transform.position += new Vector3(Time.deltaTime * speed * Mathf.Sign(x - transform.position.x), 0f, 0f);
            }
        }
    }
}
