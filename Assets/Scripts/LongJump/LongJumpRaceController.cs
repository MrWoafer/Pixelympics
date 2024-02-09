using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LongJumpRaceController : MonoBehaviour
{
    [Header("References")]
    public GameObject foulTextObj;
    private Text foulText;

    public GameObject foulTextBackObj;
    private Text foulTextBack;

    public GameObject recordTextObj;
    public GameObject winnerTextObj;
    private TextMeshProUGUI recordText;
    private TextMeshProUGUI winnerText;

    public GameObject foulX;

    private bool fouled = false;

    // Start is called before the first frame update
    void Start()
    {
        foulText = foulTextObj.GetComponent<Text>();
        foulTextBack = foulTextBackObj.GetComponent<Text>();

        Debug.Log("The current record is: " + PlayerPrefs.GetFloat("Long Jump Record", 9f).ToString() + " m");
        Debug.Log("The current worst record is: " + PlayerPrefs.GetFloat("Long Jump Worst Record", 1f).ToString() + " m");

        foulX.SetActive(false);

        recordText = recordTextObj.GetComponent<TextMeshProUGUI>();
        winnerText = winnerTextObj.GetComponent<TextMeshProUGUI>();

        SetRecordDistance(PlayerPrefs.GetFloat("Long Jump Record", 9f));
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
            //foulText.text = "FOUL";
            //foulTextBack.text = "FOUL";
            fouled = true;
            foulX.SetActive(true);
        }
    }

    public void SetWinnerDistance(float distance)
    {
        winnerText.text = distance.ToString("n2");
    }
    public void SetRecordDistance(float distance)
    {
        recordText.text = distance.ToString("n2");
    }
    public void SetRecordDistance(string text)
    {
        recordText.text = text;
    }
}
