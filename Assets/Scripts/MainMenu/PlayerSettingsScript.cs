using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerSettingsScript : MonoBehaviour
{
    //public static bool instance = false;

    [Header("Settings")]
    public string[] button1;
    public string[] button2;
    public string[] button3;
    public string[] button4;
    public string[] button5;

    public Difficulty[] difficulty;

    public string[] names;

    public bool[] isAI;

    public int[] playerIDs;

    private bool firstInstance = false;

    //[Header("References")]
    //public GameObject[] nameBoxes;
    //public GameObject[] difficultyBoxes;
    //public GameObject[] aiBoxes;

    // Start is called before the first frame update
    void Start()
    {
        bool dead = CheckIfShouldBeDestroyed();

        playerIDs = new int[4] { -1, -1, -1, -1 };

        names = new string[4] { "Blue", "Green", "Purple", "Yellow" };

        button1 = new string[4] { "a", "j", "[4]", "j" };
        button2 = new string[4] { "d", "l", "[6]", "l" };
        button3 = new string[4] { "e", "o", "[9]", "o" };
        button4 = new string[4] { "w", "i", "[8]", "i" };
        button5 = new string[4] { "s", "k", "[5]", "k" };

        //difficulty = new Difficulty[4] { Difficulty.Medium, Difficulty.Medium, Difficulty.Medium, Difficulty.Medium };
        difficulty = new Difficulty[4] { Difficulty.Hard, Difficulty.Hard, Difficulty.Hard, Difficulty.Hard };

        //isAI = new bool[4] { false, true, false, true };
        isAI = new bool[4] { false, true, true, true };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerName(int playerNum, string name)
    {
        names[playerNum] = name;
    }

    public void SetPlayerAI(int playerNum, bool ai)
    {
        isAI[playerNum] = ai;
    }

    public void SetPlayerDifficulty(int playerNum, int difficultyNum)
    {
        difficulty[playerNum] = (Difficulty)difficultyNum;
    }

    public void SetPlayerDifficulty(int playerNum, Difficulty playerDifficulty)
    {
        difficulty[playerNum] = playerDifficulty;
    }

    public void SetPlayerID(int playerNum, int playerID)
    {
        playerIDs[playerNum] = playerID;
    }

    public void OnLevelWasLoaded(int level)
    {
        //Debug.Log("Level: " + level.ToString());
        bool dead = CheckIfShouldBeDestroyed();

        if (!dead && level == 0)
        {
            int i;
            for (i = 0; i < 4; i++)
            {
                //Debug.Log(i);
                //Debug.Log(names[i]);
                GameObject.Find("NameP" + (i + 1).ToString()).GetComponent<TMP_InputField>().text = names[i];
            }
            for (i = 0; i < 4; i++)
            {
                GameObject.Find("DifficultyP" + (i + 1).ToString()).GetComponent<TMP_Dropdown>().value = (int)difficulty[i];
            }
            for (i = 0; i < 4; i++)
            {
                GameObject.Find("IsAIP" + (i + 1).ToString()).GetComponent<Toggle>().isOn = isAI[i];
            }
        }        
    }

    private bool CheckIfShouldBeDestroyed()
    {
        if (GameObject.FindObjectsOfType<PlayerSettingsScript>().Length == 1 || firstInstance)
        //if (instance == false)
        {
            //Debug.Log("I'm the first!");
            //instance = true;
            firstInstance = true;
            DontDestroyOnLoad(this.gameObject);
            return false;
        }
        else
        {
            //Debug.Log("I'm too late!");
            Destroy(gameObject);
            return true;
        }
    }
}
