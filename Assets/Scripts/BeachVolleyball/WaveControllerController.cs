using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveControllerController : MonoBehaviour
{
    [Header("References")]
    public GameObject wavePrefab;

    private float countdown = 0f;

    float x;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if (countdown <= 0)
        {
            SpawnWave();
            countdown = Random.Range(0.5f, 2f);
        }
    }

    private void SpawnWave()
    {
        x = Random.Range(-12f, 12f);

        GameObject.Instantiate(wavePrefab, new Vector3(x, -2.15f, 0f), Quaternion.identity);
    }
}
