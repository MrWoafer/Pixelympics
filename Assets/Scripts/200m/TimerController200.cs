using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController200 : MonoBehaviour
{
    public GameObject target;
    private Sprint200RaceController targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        targetScript = target.GetComponent<Sprint200RaceController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = targetScript.GetTime().ToString("n2");
    }
}
