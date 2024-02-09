using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerControllerLongJump : MonoBehaviour
{
    public GameObject target;
    private LongJumpController targetScript;
    private LongJumpConfig config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<LongJumpConfig>();
        targetScript = target.GetComponent<LongJumpController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
