using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OlympicsPlayer
{
    public int playerID = -1;
    public string playerName;
    public bool isAI;
    public Difficulty difficulty = Difficulty.Hard;

    public int[] medals = new int[] { 0, 0, 0, 0 };
    public int golds
    {
        get
        {
            return medals[0];
        }
    }
    public int silvers
    {
        get
        {
            return medals[1];
        }
    }
    public int bronzes
    {
        get
        {
            return medals[2];
        }
    }
    public int nones
    {
        get
        {
            return medals[3];
        }
    }

    public List<string>[] medalsFor;
    public List<string> goldsFor
    {
        get
        {
            return medalsFor[0];
        }
    }
    public List<string> silversFor
    {
        get
        {
            return medalsFor[1];
        }
    }
    public List<string> bronzesFor
    {
        get
        {
            return medalsFor[2];
        }
    }
    public List<string> nonesFor
    {
        get
        {
            return medalsFor[3];
        }
    }

    public Medal latestMedal = Medal.none;
    public int latestRank = 0;

    public List<float> currentEventScores = new List<float>();

    public int totalPoints
    {
        get
        {
            return GetTotalPoints();
        }
    }

    public OlympicsPlayer(string _playerName)
    {
        playerName = _playerName;
        isAI = true;
        medals = new int[] { 0, 0, 0, 0 };
        ResetMedalsFor();
    }

    public OlympicsPlayer(string _playerName, bool _isAI)
    {
        playerName = _playerName;
        isAI = _isAI;
        medals = new int[] { 0, 0, 0, 0 };
        ResetMedalsFor();
    }

    public OlympicsPlayer(string _playerName, int _golds, int _silvers, int _bronzes, int _nones = 0)
    {
        playerName = _playerName;
        isAI = true;
        medals = new int[] { _golds, _silvers, _bronzes, _nones };
        ResetMedalsFor();
    }

    public OlympicsPlayer(string _playerName, bool _isAI, int _golds, int _silvers, int _bronzes, int _nones = 0)
    {
        playerName = _playerName;
        isAI = _isAI;
        medals = new int[] { _golds, _silvers, _bronzes, _nones };
        ResetMedalsFor();
    }

    public int GetTotalPoints()
    {
        return golds * 3 + silvers * 2 + bronzes * 1;
    }

    public void ResetScores()
    {
        currentEventScores = new List<float>();
    }

    public void AddScore(float score)
    {
        currentEventScores.Add(score);
    }

    public float GetBestScore(bool higherIsBetter)
    {
        if (higherIsBetter)
        {
            if (currentEventScores.Count <= 0)
            {
                return float.MinValue;
            }
            else
            {
                return currentEventScores.Max();
            }
        }
        else
        {
            if (currentEventScores.Count <= 0)
            {
                return float.MaxValue;
            }
            else
            {
                return currentEventScores.Min();
            }
        }
    }

    public float GetBestScore(string eventName)
    {
        if (eventName == "Karate")
        {
            if (currentEventScores.CountOccurrences(OlympicsConfig.NotParticipateValue(true)) == currentEventScores.Count)
            {
                return OlympicsConfig.NotParticipateValue(true);
            }
            return currentEventScores.CountOccurrences(2f);
        }
        else if (eventName == "Wrestling")
        {
            if (currentEventScores.CountOccurrences(OlympicsConfig.NotParticipateValue(true)) == currentEventScores.Count)
            {
                return OlympicsConfig.NotParticipateValue(true);
            }
            return currentEventScores.CountOccurrences(1f);
        }
        else
        {
            return GetBestScore(OlympicsConfig.IsHigherBetter(eventName));
        }
    }

    public void GiveMedal(Medal medal, int rank, string eventFor)
    {
        latestRank = rank;
        switch (medal)
        {
            case Medal.gold: medals[0]++; medalsFor[0].Add(eventFor); latestMedal = Medal.gold; break;
            case Medal.silver: medals[1]++; medalsFor[1].Add(eventFor); latestMedal = Medal.silver; break;
            case Medal.bronze: medals[2]++; medalsFor[2].Add(eventFor); latestMedal = Medal.bronze; break;
            case Medal.none: medals[3]++; medalsFor[3].Add(eventFor); latestMedal = Medal.none; break;
            default: throw new System.Exception("Unknown medal type: " + medal.ToString());
        }
    }

    public void ResetMedalsFor()
    {
        medalsFor = new List<string>[4] { new List<string>(), new List<string>(), new List<string>(), new List<string>() };
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }
}

public enum Medal
{
    gold,
    silver,
    bronze,
    none
}
