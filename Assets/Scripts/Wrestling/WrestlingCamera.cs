using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrestlingCamera : MonoBehaviour
{
    [Header("Settings")]
    public bool followPlayers = true;
    public float speed = 10f;
    public float snapRange = 0.2f;

    [Header("References")]
    public Wrestler p1;
    public Wrestler p2;
    public WrestlingBall ball;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (followPlayers)
        {
            if (p1.inBall)
            {
                float x = ball.transform.position.x;
                if (Mathf.Abs(transform.position.x - x) < snapRange)
                {
                    transform.position += new Vector3(x - transform.position.x, 0f, 0f);
                }
                else
                {
                    transform.position += new Vector3(Time.deltaTime * speed * Mathf.Sign(x - transform.position.x), 0f, 0f);
                }
            }
            else
            {
                float x = (p1.transform.position.x + p2.transform.position.x) / 2f;
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
}
