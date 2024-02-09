using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class MedalController : MonoBehaviour
{
    [Header("Settings")]
    public MedalOrderRule orderRule = MedalOrderRule.totalPoints;
    public float tooltipWaitTime = 1f;
    public Vector2 tooltipOffset;

    [Header("References")]
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject medalTilePrefab;
    [HideInInspector]
    public OlympicsController olympicsController;
    [SerializeField]
    private MedalTooltipController tooltip;
    [SerializeField]
    private Text nextEventTextBox;
    [SerializeField]
    private Text upNextTextBox;

    private List<GameObject> medalTiles = new List<GameObject>();
    private List<OlympicsPlayer> players = new List<OlympicsPlayer>();
    private int[] selectedPlayers = new int[0];

    private float tileY;
    //private const float tileStartingY = -4f;
    private const float tileStartingY = -8f;
    private const float verticalSpacing = 3f;
    private int tileNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        tileY = tileStartingY;
        tileNum = 0;
        //selectedPlayers = new int[] { 0, 3, 5, 4 };

        HideTooltip();

        /*AddPlayer(new OlympicsPlayer("William", 1, 2, 3));
        AddPlayer(new OlympicsPlayer("Joe", 2, 2, 3));
        AddPlayer(new OlympicsPlayer("Wilmo", 2, 1, 0));
        AddPlayer(new OlympicsPlayer("Jono", 2, 3, 3));
        AddPlayer(new OlympicsPlayer("Joseph", 4, 1, 1));
        AddPlayer(new OlympicsPlayer("Samuel", 0, 1, 5));
        AddPlayer(new OlympicsPlayer("Mrs C", 0, 2, 1));
        AddPlayer(new OlympicsPlayer("Doz", 0, 3, 5));

        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetPlayerID(i);
        }

        players[0].medalsFor = new List<string>[] {new List<string>(new string[] { "Triple Jump" }), new List<string>(new string[] { "High Jump", "100m" }),
            new List<string>(new string[] { "Javelin", "Long Jump", "Karate" }), new List<string>(new string[] { "200m", "Rowing", "Hammer", "Ski Jump" })};

        UpdateDisplay();*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer(OlympicsPlayer player)
    {
        players.Add(player);

        UpdateDisplay();
    }

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
                AddMedalTile(player, Colours.GetColour(selectedPlayers.IndexOf(player.playerID)));
            }
            else
            {
                AddMedalTile(player);
            }
        }
    }
    
    public void ClearDisplay()
    {
        tileY = tileStartingY;
        tileNum = 0;

        while (medalTiles.Count > 0)
        {
            Destroy(medalTiles[0]);
            medalTiles.RemoveAt(0);
        }
    }

    private void ReorderPlayers()
    {
        if (orderRule == MedalOrderRule.totalPoints)
        {
            players = players.OrderByDescending(i => i.totalPoints).ThenByDescending(i => i.golds).ThenByDescending(i => i.silvers).ThenByDescending(i => i.bronzes).ToList();
        }
        else if (orderRule == MedalOrderRule.golds)
        {
            players = players.OrderByDescending(i => i.golds).ThenByDescending(i => i.silvers).ThenByDescending(i => i.bronzes).ToList();
        }
        else if (orderRule == MedalOrderRule.silvers)
        {
            players = players.OrderByDescending(i => i.silvers).ThenByDescending(i => i.golds).ThenByDescending(i => i.bronzes).ToList();
        }
        else if (orderRule == MedalOrderRule.bronzes)
        {
            players = players.OrderByDescending(i => i.bronzes).ThenByDescending(i => i.golds).ThenByDescending(i => i.silvers).ToList();
        }
        else if (orderRule == MedalOrderRule.playerName)
        {
            players = players.OrderBy(i => i.playerName).ToList();
        }
        else
        {
            throw new System.Exception("Unimplemented medal ordering rule: " + orderRule.ToString());
        }
    }

    private void AddMedalTile(OlympicsPlayer player)
    {
        GameObject tile = Instantiate(medalTilePrefab, canvas.transform);
        MedalsTileController medalTile = tile.GetComponent<MedalsTileController>();

        medalTile.SetDetails(player);
        medalTile.SetRowNum(tileNum);
        medalTile.medalController = this;

        tile.transform.localPosition = new Vector3(0f, tileY, 0f);
        medalTiles.Add(tile);

        tileY -= verticalSpacing;
        tileNum++;
    }

    private void AddMedalTile(OlympicsPlayer player, Color colour)
    {
        AddMedalTile(player);

        SetMedalTileColour(medalTiles.Count - 1, colour);
    }

    public void SetMedalTileColour(int medalTileIndex, Color colour)
    {
        MedalsTileController medalTile = medalTiles[medalTileIndex].GetComponent<MedalsTileController>();

        medalTile.SetColour(colour);
    }

    public void SetOrderRule(MedalOrderRule newOrderRule)
    {
        orderRule = newOrderRule;

        UpdateDisplay();
    }

    public void SetOrderRule(string newOrderRule)
    {
        switch (newOrderRule)
        {
            case "totalPoints": SetOrderRule(MedalOrderRule.totalPoints); break;
            case "golds": SetOrderRule(MedalOrderRule.golds); break;
            case "silvers": SetOrderRule(MedalOrderRule.silvers); break;
            case "bronzes": SetOrderRule(MedalOrderRule.bronzes); break;
            case "playerName": SetOrderRule(MedalOrderRule.playerName); break;
            default: throw new System.Exception("Unknown medal ordering rule: " + newOrderRule.ToString());
        }
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

    public void ShowTooltip(int rowNum, int columnNum)
    {
        tooltip.SetText(Functions.ArrayToString(players[rowNum].medalsFor[columnNum].ToArray()));

        tooltip.SetActive(true);

        MedalsTileController medalTile = medalTiles[rowNum].GetComponent<MedalsTileController>();

        tooltip.SetPosition(new Vector2(medalTile.GetColumnXCoord(columnNum), medalTile.GetYCoord()) + tooltipOffset);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void UpdateSelectedPlayers(int[] selectedPlayerIDs)
    {
        selectedPlayers = selectedPlayerIDs;

        UpdateDisplay();
    }

    public void NoMoreEvents()
    {
        nextEventTextBox.text = "End";
    }

    public void SetNextEventText(string text)
    {
        upNextTextBox.text = "Up Next: " + text;
    }
}

public enum MedalOrderRule
{
    totalPoints,
    golds,
    silvers,
    bronzes,
    playerName
}
