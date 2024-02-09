using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerControllerHurdles : MonoBehaviour
{
    public GameObject target;
    private HurdlesController targetScript;

    private Text text;

    private HurdlesConfig config;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<HurdlesConfig>();
        targetScript = target.GetComponent<HurdlesController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
