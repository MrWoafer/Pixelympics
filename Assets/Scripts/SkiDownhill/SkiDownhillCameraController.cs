using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiDownhillCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public SkiDownhillPlayer[] players;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y - cam.orthographicSize + 3f, -100f);
    }
}
