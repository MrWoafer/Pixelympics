
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerController400 : MonoBehaviour
{
    public GameObject target;
    private Run400Controller targetScript;
    private Run400Config config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Run400Config>();
        targetScript = target.GetComponent<Run400Controller>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
