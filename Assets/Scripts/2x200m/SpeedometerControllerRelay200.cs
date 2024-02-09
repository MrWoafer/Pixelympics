using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerControllerRelay200 : MonoBehaviour
{
    public GameObject target;
    private Relay200Controller targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = target.GetComponent<Relay200Controller>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetSpeed() / Relay200Config.maxSpeed * Relay200Config.maxMPS).ToString("n2") + " m/s";
    }
}
