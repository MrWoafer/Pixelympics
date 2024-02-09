using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinJavelinController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    private JavelinConfig config;
    private OlympicsController olympicsController;

    public float angle;

    private bool released = false;

    private float t = 0f;

    private float releaseAngle;
    private Vector3 releasePoint;

    private float uUp;
    private float uRight;
    private float maxHeight;
    private float distance;
    private float k;

    private bool displayedDistance = false;

    public bool fouled = false;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<JavelinConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!released)
        {
            angle = transform.eulerAngles.z;
        }
        else
        {
            t += Time.deltaTime;
            angle = releaseAngle;
        }

        //if (transform.position.y > -2.5f && released)
        if (transform.position.y + 1f * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad) > -3.2f && released)
        {
            //transform.position = new Vector3(releasePoint.x + t, releasePoint.y + -k * t * (t - distance), 0f);
            transform.position = new Vector3(releasePoint.x + uRight*t, releasePoint.y + uUp*t - config.gravity * t * t / 2, 0f);

            //transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan(-2 * k * t + k * distance));
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan((uUp - config.gravity * t) / uRight));
        }
        else if (transform.position.y + 1f * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad) <= -3.2f && released && !displayedDistance)
        {
            displayedDistance = true;

            var distanceThrown = Functions.RoundToRange(transform.position.x + 1f * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad) - config.lineX, 0f, 10000f);
            Debug.Log("Distance thrown: " + distanceThrown.ToString("n2") + " m");


            if (!fouled)
            {
                BroadcastScore(distanceThrown);
            }

            string playerName = player.GetComponent<JavelinPlayerController>().playerName;
            bool eligibleForRecord = player.GetComponent<JavelinPlayerController>().eligibleForRecord;

            if (distanceThrown > PlayerPrefs.GetFloat("Javelin PB " + playerName, 0f) && eligibleForRecord && !fouled)
            {
                Debug.Log(playerName + " got a new PB!");
                PlayerPrefs.SetFloat("Javelin PB " + playerName, distanceThrown);
            }

            if (distanceThrown > PlayerPrefs.GetFloat("Javelin Record", 90f) && eligibleForRecord && !fouled)
            {
                PlayerPrefs.SetFloat("Javelin Record", distanceThrown);
                Debug.Log("New Record!");
            }
            else if (distanceThrown < PlayerPrefs.GetFloat("Javelin Worst Record", 5f) && distanceThrown > 0f && eligibleForRecord && !fouled)
            {
                PlayerPrefs.SetFloat("Javelin Worst Record", distanceThrown);
                Debug.Log("New Worst Record!");
            }
        }
    }

    public void FixedUpdate()
    {
        /*if (released)
        {
            t += Time.fixedDeltaTime;
        }*/
    }

    public void Throw(float angle, float speed, Vector3 releasedFrom)
    {
        //Debug.Log(angle.ToString() + ", " + speed.ToString() + ", " + releasedFrom.ToString());
        Debug.Log(angle.ToString() + "°, " + speed.ToString() + " (" + (speed / config.maxSpeed * config.maxMPS).ToString("n2") + "m/s), " + releasedFrom.ToString());

        released = true;

        releaseAngle = angle;

        releasePoint = releasedFrom;

        uUp = speed * config.releaseSpeedMultiplier * Mathf.Sin(angle * Mathf.Deg2Rad);
        uRight = speed * config.releaseSpeedMultiplier * Mathf.Cos(angle * Mathf.Deg2Rad);

        maxHeight = uUp * uUp / (2 * config.gravity);
        distance = 2 * uUp * uRight / config.gravity;
        //k = 4 * maxHeight / (distance * distance);

        //Debug.Log("uUp: " + uUp.ToString());
        //Debug.Log("uRight: " + uRight.ToString());
        //Debug.Log("maxHeight: " + maxHeight.ToString());
        //Debug.Log("distance: " + distance.ToString());
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score);
        }
    }
}
