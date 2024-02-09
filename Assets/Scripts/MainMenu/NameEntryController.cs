using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameEntryController : MonoBehaviour
{
    [Header("References")]
    public GameObject cameraObj;
    private CameraControllerMainMenu cameraScript;

    private GameObject playerSettingsObj;
    private PlayerSettingsScript playerSettings;

    public int playerNum = 0;

    // Start is called before the first frame update
    void Awake()
    {
        cameraScript = cameraObj.GetComponent<CameraControllerMainMenu>();
        playerSettingsObj = GameObject.Find("PlayerSettings");
        playerSettings = playerSettingsObj.GetComponent<PlayerSettingsScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        cameraScript.Typing(true);
    }
    public void Deselect()
    {
        //cameraScript.Typing(false);
        EndEdit();
    }
    public void EndEdit()
    {
        playerSettings.SetPlayerName(playerNum, GetComponent<TMP_InputField>().text);
        cameraScript.Typing(false);
    }
}
