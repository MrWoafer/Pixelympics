using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardTileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    public GameObject keyTextBox;
    [SerializeField]
    private TextMeshProUGUI keyText;
    [SerializeField]
    private GameObject courseeTextBox;
    [SerializeField]
    private TextMeshProUGUI courseText;
    [SerializeField]
    private TextMeshProUGUI valueText;
    [SerializeField]
    private GameObject ownerTextBox;
    [SerializeField]
    private TextMeshProUGUI ownerText;

    // Start is called before the first frame update
    void Awake()
    {
        /*keyText = keyTextObj.GetComponent<TextMeshProUGUI>();
        valueText = valueTextObj.GetComponent<TextMeshProUGUI>();
        ownerText = ownerTextObj.GetComponent<TextMeshProUGUI>();
        courseText = courseTextObj.GetComponent<TextMeshProUGUI>();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string key, string value)
    {
        SetText(key, "", value, new string[] { });
    }

    public void SetText(string key, string value, string course)
    {
        SetText(key, course, value, new string[] { });
    }

    public void SetText(string key, string value, string[] owners)
    {
        SetText(key, "", value, owners);
    }

    public void SetText(string key, string course, string value, string[] owners)
    {
        //Debug.Log(key + " " + value);
        //Debug.Log(keyText + " " + valueText);
        keyText.text = key;
        valueText.text = value;

        if (key == "")
        {
            keyTextBox.SetActive(false);
        }
        else
        {
            keyTextBox.SetActive(true);
        }

        if (course == "")
        {
            courseText.text = "";
            courseeTextBox.SetActive(false);
        }
        else
        {
            courseText.text = course;
            courseeTextBox.SetActive(true);
        }

        if (owners.Length == 0)
        {
            ownerTextBox.SetActive(false);
            //ownerTextObj.SetActive(false);
            ownerText.text = "";
        }
        else
        {
            ownerTextBox.SetActive(true);
            string ownerString = "";
            for (int i = 0; i < owners.Length; i++)
            {
                ownerString += owners[i];
                if (i < owners.Length - 1)
                {
                    ownerString += ", ";
                }
            }

            ownerText.text = ownerString;
        }
    }
}
