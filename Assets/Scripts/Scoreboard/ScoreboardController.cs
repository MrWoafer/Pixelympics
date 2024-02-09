using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    [Header("Settings")]
    public ScoreboardOrderRule orderRule;
    public int roundToOrderBy = 1;
    [Min(0)]
    public int scrollOffset = 0;
    public bool higherIsBetter = true;

    [Header("References")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject scoreboardTilePrefab;
    [HideInInspector]
    public OlympicsController olympicsController;
    [SerializeField]
    private Text[] roundTexts;
    [SerializeField]
    private Text eventText;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Text startButtonText;

    private List<GameObject> scoreboardTiles = new List<GameObject>();
    private List<OlympicsPlayer> players = new List<OlympicsPlayer>();
    private int[] selectedPlayers = new int[0];

    private int scoreboardTileNum;
    private float tileY;
    private const float tileStartingY = -8f;
    private const float verticalSpacing = 3f;

    private string currentEvent;
    private int numOfPlayers = 1;
    private int resolution = 2;

    // Start is called before the first frame update
    void Start()
    {
        scoreboardTileNum = 0;
        tileY = tileStartingY;

        /*AddPlayer(new ScoreboardPlayer("William", new float[] { 1f, float.MinValue, 2.3f, 3.45f }));
        AddPlayer(new ScoreboardPlayer("Joe", new float[] { 3.675f, 2.1f, 1.9342f, float.MinValue }));
        AddPlayer(new ScoreboardPlayer("Wilmo", new float[] { -5f, 42f, 192.34f, 3.14f }));
        AddPlayer(new ScoreboardPlayer("Jono", new float[] { 10.2f, 35f, 3.33f, 7.21f, 9.87f, 13.45f, 15.43f }));
        AddPlayer(new ScoreboardPlayer("Samuel", new float[] { }));
        AddPlayer(new ScoreboardPlayer("Joseph", new float[] { 1f, 3.6f }));

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetPlayerID(i);
        }

        SetEvent("Test", 4);
        UpdateSelectedPlayers(new int[] { 0, 3, 5, 4 });*/
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ScrollLeft();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ScrollRight();
        }
    }

    public void AddPlayer(OlympicsPlayer player)
    {
        players.Add(player);

        UpdateDisplay();
    }

    /*public void AddPlayer(ScoreboardPlayer player)
    {
        players.Add(new OlympicsPlayer(player.playerID, player.playerName, player.scores));

        UpdateDisplay();
    }

    public void AddPlayers(ScoreboardPlayer[] players)
    {
        foreach (ScoreboardPlayer player in players)
        {
            AddPlayer(player);
        }
    }*/

    public void AddPlayers(OlympicsPlayer[] players)
    {
        foreach (OlympicsPlayer player in players)
        {
            AddPlayer(player);
        }
    }

    public void UpdateDisplay()
    {
        ClearDisplay();

        ReorderPlayers();

        foreach (OlympicsPlayer player in players)
        {
            if (selectedPlayers.Contains(player.playerID))
            {
                AddScoreboardTile(player, Colours.GetColour(selectedPlayers.IndexOf(player.playerID)));
            }
            else
            {
                AddScoreboardTile(player);
            }
        }

        for (int i = 0; i < roundTexts.Length; i++)
        {
            roundTexts[i].text = (scrollOffset + i + 1).ToString();
        }

        DisplayStartButton();
    }

    public void ClearDisplay()
    {
        scoreboardTileNum = 0;
        tileY = tileStartingY;

        while (scoreboardTiles.Count > 0)
        {
            Destroy(scoreboardTiles[0]);
            scoreboardTiles.RemoveAt(0);
        }
    }

    private void ReorderPlayers()
    {
        if (orderRule == ScoreboardOrderRule.bestScore)
        {
            if (higherIsBetter)
            {
                players = players.OrderByDescending(i => i.GetBestScore(currentEvent)).ToList();
            }
            else
            {
                players = players.OrderBy(i => i.GetBestScore(currentEvent)).ToList();
            }
        }
        else if (orderRule == ScoreboardOrderRule.specificRound)
        {
            if (higherIsBetter)
            {
                players = players.OrderByDescending(i =>
                {
                    if (roundToOrderBy - 1 < i.currentEventScores.Count)
                    {
                        if (i.currentEventScores[roundToOrderBy - 1] == float.MinValue || i.currentEventScores[roundToOrderBy - 1] == float.MaxValue)
                        {
                            /// This test is just to make failed scores (stored as float.MinValue) appear above people who haven't yet attempted the round
                            return OlympicsConfig.FoulOrderingValue(higherIsBetter);
                        }
                        else
                        {
                            return i.currentEventScores[roundToOrderBy - 1];
                        }
                    }
                    else
                    {
                        return higherIsBetter ? float.MinValue : float.MaxValue;
                    }
                }
                ).ThenByDescending(i => i.GetBestScore(currentEvent)).ToList();
            }
            else
            {
                players = players.OrderBy(i =>
                {
                    if (roundToOrderBy - 1 < i.currentEventScores.Count)
                    {
                        if (i.currentEventScores[roundToOrderBy - 1] == float.MinValue || i.currentEventScores[roundToOrderBy - 1] == float.MaxValue)
                        {
                            /// This test is just to make failed scores (stored as float.MinValue) appear above people who haven't yet attempted the round
                            return OlympicsConfig.FoulOrderingValue(higherIsBetter);
                        }
                        else
                        {
                            return i.currentEventScores[roundToOrderBy - 1];
                        }
                    }
                    else
                    {
                        return higherIsBetter ? float.MinValue : float.MaxValue;
                    }
                }
                ).ThenBy(i => i.GetBestScore(currentEvent)).ToList();
            }
        }
        else if (orderRule == ScoreboardOrderRule.playerName)
        {
            players = players.OrderBy(i => i.playerName).ToList();
        }
        else
        {
            throw new System.Exception("Unimplemented scoreboard ordering rule: " + orderRule.ToString());
        }
    }

    private void AddScoreboardTile(OlympicsPlayer player)
    {
        GameObject tile = Instantiate(scoreboardTilePrefab, canvas.transform);
        ScoreboardTileController scoreboardTile = tile.GetComponent<ScoreboardTileController>();

        //scoreboardTile.SetDetails(player, higherIsBetter, resolution: resolution, scrollOffset: scrollOffset);
        scoreboardTile.SetDetails(player.playerName, player.currentEventScores.ToArray(), player.GetBestScore(currentEvent), resolution: resolution, scrollOffset: scrollOffset);

        int scoreboardTileNum_temp = scoreboardTileNum;
        scoreboardTile.playerButton.onClick.AddListener(() => SelectPlayer(scoreboardTileNum_temp));

        tile.transform.localPosition = new Vector3(0f, tileY, 0f);
        scoreboardTiles.Add(tile);

        tileY -= verticalSpacing;
        scoreboardTileNum++;
    }

    private void AddScoreboardTile(OlympicsPlayer player, Color colour)
    {
        AddScoreboardTile(player);

        SetScoreboardTileColour(scoreboardTiles.Count - 1, colour);
    }

    public void SetScoreboardTileColour(int medalTileIndex, Color colour)
    {
        ScoreboardTileController scoreboardTile = scoreboardTiles[medalTileIndex].GetComponent<ScoreboardTileController>();

        scoreboardTile.SetColour(colour);
    }

    public void ClearPlayers()
    {
        players = new List<OlympicsPlayer>();

        UpdateDisplay();
    }

    public void NextEvent()
    {
        olympicsController.NextEvent();
    }

    public void SetOrderRule(ScoreboardOrderRule newOrderRule)
    {
        orderRule = newOrderRule;

        UpdateDisplay();
    }

    public void SetOrderRule(string newOrderRule)
    {
        switch (newOrderRule)
        {
            case "bestScore": SetOrderRule(ScoreboardOrderRule.bestScore); break;
            case "specificRound": SetOrderRule(ScoreboardOrderRule.specificRound); break;
            case "playerName": SetOrderRule(ScoreboardOrderRule.playerName); break;
            default: throw new System.Exception("Unknown scoreboard ordering rule: " + newOrderRule.ToString());
        }
    }

    public void SetOrderToRuleToRound(int newRoundToOrderBy)
    {
        roundToOrderBy = newRoundToOrderBy;
        SetOrderRule(ScoreboardOrderRule.specificRound);
    }

    public void SetScrollOffset(int newScrollOffset)
    {
        scrollOffset = newScrollOffset;
        if (scrollOffset < 0)
        {
            scrollOffset = 0;
        }

        UpdateDisplay();
    }

    public void Scroll(int amount)
    {
        SetScrollOffset(scrollOffset + amount);
    }

    public void ScrollLeft()
    {
        Scroll(-1);
    }

    public void ScrollRight()
    {
        Scroll(1);
    }

    private void SelectPlayer(int playerNum)
    {
        Debug.Log("Selected player: " + players[playerNum].playerName);

        if (olympicsController != null)
        {
            olympicsController.SelectPlayerForEvent(players[playerNum].playerID);
        }
        else
        {
            List<int> selectedPlayersList = selectedPlayers.ToList();
            if (selectedPlayers.Contains(players[playerNum].playerID))
            {
                selectedPlayersList.Remove(players[playerNum].playerID);
            }
            else if (selectedPlayers.Length < 4)
            {
                
                selectedPlayersList.Add(players[playerNum].playerID);
            }
            selectedPlayers = selectedPlayersList.ToArray();

            UpdateSelectedPlayers(selectedPlayers);
        }
    }

    public void SetEvent(string eventName, int numberOfPlayers, int resolution)
    {
        currentEvent = eventName;
        eventText.text = eventName;
        numOfPlayers = numberOfPlayers;
        this.resolution = resolution;

        UpdateDisplay();
    }

    public void EndEvent()
    {
        olympicsController.EndEvent();
    }

    public void UpdateSelectedPlayers(int[] selectedPlayerIDs)
    {
        selectedPlayers = selectedPlayerIDs;

        UpdateDisplay();

        DisplayStartButton();
    }

    public void DisplayStartButton()
    {
        /*if (numOfPlayers > 1 && selectedPlayers.Length == numOfPlayers)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }*/

        if (numOfPlayers > 1 && selectedPlayers.Length == numOfPlayers)
        {
            startButton.enabled = true;
            startButtonText.text = "Start Event";
        }
        else
        {
            startButton.enabled = false;
            startButtonText.text = selectedPlayers.Length + "/" + numOfPlayers + " Players";
        }
    }

    public void StartEvent()
    {
        olympicsController.StartEvent();
    }

    public void SetHigherIsBetter(bool higherIsBetter)
    {
        this.higherIsBetter = higherIsBetter;
    }
}

public enum ScoreboardOrderRule
{
    bestScore,
    specificRound,
    playerName
}
