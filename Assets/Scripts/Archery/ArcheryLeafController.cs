using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryLeafController : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;
    private float rotationCoefficient;

    [Header("References")]
    private ArcheryConfig config;

    private bool begun = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.eulerAngles = new Vector3(0f, 0f, Random.Range(0f, 360f));
        rotationCoefficient = Random.Range(0.9f, 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (begun)
        {
            transform.position += config.GetWindDirection() * config.windSpeed * Time.fixedDeltaTime;

            if (config.GetWindDirection().x >= 0f)
            {
                transform.eulerAngles += new Vector3(0f, 0f, -rotationCoefficient * config.windSpeed * config.windRotationScalar);
            }
            else
            {
                transform.eulerAngles += new Vector3(0f, 0f, rotationCoefficient * config.windSpeed * config.windRotationScalar);
            }
        }
    }

    public void Begin(ArcheryConfig configReference)
    {
        begun = true;
        config = configReference;
        Destroy(gameObject, lifetime);
    }
}
