using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerTripleJump : MonoBehaviour
{
    public GameObject target;
    private TripleJumpPlayerController targetScript;
    public GameObject configObj;
    private TripleJumpConfig config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = target.GetComponent<TripleJumpPlayerController>();
        text = GetComponent<Text>();

        config = configObj.GetComponent<TripleJumpConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = config.speed + " m/s";
        text.text = targetScript.GetSpeed().ToString("n3") + " m/s";
    }
}
