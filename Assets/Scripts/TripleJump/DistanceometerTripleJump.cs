using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DistanceometerTripleJump : MonoBehaviour
{
    public GameObject target;
    public GameObject configObj;
    private TripleJumpConfig config;

    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();

        config = configObj.GetComponent<TripleJumpConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Functions.RoundToRange(target.transform.position.x - config.lineX, 0f, 10000f).ToString("n2") + " m";
    }
}
