using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedClimbingCamera : MonoBehaviour
{
    [Header("Settings")]
    public bool followPlayer = true;
    public bool smoothFollow = true;
    public float speed = 5f;
    public float snapRange = 0.2f;
    public float minY = 4f;
    public float offsetY = 2f;

    [Header("References")]
    public SpeedClimber target;

    private void Awake()
    {
        UpdateCamera();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        if (followPlayer)
        {
            //float y = target.transform.position.y;
            float y = target.transform.position.y + offsetY;
            y = Mathf.Max(minY, y);

            if (smoothFollow)
            {
                if (Mathf.Abs(transform.position.y - y) < snapRange)
                {
                    transform.position += new Vector3(0f, y - transform.position.y, 0f);
                }
                else
                {
                    transform.position += new Vector3(0f, Time.deltaTime * speed * Mathf.Sign(y - transform.position.y), 0f);
                }
            }
            else
            {
                transform.position += new Vector3(0f, y - transform.position.y, 0f);
            }
        }
    }
}
