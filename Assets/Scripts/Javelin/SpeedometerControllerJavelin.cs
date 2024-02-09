using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerControllerJavelin : MonoBehaviour
{
    public GameObject target;
    private JavelinPlayerController targetScript;

    private Text text;

    private JavelinConfig config;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<JavelinConfig>();
        targetScript = target.GetComponent<JavelinPlayerController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / config.maxSpeed * config.maxMPS).ToString("n2") + " m/s";
    }
}
