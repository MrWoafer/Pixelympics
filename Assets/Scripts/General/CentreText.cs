using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentreText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string _text = "";
    public string text
    {
        set
        {
            SetText(value); 
        }
        get
        {
            return centreText.text;
        }
    }

    [Header("References")]
    [SerializeField]
    private Text centreText;
    [SerializeField]
    private Text centreTextDropShadow;

    private void OnValidate()
    {
        SetText(_text);
    }

    public void SetText(string text)
    {
        centreText.text = text;
        centreTextDropShadow.text = text;
    }
}
