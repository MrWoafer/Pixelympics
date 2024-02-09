using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIToggleScript : MonoBehaviour
{
    [Header("References")]
    private GameObject playerSettingsObj;
    private PlayerSettingsScript playerSettings;

    public int playerNum = 0;

    // Start is called before the first frame update
    void Awake()
    {
        playerSettingsObj = GameObject.Find("PlayerSettings");
        playerSettings = playerSettingsObj.GetComponent<PlayerSettingsScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public void Toggle(bool value)
    public void Toggle()
    {
        //playerSettings.SetPlayerAI(playerNum, value);
        playerSettings.SetPlayerAI(playerNum, GetComponent<Toggle>().isOn);
    }
}
