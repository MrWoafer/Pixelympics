using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EventsMedalsController : MonoBehaviour
{
    [Header("Settings")]
    public MedalRankingRule medalRankingRule = MedalRankingRule.jumpWhenTie;

    [Header("References")]
    [SerializeField]
    private Text[] medalHeaderTextBoxes;
    [SerializeField]
    private Text[] medalHolderTextBoxes;
    [HideInInspector]
    public OlympicsController olympicsController;
    [SerializeField]
    private Image[] medalImageBoxes;
    [SerializeField]
    private Sprite[] medalImages;
    [SerializeField]
    private GameObject[] medalRows;

    // Start is called before the first frame update
    void Start()
    {
        /*foreach (GameObject row in medalRows)
        {
            row.SetActive(true);
        }*/

        /*string[] _players = new string[] { "William", "Joe", "Wilmo", "Jono", "Samuel", "Joseph", "Doz", "Mrs C" };
        float[] _scores = new float[] { 1f, 2f, 5f, 5f, 7f, 7f, 7f, 8f };*/
        /*string[] _players = new string[] { "William", "Joe", "Wilmo", "Jono", "Samuel", "Joseph"};
        float[] _scores = new float[] { 1f, 2f, 5f, 5f, -7f, 8f };*/

        /*List<OlympicsPlayer> _players = new List<OlympicsPlayer>();

        _players.Add(new OlympicsPlayer("William"));
        _players.Add(new OlympicsPlayer("Joe"));
        _players.Add(new OlympicsPlayer("Wilmo"));
        _players.Add(new OlympicsPlayer("Jono"));
        _players.Add(new OlympicsPlayer("Samuel"));
        _players.Add(new OlympicsPlayer("Joseph"));
        _players.Add(new OlympicsPlayer("Doz"));
        //_players.Add(new OlympicsPlayer("Mrs C"));

        _players[0].latestRank = 1;
        _players[1].latestRank = 2;
        _players[2].latestRank = 2;
        _players[3].latestRank = 4;
        _players[4].latestRank = 8;
        _players[5].latestRank = 7;
        _players[6].latestRank = 4;
        //_players[7].latestRank = 4;

        DisplayRankings(_players.ToArray());*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayRankings(OlympicsPlayer[] players)
    {
        /// Just to mix up the order tied players display in
        Functions.Shuffle(players);

        players = players.OrderBy(i => i.latestRank).ToArray();

        for (int i = 0; i < players.Length && i < 8; i++)
        {
            medalHeaderTextBoxes[i].text = players[i].latestRank.ToString();

            medalHolderTextBoxes[i].text = players[i].playerName;

            medalImageBoxes[i].sprite = medalImages[Functions.RoundToRange(players[i].latestRank - 1, 0, 3)];
        }

        for (int i = players.Length; i < 8; i++)
        {
            medalRows[i].SetActive(false);
        }
    }

    public float[] RemoveDuplicateScores(float[] scores)
    {
        List<float> scoresNoDuplicates = new List<float>();
        foreach (float score in scores)
        {

            if (!scoresNoDuplicates.Contains(score))
            {
                scoresNoDuplicates.Add(score);
            }
        }

        return scoresNoDuplicates.ToArray();
    }

    public void ToMedalTable()
    {
        olympicsController.EndEventsMedalScreen();
    }
}

public enum MedalRankingRule
{
    jumpWhenTie,
    noJumpWhenTie
}