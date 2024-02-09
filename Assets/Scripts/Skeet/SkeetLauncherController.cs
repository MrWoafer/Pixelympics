using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeetLauncherController : MonoBehaviour
{
    [Header("Settings")]
    public float angle = 45f;
    public float speed = 10f;
    public float stationNum = 1;
    public float countdown = 2f;
    public bool launched = false;

    [Header("References")]
    public GameObject skeetPrefab;
    public GameObject configObj;
    private SkeetConfig config;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<SkeetConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        if (config.pulled && stationNum == config.stationNum && !launched)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f)
            {
                Launch();
            }
        }
    }

    public void Launch()
    {
        launched = true;
        GameObject skeet = Instantiate(skeetPrefab, transform.position, transform.rotation);
        skeet.GetComponent<Rigidbody>().velocity = skeet.transform.forward * speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        skeet.GetComponent<Rigidbody>().velocity += new Vector3(0f, speed * Mathf.Sin(angle * Mathf.Deg2Rad), 0f);
    }
}
