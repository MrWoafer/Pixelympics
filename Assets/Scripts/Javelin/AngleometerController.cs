using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleometerController : MonoBehaviour
{
    public GameObject target;
    private JavelinJavelinController targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        targetScript = target.GetComponent<JavelinJavelinController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.angle.ToString("n2") + "°");
    }
}
