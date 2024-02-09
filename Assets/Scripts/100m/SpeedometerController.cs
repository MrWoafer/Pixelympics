using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SpeedometerController : MonoBehaviour
{
    public GameObject target;
    private Sprint100Controller targetScript;
    private Sprint100Config config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Sprint100Config>();
        targetScript = target.GetComponent<Sprint100Controller>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = targetScript.GetSpeed().ToString();
        //text.text = (targetScript.GetSpeed() / Sprint100Config.maxSpeed * 100 / ((Sprint100Config.finishX- Sprint100Config.startX)/60/ Sprint100Config.maxSpeed)).ToString("n2") + " m/s";
        //text.text = (targetScript.GetSpeed() / Sprint100Config.maxSpeed * 100 / Sprint100Config.minTime).ToString("n2") + " m/s";
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
