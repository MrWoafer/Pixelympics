using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class SpeedClimbingHold : MonoBehaviour
{
    [Header("Hold Settings")]
    public string button = "w";

    [Header("References")]
    public Text keyText;
    public GameObject arrowU;
    public GameObject arrowR;
    public GameObject arrowL;
    public GameObject arrowD;
    public SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        UpdateArrow();
        UpdateText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string ButtonToString(string key)
    {
        key = key.ToLower();

        if (Regex.IsMatch(key, @"^([a-z]|[0-9])$"))
        {
            return key;
        }
        else if (Regex.IsMatch(key, @"^\[[0-9]\]$"))
        {
            return key[1].ToString();
        }
        else
        {
            throw new System.Exception("Unable to convert key '" + key + "' to string.");
        }
    }

    public void UpdateText()
    {
        keyText.text = ButtonToString(button);
    }

    private void OnValidate()
    {
        UpdateText();
        UpdateArrow();
    }

    public void UpdateArrow()
    {
        arrowU.SetActive(false);
        arrowR.SetActive(false);
        arrowL.SetActive(false);
        arrowD.SetActive(false);

        if (ButtonToString(button) == "w")
        {
            arrowU.SetActive(true);
        }
        else if (ButtonToString(button) == "d")
        {
            arrowR.SetActive(true);
        }
        else if (ButtonToString(button) == "s")
        {
            arrowD.SetActive(true);
        }
        else if (ButtonToString(button) == "a")
        {
            arrowL.SetActive(true);
        }
    }

    public void RandomiseButton()
    {
        button = new string[] { "w", "a", "s", "d" }[Random.Range(0, 4)];
    }

    public void SetSprite(Sprite sprite)
    {
        spr.sprite = sprite;
    }
}
