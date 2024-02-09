using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerController200 : MonoBehaviour
{
    public GameObject target;
    private Sprint200Controller targetScript;

    private Text text;

    private Sprint200Config config;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<Sprint200Config>();
        targetScript = target.GetComponent<Sprint200Controller>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
