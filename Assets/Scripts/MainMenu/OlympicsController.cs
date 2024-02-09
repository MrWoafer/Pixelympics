using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class OlympicsController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private string olympicsName;
    [SerializeField]
    private string[] events;
    [SerializeField]
    private MedalRankingRule medalRankingRule = MedalRankingRule.jumpWhenTie;

    [Header("References")]
    [SerializeField]
    private PlayerSettingsScript playerSettings;

    private OlympicsPlayer[] players;

    private List<int> currentPlayerIDs = new List<int>();

    private bool receivedScores = false;
    private int receivedScoreCount = 0;
    private bool inEvent = false;

    private Sprint100Controller[] sprint100Players;
    private Sprint200Controller[] sprint200Players;
    private Run400Controller[] sprint400Players;
    private HurdlesController[] hurdlesPlayers;
    private RowingPlayer[] rowingPlayers;
    private JavelinPlayerController javelinPlayer;
    private JavelinJavelinController javelinJavelin;
    private HammerThrower hammerPlayer;
    private LongJumpController longJumpPlayer;
    private TripleJumpPlayerController tripleJumpPlayer;
    private HighJumpPlayerController highJumpPlayer;
    private PoleVaultPlayerController poleVaultPlayer;
    private WeightliftingPlayerController weightliftingPlayer;
    private ArcheryPlayerController archeryPlayer;
    private SkeetPlayerController skeetPlayer;
    private SpeedClimber[] speedClimbingPlayers;
    private SkiJumper skiJumpPlayer;
    private KaratePlayerController[] karatePlayers;
    private KarateRefereeController karateReferee;
    private Wrestler[] wrestlingPlayers;
    private WrestlingReferee wrestlingReferee;
    private ShotPutPlayer shotPutPlayer;
    private SwimmingFreestylePlayer[] swimmingFreestylePlayers;

    public int eventNum = -10;
    private int numOfEvents;

    private bool firstInstance = false;

    private MedalController medalController;
    private ScoreboardController scoreboardController;
    private EventsMedalsController eventsMedalsController;

    public string currentEvent
    {
        get
        {
            return events[eventNum];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckIfShouldBeDestroyed();
    }

    // Update is called once per frame
    void Update()
    {
        if (inEvent && receivedScores && Input.GetKeyDown(KeyCode.Return))
        {
            inEvent = false;

            Cursor.lockState = CursorLockMode.None;

            ResetSelectedPlayers();

            SceneManager.LoadScene("Scoreboard");
        }
    }

    public void StartOlympics(string _olympicsName, string[] _events, string[] _players, bool[] _isAIs)
    {
        olympicsName = _olympicsName;
        events = _events;
        players = new OlympicsPlayer[_players.Length];

        for (int i = 0; i < _players.Length; i++)
        {
            players[i] = new OlympicsPlayer(_players[i], _isAIs[i]);
        }

        numOfEvents = events.Length;
        eventNum = 0;

        Debug.Log("-------------------");
        Debug.Log("Starting Pixelympics: " + olympicsName + "!");
        DebugLogPlayers();
        DebugLogEvents();
        Debug.Log("-------------------");

        AssignPlayerIDs();

        eventNum = -1;
        LoadMedalScreen();
    }

    public void OnLevelWasLoaded(int level)
    {
        bool dead = CheckIfShouldBeDestroyed();

        Debug.Log("Checks done!");

        if (!dead && eventNum != -10)
        {
            Debug.Log("Made it!");

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "MainMenu")
            {
                eventNum = -10;
            }
            else if (currentScene == "Medals")
            {
                medalController = GameObject.Find("MedalController").GetComponent<MedalController>();
                medalController.olympicsController = this;

                medalController.AddPlayers(players);

                if (eventNum == events.Length - 1)
                {
                    medalController.NoMoreEvents();
                    medalController.SetNextEventText("Awards");
                }
                else
                {
                    medalController.SetNextEventText(events[eventNum + 1]);
                }
            }
            else if (currentScene == "Scoreboard")
            {
                scoreboardController = GameObject.Find("ScoreboardController").GetComponent<ScoreboardController>();
                scoreboardController.olympicsController = this;

                scoreboardController.SetHigherIsBetter(OlympicsConfig.IsHigherBetter(currentEvent));
                scoreboardController.SetEvent(currentEvent, OlympicsConfig.GetNumOfPlayers(currentEvent), OlympicsConfig.GetResolution(currentEvent));
                scoreboardController.AddPlayers(players);
            }
            else if (currentScene == "EventMedals")
            {
                eventsMedalsController = GameObject.Find("EventsMedalsController").GetComponent<EventsMedalsController>();
                eventsMedalsController.olympicsController = this;

                eventsMedalsController.DisplayRankings(players);
            }
            else if (currentScene == "100Sprint")
            {
                sprint100Players = new Sprint100Controller[4];
                sprint100Players[0] = GameObject.Find("TestChar").GetComponent<Sprint100Controller>();
                sprint100Players[1] = GameObject.Find("TestChar2").GetComponent<Sprint100Controller>();
                sprint100Players[2] = GameObject.Find("TestChar3").GetComponent<Sprint100Controller>();
                sprint100Players[3] = GameObject.Find("TestChar4").GetComponent<Sprint100Controller>();

                foreach (Sprint100Controller player in sprint100Players)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "200Metres")
            {
                sprint200Players = new Sprint200Controller[4];
                sprint200Players[0] = GameObject.Find("TestChar (4)").GetComponent<Sprint200Controller>();
                sprint200Players[1] = GameObject.Find("TestChar (7)").GetComponent<Sprint200Controller>();
                sprint200Players[2] = GameObject.Find("TestChar (6)").GetComponent<Sprint200Controller>();
                sprint200Players[3] = GameObject.Find("TestChar (5)").GetComponent<Sprint200Controller>();

                foreach (Sprint200Controller player in sprint200Players)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "400Metres")
            {
                sprint400Players = new Run400Controller[4];
                sprint400Players[0] = GameObject.Find("TestChar").GetComponent<Run400Controller>();
                sprint400Players[1] = GameObject.Find("TestChar (1)").GetComponent<Run400Controller>();
                sprint400Players[2] = GameObject.Find("TestChar (2)").GetComponent<Run400Controller>();
                sprint400Players[3] = GameObject.Find("TestChar (3)").GetComponent<Run400Controller>();

                foreach (Run400Controller player in sprint400Players)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "100Hurdles")
            {
                hurdlesPlayers = new HurdlesController[4];
                hurdlesPlayers[0] = GameObject.Find("TestChar").GetComponent<HurdlesController>();
                hurdlesPlayers[1] = GameObject.Find("TestChar2").GetComponent<HurdlesController>();
                hurdlesPlayers[2] = GameObject.Find("TestChar3").GetComponent<HurdlesController>();
                hurdlesPlayers[3] = GameObject.Find("TestChar4").GetComponent<HurdlesController>();

                foreach (HurdlesController player in hurdlesPlayers)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "Rowing")
            {
                rowingPlayers = new RowingPlayer[4];
                rowingPlayers[0] = GameObject.Find("Boat 1").GetComponent<RowingPlayer>();
                rowingPlayers[1] = GameObject.Find("Boat 2").GetComponent<RowingPlayer>();
                rowingPlayers[2] = GameObject.Find("Boat 3").GetComponent<RowingPlayer>();
                rowingPlayers[3] = GameObject.Find("Boat 4").GetComponent<RowingPlayer>();

                foreach (RowingPlayer player in rowingPlayers)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "Javelin")
            {
                javelinPlayer = GameObject.Find("Player").GetComponent<JavelinPlayerController>();
                javelinPlayer.SetOlympicsController(this);

                javelinJavelin = GameObject.Find("Javelin").GetComponent<JavelinJavelinController>();
                javelinJavelin.SetOlympicsController(this);
            }
            else if (currentScene == "Hammer")
            {
                hammerPlayer = GameObject.Find("Player").GetComponent<HammerThrower>();
                hammerPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "LongJump")
            {
                longJumpPlayer = GameObject.Find("Player").GetComponent<LongJumpController>();
                longJumpPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "TripleJump")
            {
                tripleJumpPlayer = GameObject.Find("Player").GetComponent<TripleJumpPlayerController>();
                tripleJumpPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "HighJump")
            {
                highJumpPlayer = GameObject.Find("Player").GetComponent<HighJumpPlayerController>();
                highJumpPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "PoleVault")
            {
                poleVaultPlayer = GameObject.Find("Player").GetComponent<PoleVaultPlayerController>();
                poleVaultPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "Weightlifting")
            {
                weightliftingPlayer = GameObject.Find("Player").GetComponent<WeightliftingPlayerController>();
                weightliftingPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "Archery")
            {
                archeryPlayer = GameObject.Find("Player").GetComponent<ArcheryPlayerController>();
                archeryPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "Skeet")
            {
                skeetPlayer = GameObject.Find("Player").GetComponent<SkeetPlayerController>();
                skeetPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "SpeedClimbing")
            {
                speedClimbingPlayers = new SpeedClimber[2];
                speedClimbingPlayers[0] = GameObject.Find("Player 1").GetComponent<SpeedClimber>();
                speedClimbingPlayers[1] = GameObject.Find("Player 2").GetComponent<SpeedClimber>();

                foreach (SpeedClimber player in speedClimbingPlayers)
                {
                    player.SetOlympicsController(this);
                }
            }
            else if (currentScene == "SkiJump")
            {
                skiJumpPlayer = GameObject.Find("Player").GetComponent<SkiJumper>();
                skiJumpPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "Karate")
            {
                karatePlayers = new KaratePlayerController[2];
                karatePlayers[0] = GameObject.Find("Player1").GetComponent<KaratePlayerController>();
                karatePlayers[1] = GameObject.Find("Player2").GetComponent<KaratePlayerController>();

                karateReferee = GameObject.Find("Referee").GetComponent<KarateRefereeController>();
                karateReferee.SetOlympicsController(this);
            }
            else if (currentScene == "Wrestling")
            {
                wrestlingPlayers = new Wrestler[2];
                wrestlingPlayers[0] = GameObject.Find("Player1").GetComponent<Wrestler>();
                wrestlingPlayers[1] = GameObject.Find("Player2").GetComponent<Wrestler>();

                wrestlingReferee = GameObject.Find("Referee").GetComponent<WrestlingReferee>();
                wrestlingReferee.SetOlympicsController(this);
            }
            else if (currentScene == "ShotPut")
            {
                shotPutPlayer = GameObject.Find("Player").GetComponent<ShotPutPlayer>();
                shotPutPlayer.SetOlympicsController(this);
            }
            else if (currentScene == "SwimmingFreestyle")
            {
                swimmingFreestylePlayers = new SwimmingFreestylePlayer[4];
                swimmingFreestylePlayers[0] = GameObject.Find("Player1").GetComponent<SwimmingFreestylePlayer>();
                swimmingFreestylePlayers[1] = GameObject.Find("Player2").GetComponent<SwimmingFreestylePlayer>();
                swimmingFreestylePlayers[2] = GameObject.Find("Player3").GetComponent<SwimmingFreestylePlayer>();
                swimmingFreestylePlayers[3] = GameObject.Find("Player4").GetComponent<SwimmingFreestylePlayer>();

                foreach (SwimmingFreestylePlayer player in swimmingFreestylePlayers)
                {
                    player.SetOlympicsController(this);
                }
            }
        }
    }

    private bool CheckIfShouldBeDestroyed()
    {
        if (GameObject.FindObjectsOfType<OlympicsController>().Length == 1 || firstInstance)
        {
            firstInstance = true;
            DontDestroyOnLoad(this.gameObject);
            return false;
        }
        else
        {
            Debug.Log("Destroying OlympicsController.");
            Destroy(gameObject);
            return true;
        }
    }

    public void DebugLogPlayers()
    {
        string displayString = "Players:   ";
        for (int i = 0; i < players.Length; i++)
        {
            displayString += players[i].playerName;

            if (players[i].isAI)
            {
                displayString += " [AI]";
            }

            if (i < players.Length - 1)
            {
                displayString += ", ";
            }
        }

        Debug.Log(displayString);
    }

    public void DebugLogEvents()
    {
        string displayString = "Events:   ";
        for (int i = 0; i < events.Length; i++)
        {
            displayString += events[i];
            if (i < events.Length - 1)
            {
                displayString += ", ";
            }
        }

        Debug.Log(displayString);
    }

    public void EndEvent()
    {
        GiveMedals();

        //Debug.Log("Moving to medals screen");
        Debug.Log("Moving to event medals screen");

        //LoadMedalScreen();
        LoadEventMedalScreen();
    }

    public void LoadEventMedalScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("EventMedals");
    }

    public void EndEventsMedalScreen()
    {
        Debug.Log("Moving to medals screen");

        LoadMedalScreen();
    }

    public void NextEvent()
    {
        eventNum++;

        if (eventNum >= events.Length)
        {
            Debug.Log("Moving to awards ceremony");

            eventNum = -10;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Debug.Log("Moving to next event: " + currentEvent);

            ResetPlayerScores();
            ResetSelectedPlayers();

            /*if (currentEvent == "Triple Jump")
            {
                players.Shuffle();
                SceneManager.LoadScene("Scoreboard");
            }
            else
            {
                throw new System.Exception("Unimplemented Olympics Event: " + currentEvent);
            }*/

            SceneManager.LoadScene("Scoreboard");
        }
    }

    private void ResetPlayerScores()
    {
        foreach(OlympicsPlayer player in players)
        {
            player.ResetScores();
        }
    }

    private void LoadMedalScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Medals");
    }

    public void SelectPlayerForEvent(int playerID)
    {
        if (OlympicsConfig.GetNumOfPlayers(currentEvent) == 1)
        {
            currentPlayerIDs = new List<int>(new int[] { playerID });

            StartEvent();
        }
        else
        {
            if (currentPlayerIDs.Contains(playerID))
            {
                currentPlayerIDs.Remove(playerID);
            }
            else
            {
                currentPlayerIDs.Add(playerID);
            }

            scoreboardController.UpdateSelectedPlayers(currentPlayerIDs.ToArray());
        }
    }

    private void AssignPlayerIDs()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetPlayerID(i);
        }
    }

    public OlympicsPlayer GetPlayerWithID(int playerID)
    {
        foreach(OlympicsPlayer player in players)
        {
            if (player.playerID == playerID)
            {
                return player;
            }
        }
        return null;
    }

    public void RecordScore(float score, int playerID = -1)
    {
        if (playerID == -1)
        {
            OlympicsPlayer player = GetPlayerWithID(currentPlayerIDs[0]);
            player.AddScore(score);
        }
        else
        {
            OlympicsPlayer player = GetPlayerWithID(playerID);
            player.AddScore(score);
        }

        receivedScoreCount++;

        if (receivedScoreCount == OlympicsConfig.GetNumOfPlayers(currentEvent))
        {
            receivedScores = true;

            if (OlympicsConfig.GetNumOfPlayers(currentEvent) > 1)
            {
                foreach (OlympicsPlayer player in players)
                {
                    if (!currentPlayerIDs.Contains(player.playerID))
                    {
                        player.AddScore(OlympicsConfig.NotParticipateValue(OlympicsConfig.IsHigherBetter(currentEvent)));
                    }
                }
            }
        }
    }

    public void RecordScores(float[] scores)
    {
        if (scores.Length != currentPlayerIDs.Count)
        {
            throw new System.Exception("Number of scores does not match number of players. Score Count: " + scores.Length + " Player Count: " + currentPlayerIDs.Count);
        }

        for (int i = 0; i < scores.Length; i++)
        {
            RecordScore(scores[i], currentPlayerIDs[i]);
        }
    }

    private void GiveMedals()
    {
        /*List<float> scoresNoDuplicates = new List<float>();
        foreach(OlympicsPlayer player in players)
        {
            float score = player.GetBestScore(IsHigherBetter(currentEvent));

            if (!scoresNoDuplicates.Contains(score))
            {
                scoresNoDuplicates.Add(score);
            }
        }

        if (IsHigherBetter(currentEvent))
        {
            scoresNoDuplicates = scoresNoDuplicates.OrderByDescending(i => i).ToList();
        }
        else
        {
            scoresNoDuplicates = scoresNoDuplicates.OrderBy(i => i).ToList();
        }

        string displayString = "";
        foreach (float i in scoresNoDuplicates)
        {
            displayString += i.ToString() + " ";
        }
        Debug.Log("scoresNoDuplicates: " + displayString);

        foreach (OlympicsPlayer player in players)
        {
            float score = player.GetBestScore(IsHigherBetter(currentEvent));

            if (score == scoresNoDuplicates[0])
            {
                player.GiveMedal(Medal.gold, currentEvent);
            }
            else if (score == scoresNoDuplicates[1])
            {
                player.GiveMedal(Medal.silver, currentEvent);
            }
            else if (score == scoresNoDuplicates[2])
            {
                player.GiveMedal(Medal.bronze, currentEvent);
            }
            else
            {
                player.GiveMedal(Medal.none, currentEvent);
            }
        }*/

        float[] scores = GetPlayerCurrentEventScores();

        float[] scoresNoDuplicates = Functions.RemoveDuplicates(scores);

        bool higherIsBetter = OlympicsConfig.IsHigherBetter(currentEvent);

        if (higherIsBetter)
        {
            scoresNoDuplicates = scoresNoDuplicates.ToList().OrderByDescending(i => i).ToArray();
        }
        else
        {
            scoresNoDuplicates = scoresNoDuplicates.ToList().OrderBy(i => i).ToArray();
        }
        
        int rankOffset = 0;

        /// i - the current rankwe are looking at (e.g. 1st place, 2nd place...)
        /// j - goes through each player to see if their score matches the score for the current rank
        /// rankOffset - used to make the rank number jump if we have ties (e.g., 1st, 2nd, 2nd, 4th instead of 1st, 2nd, 2nd, 3rd)
        for (int i = 0; i < scoresNoDuplicates.Length; i++)
        {
            int medalsForThisRank = 0;
            for (int j = 0; j < players.Length; j++)
            {
                if (scores[j] == scoresNoDuplicates[i])
                {
                    int rank;
                    if (medalRankingRule == MedalRankingRule.jumpWhenTie)
                    {
                        rank = i + 1 + rankOffset;
                    }
                    else
                    {
                        rank = i + 1;
                    }

                    players[j].GiveMedal((Medal)Functions.RoundToRange(rank - 1, 0, 3), rank, currentEvent);

                    medalsForThisRank++;
                }
            }
            rankOffset += medalsForThisRank - 1;
        }
    }

    public string[] GetPlayerNames()
    {
        List<string> names = new List<string>();

        foreach(OlympicsPlayer player in players)
        {
            names.Add(player.playerName);
        }

        return names.ToArray();
    }

    public float[] GetPlayerCurrentEventScores()
    {
        List<float> playerScores = new List<float>();

        foreach (OlympicsPlayer player in players)
        {
            //playerScores.Add(player.GetBestScore(OlympicsConfig.IsHigherBetter(currentEvent)));
            playerScores.Add(player.GetBestScore(currentEvent));
        }

        return playerScores.ToArray();
    }

    public void StartEvent()
    {
        receivedScores = false;
        receivedScoreCount = 0;
        inEvent = true;

        for (int i = 0; i < currentPlayerIDs.Count; i++)
        {
            OlympicsPlayer player = GetPlayerWithID(currentPlayerIDs[i]);

            playerSettings.SetPlayerID(i, player.playerID);
            playerSettings.SetPlayerName(i, player.playerName);
            playerSettings.SetPlayerAI(i, player.isAI);
            playerSettings.SetPlayerDifficulty(i, player.difficulty);
        }

        LoadEventScene(currentEvent);
    }

    private void LoadEventScene(string eventName)
    {
        switch (eventName)
        {
            case "100m": SceneManager.LoadScene("100Sprint"); break;
            case "200m": SceneManager.LoadScene("200Metres"); break;
            case "400m": SceneManager.LoadScene("400Metres"); break;
            case "Hurdles": SceneManager.LoadScene("100Hurdles"); break;
            case "Rowing": SceneManager.LoadScene("Rowing"); break;
            case "Javelin": SceneManager.LoadScene("Javelin"); break;
            case "Hammer": SceneManager.LoadScene("Hammer"); break;
            case "Long Jump": SceneManager.LoadScene("LongJump"); break;
            case "Triple Jump": SceneManager.LoadScene("TripleJump"); break;
            case "High Jump": SceneManager.LoadScene("HighJump"); break;
            case "Pole Vault": SceneManager.LoadScene("PoleVault"); break;
            case "Weightlifting": SceneManager.LoadScene("Weightlifting"); break;
            case "Archery": SceneManager.LoadScene("Archery"); break;
            case "Skeet": SceneManager.LoadScene("Skeet"); break;
            case "Speed Climbing": SceneManager.LoadScene("SpeedClimbing"); break;
            case "Ski Jump": SceneManager.LoadScene("SkiJump"); break;
            case "Karate": SceneManager.LoadScene("Karate"); break;
            case "Wrestling": SceneManager.LoadScene("Wrestling"); break;
            case "Shot Put": SceneManager.LoadScene("ShotPut"); break;
            case "100m Freestyle": SceneManager.LoadScene("SwimmingFreestyle"); break;
            default: throw new System.Exception("Unimplemented event: " + eventName);
        }
    }

    public void ResetSelectedPlayers()
    {
        currentPlayerIDs = new List<int>();
    }
}


