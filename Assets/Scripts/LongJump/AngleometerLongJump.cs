using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleometerLongJump : MonoBehaviour
{
    public GameObject target;
    private LongJumpController targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        targetScript = target.GetComponent<LongJumpController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetAngle().ToString("n2") + "°");
    }
}
