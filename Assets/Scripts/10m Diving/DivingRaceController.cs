using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DivingRaceController : MonoBehaviour
{
    [Header("References")]
    public GameObject cloud;
    public GameObject bird;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Mathf.RoundToInt(Random.Range(10f, 20f)); i++)
        {
            //Instantiate(cloud, new Vector3(Random.Range(-150f, 150), Random.Range(10f, 70f), Random.Range(70f, 100f)), Quaternion.identity);
            Instantiate(cloud, new Vector3(Random.Range(-70f, 70f), Random.Range(10f, 40f), Random.Range(20f, 50f)), Quaternion.identity);
        }
        /*for (int i = 0; i < Mathf.RoundToInt(Random.Range(3f, 7f)); i++)
        {
            var obj = Instantiate(bird, new Vector3(Random.Range(-150, 150), Random.Range(10f, 60f), Random.Range(70f, 100f)), Quaternion.identity);
            obj.GetComponent<SpriteRenderer>().flipX = Functions.RandomBool();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
