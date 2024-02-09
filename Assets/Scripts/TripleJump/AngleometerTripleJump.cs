using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleometerTripleJump : MonoBehaviour
{
    public GameObject target;
    private TripleJumpPlayerController targetScript;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        targetScript = target.GetComponent<TripleJumpPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = (targetScript.GetAngle().ToString("n2") + "°");
    }
}
