using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceometerControllerLongJump : MonoBehaviour
{
    public GameObject target;
    private LongJumpConfig config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<LongJumpConfig>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = Functions.RoundToRange(target.transform.position.x - JavelinConfig.lineX, 0f, 10000f).ToString("n2") + " m";
        //text.text = Functions.RoundToRange(target.transform.position.x + 1f * Mathf.Cos(target.transform.eulerAngles.z * Mathf.Deg2Rad) - LongJumpConfig.lineX, 0f, 10000f).ToString("n2") + " m";
        text.text = Functions.RoundToRange(target.transform.position.x - config.lineX, 0f, 10000f).ToString("n2") + " m";
    }
}
