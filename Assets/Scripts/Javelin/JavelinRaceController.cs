using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JavelinRaceController : MonoBehaviour
{
    [Header("References")]
    public GameObject foulTextObj;
    private Text foulText;

    public GameObject foulTextBackObj;
    private Text foulTextBack;

    public GameObject cloud;
    public GameObject bird;

    public JavelinJavelinController javelin;

    private bool fouled = false;

    // Start is called before the first frame update
    void Start()
    {
        foulText = foulTextObj.GetComponent<Text>();
        foulTextBack = foulTextBackObj.GetComponent<Text>();

        for (int i = 0; i < Mathf.RoundToInt(Random.Range(10f, 20f)); i++)
        {
            Instantiate(cloud, new Vector3(Random.Range(-40f, 160f), Random.Range(7f, 27f), 0f), Quaternion.identity);
        }
        for (int i = 0; i < Mathf.RoundToInt(Random.Range(3f, 7f)); i++)
        {
            var obj = Instantiate(bird, new Vector3(Random.Range(-40f, 160f), Random.Range(7f, 27f), 0f), Quaternion.identity);
            obj.GetComponent<SpriteRenderer>().flipX = Functions.RandomBool();
        }

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Javelin Record", 90f).ToString() + " m");
        Debug.Log("The current worst record is: " + PlayerPrefs.GetFloat("Javelin Worst Record", 5f).ToString() + " m");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Foul(string playerName)
    {
        if (!fouled)
        {
            Debug.Log(playerName + " got a foul!");
            foulText.text = "FOUL";
            foulTextBack.text = "FOUL";
            fouled = true;
            javelin.fouled = true;
        }
    }
}
