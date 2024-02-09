using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryLeafSpawner : MonoBehaviour
{
    [Header("Settings")]
    public float minSpawnCountdown = 0.5f;
    public float maxSpawnCountdown = 1f;
    public float rangeX = 10f;
    public float rangeY = 5f;
    public float rangeZ = 10f;

    [Header("References")]
    public GameObject leafPrefab;
    public GameObject configObj;
    private ArcheryConfig config;

    private float countdown;

    // Start is called before the first frame update
    void Start()
    {
        countdown = Random.Range(minSpawnCountdown, maxSpawnCountdown);

        config = configObj.GetComponent<ArcheryConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            countdown = Random.Range(minSpawnCountdown, maxSpawnCountdown);
            countdown /= config.windSpeed;

            Vector3 coords = transform.position + new Vector3(Random.Range(-rangeX, rangeX), Random.Range(-rangeY, rangeY), Random.Range(-rangeZ, rangeZ));
            GameObject leaf = Instantiate(leafPrefab, coords, Quaternion.identity);
            leaf.GetComponent<ArcheryLeafController>().Begin(config);
        }
    }
}
