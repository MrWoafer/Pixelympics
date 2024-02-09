using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public float speed = 1f;
    public float scale = 10f;
    public float windSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1f, 5f);
        scale = Random.Range(3f, 11f);
        transform.localScale = new Vector3(scale, scale, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * windSpeed * Time.deltaTime, 0f, 0f);
    }
}
