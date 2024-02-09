using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RowingCamera : MonoBehaviour
{
    [Header("Settings")]
    public float padding = 0f;

    [Header("References")]
    public GameObject[] boats;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float maxX = Enumerable.Max(from boat in boats select boat.transform.position.x);
        float minX = Enumerable.Min(from boat in boats select boat.transform.position.x);

        float midX = (maxX + minX) / 2f;
        midX = Mathf.Max(midX, 0f);

        cam.orthographicSize = Mathf.Max(5f, (maxX - minX + padding) * Screen.height / Screen.width / 2f);

        transform.position = new Vector3(midX, transform.position.y, transform.position.z);
    }
}
