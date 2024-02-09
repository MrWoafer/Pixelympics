using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBallShooter : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("-1 for infinite")]
    public int ballsLeft = -1;
    public float maxWaitTime = 2.3f;
    public float minWaitTime = 1.7f;
    public float maxSpeed = 15f;
    public float minSpeed = 13f;
    public float maxAngleH = 30f;
    public float minAngleH = 0f;
    public float maxAngleV = 20f;
    public float minAngleV = 15f;

    [Header("References")]
    public GameObject tennisBall;

    private float t;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;

        if (t <= 0f && (ballsLeft > 0 || ballsLeft == -1))
        {
            //Debug.Log("Pew!");

            TennisBall ball = Instantiate(tennisBall, transform.position, Quaternion.identity).GetComponent<TennisBall>();

            float angleH = Random.Range(minAngleH, maxAngleH) * (Functions.RandomBool() ? 1f : -1f);
            float angleV = Random.Range(minAngleV, maxAngleV);
            float speed = Random.Range(minSpeed, maxSpeed);

            //Debug.Log("AngleH: " + angleH);

            ball.Hit(speed, angleH + 180f, angleV);

            t = Random.Range(minWaitTime, maxWaitTime);

            if (ballsLeft > 0)
            {
                ballsLeft--;
            }
        }
    }
}
