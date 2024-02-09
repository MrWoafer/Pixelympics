using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TournamentController : MonoBehaviour
{
    private readonly string[] events = new string[]
    { "100m", "200m", "400m", "Hurdles", "Rowing", "Javelin", "Hammer", "Shot Put", "Long Jump", "Triple Jump", "High Jump", "Pole Vault", "100m Freestyle", "Weightlifting", "Archery", "Skeet",
        "Speed Climbing", "Ski Jump", "Karate", "Wrestling" };

    [SerializeField]
    private List<string> addedEvents = new List<string>();
    private List<GameObject> addedEventTiles = new List<GameObject>();

    [SerializeField]
    private List<string> addedPlayers = new List<string>();
    [SerializeField]
    private List<bool> addedPlayersIsAI = new List<bool>();
    private List<GameObject> addedPlayerTiles = new List<GameObject>();

    [Header("References")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    public GameObject eventTilePrefab;
    [SerializeField]
    public GameObject playerTilePrefab;
    private OlympicsController olympicsController;
    [SerializeField]
    private CameraControllerMainMenu camController;
    [SerializeField]
    private TMP_InputField olympicsNameInput;
    [SerializeField]
    private CameraControllerMainMenu cam;

    private const float verticalSpacing = 2.5f;

    private const float eventStartingY = 8f;
    private float eventY;
    
    private const float playerStartingY = 8f;
    private float playerY;

    private int eventTileNum = 0;
    private int playerTileNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        eventY = eventStartingY;
        eventTileNum = 0;

        playerY = playerStartingY;
        playerTileNum = 0;

        //addedEvents = new List<string>(new string[] { "Triple Jump" });
        switch (Random.Range(0, 4))
        {
            default: addedEvents = new List<string>(new string[] { "100m", "200m", "400m" }); break;
            case 1: addedEvents = new List<string>(new string[] { "Long Jump", "Triple Jump", "High Jump", "Pole Vault" }); break;
            case 2: addedEvents = new List<string>(new string[] { "Javelin", "Hammer" }); break;
            case 3: addedEvents = new List<string>(new string[] { "Karate", "Wrestling" }); break;
        }

        addedPlayers = new List<string>(new string[] { "Blue", "Green", "Purple", "Yellow" });
        addedPlayersIsAI = new List<bool>(new bool[] { true, true, true, true });

        UpdateEventDisplay();
        UpdatePlayerDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewEvent()
    {
        Debug.Log("Added event");

        addedEvents.Add("100m");
        UpdateEventDisplay();

        DebugLogEvents();
    }

    public void AddEventTile(string eventName)
    {
        GameObject tile = Instantiate(eventTilePrefab, canvas.transform);
        EventTile eventTile = tile.GetComponent<EventTile>();

        tile.transform.localPosition = new Vector3(0f, eventY, 0f);
        addedEventTiles.Add(tile);

        foreach(string i in events)
        {
            eventTile.dropdown.options.Add(new TMP_Dropdown.OptionData(i));
        }

        for (int i = 0; i < events.Length; i++)
        {
            if (events[i] == eventName)
            {
                eventTile.dropdown.value = i;
                break;
            }
        }
        
        eventTile.dropdown.onValueChanged.AddListener((int i) => GetChosenEvents());

        int eventTileNum_temp = eventTileNum;
        eventTile.button.onClick.AddListener(() => RemoveEvent(eventTileNum_temp));

        eventTileNum++;
        eventY -= verticalSpacing;
    }

    private void RemoveEvent(int eventToRemove)
    {
        Debug.Log("Removed event " + eventToRemove.ToString());

        addedEvents.RemoveAt(eventToRemove);
        UpdateEventDisplay();
        GetChosenEvents();
    }

    public void UpdateEventDisplay()
    {
        ClearEventDisplay();

        foreach (string addedEvent in addedEvents)
        {
            AddEventTile(addedEvent);
        }
    }

    private void ClearEventDisplay()
    {
        eventY = eventStartingY;
        eventTileNum = 0;

        while (addedEventTiles.Count > 0)
        {
            Destroy(addedEventTiles[0]);
            addedEventTiles.RemoveAt(0);
        }
    }

    private void GetChosenEvents()
    {
        addedEvents = new List<string>();
        
        foreach (GameObject tile in addedEventTiles)
        {
            addedEvents.Add(events[tile.GetComponent<EventTile>().dropdown.value]);
        }

        DebugLogEvents();
    }

    public void DebugLogEvents()
    {
        string displayString = "Events: ";
        for (int i = 0; i < addedEvents.Count; i++)
        {
            displayString += addedEvents[i];
            if (i < addedEvents.Count - 1)
            {
                displayString += ", ";
            }
        }

        Debug.Log(displayString);
    }


    public void AddNewPlayer()
    {
        Debug.Log("Added player");

        addedPlayers.Add("Blue");
        addedPlayersIsAI.Add(true);
        UpdatePlayerDisplay();

        DebugLogPlayers();
    }

    public void AddPlayerTile(string playerName, bool isAI)
    {
        GameObject tile = Instantiate(playerTilePrefab, canvas.transform);
        PlayerTile playerTile = tile.GetComponent<PlayerTile>();

        tile.transform.localPosition = new Vector3(0f, playerY, 0f);
        addedPlayerTiles.Add(tile);

        playerTile.inputField.text = playerName;
        playerTile.isAITickBox.isOn = isAI;

        playerTile.inputField.onEndEdit.AddListener((string i) => GetChosenPlayers());

        int playerTileNum_temp = playerTileNum;
        playerTile.button.onClick.AddListener(() => RemovePlayer(playerTileNum_temp));
        playerTile.isAITickBox.onValueChanged.AddListener((_isAI) => SetPlayerAI(playerTileNum_temp, _isAI));

        playerTile.inputField.onSelect.AddListener((i) => cam.Typing(true));
        playerTile.inputField.onDeselect.AddListener((i) => cam.Typing(false));

        playerTileNum++;
        playerY -= verticalSpacing;
    }

    private void RemovePlayer(int playerToRemove)
    {
        Debug.Log("Removed player " + addedPlayers[playerToRemove]);

        addedPlayers.RemoveAt(playerToRemove);
        addedPlayersIsAI.RemoveAt(playerToRemove);
        UpdatePlayerDisplay();
        GetChosenPlayers();
    }

    public void SetPlayerAI(int player, bool isAI)
    {
        Debug.Log("Changed player " + addedPlayers[player] + " AI status to: " + isAI);

        addedPlayersIsAI[player] = isAI;

        UpdatePlayerDisplay();
        GetChosenPlayers();
    }

    public void UpdatePlayerDisplay()
    {
        ClearPlayerDisplay();

        for (int i = 0; i < addedPlayers.Count; i++)
        {
            AddPlayerTile(addedPlayers[i], addedPlayersIsAI[i]);
        }
    }

    private void ClearPlayerDisplay()
    {
        playerY = playerStartingY;
        playerTileNum = 0;

        while (addedPlayerTiles.Count > 0)
        {
            Destroy(addedPlayerTiles[0]);
            addedPlayerTiles.RemoveAt(0);
        }
    }

    private void GetChosenPlayers()
    {
        addedPlayers = new List<string>();
        addedPlayersIsAI = new List<bool>();

        foreach (GameObject tile in addedPlayerTiles)
        {
            addedPlayers.Add(tile.GetComponent<PlayerTile>().inputField.text);
            addedPlayersIsAI.Add(tile.GetComponent<PlayerTile>().isAITickBox.isOn);
        }

        DebugLogPlayers();
    }

    public void DebugLogPlayers()
    {
        string displayString = "Players: ";
        for (int i = 0; i < addedPlayers.Count; i++)
        {
            displayString += addedPlayers[i];

            if (addedPlayersIsAI[i])
            {
                displayString += " [AI]";
            }

            if (i < addedPlayers.Count - 1)
            {
                displayString += ", ";
            }
        }

        Debug.Log(displayString);
    }

    public void StartOlympics()
    {
        GetChosenEvents();
        GetChosenPlayers();

        if (addedEvents.Count > 0 && addedPlayers.Count > 0)
        {
            olympicsController = GameObject.Find("OlympicsController").GetComponent<OlympicsController>();
            olympicsController.StartOlympics(olympicsNameInput.text, addedEvents.ToArray(), addedPlayers.ToArray(), addedPlayersIsAI.ToArray());
        }
    }

    public void ToggleAllAI()
    {
        bool allAI = true;
        for (int i = 0; i < addedPlayersIsAI.Count; i++)
        {
            if (addedPlayersIsAI[i] == false)
            {
                allAI = false;
            }
        }

        if (allAI)
        {
            Debug.Log("Set all players to not AI");
            for (int i = 0; i < addedPlayersIsAI.Count; i++)
            {
                addedPlayersIsAI[i] = false;
            }
        }
        else
        {
            Debug.Log("Set all players to AI");
            for (int i = 0; i < addedPlayersIsAI.Count; i++)
            {
                addedPlayersIsAI[i] = true;
            }
        }

        UpdatePlayerDisplay();
        DebugLogPlayers();
    }

    public void SetIsTyping(bool isTyping)
    {
        camController.Typing(isTyping);
    }
}
