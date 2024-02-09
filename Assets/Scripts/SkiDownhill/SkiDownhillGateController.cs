using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiDownhillGateController : MonoBehaviour
{
    [Header("Settings")]
    public float width = 6f;
    public float lineThickness = 0.05f;
    public Color lineColour = Color.red;

    [Header("References")]
    [SerializeField]
    private GameObject flagLeft;
    [SerializeField]
    private GameObject flagRight;
    [SerializeField]
    private Transform flagLeftHitPoint;
    [SerializeField]
    private Transform flagRighttHitPoint;
    [SerializeField]
    private GameObject gateLine;

    // Start is called before the first frame update
    void Start()
    {
        UpdateAppearance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        flagLeft.transform.localPosition = new Vector3(-width / 2f, 0.5f, 0f);
        flagLeft.transform.position = new Vector3(flagLeft.transform.position.x, flagLeft.transform.position.y, flagLeft.transform.position.y / 1000f);

        flagRight.transform.localPosition = new Vector3(width / 2f, 0.5f, 0f);
        flagRight.transform.position = new Vector3(flagRight.transform.position.x, flagRight.transform.position.y, flagRight.transform.position.y / 1000f);

        gateLine.transform.localScale = new Vector3(width, lineThickness, 1f);
        gateLine.transform.localPosition = new Vector3(0f, lineThickness / 2f, 0f);
        gateLine.transform.position = new Vector3(gateLine.transform.position.x, gateLine.transform.position.y, gateLine.transform.position.y / 1000f);
        gateLine.GetComponent<SpriteRenderer>().color = lineColour;
    }
}
