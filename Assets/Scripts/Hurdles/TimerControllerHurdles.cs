using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerControllerHurdles : MonoBehaviour
{
    public GameObject target;
    private HurdlesRaceController targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = target.GetComponent<HurdlesRaceController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = targetScript.GetTime().ToString("n2");
    }
}
