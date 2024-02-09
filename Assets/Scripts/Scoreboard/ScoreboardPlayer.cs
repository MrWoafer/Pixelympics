using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreboardPlayer
{
    public int playerID = 0;
    public string playerName;
    public List<float> scores = new List<float>();

    /*public float bestScore
    {
        get
        {
            return GetBestScore(high);
        }
    }*/

    public ScoreboardPlayer(string _playerName)
    {
        playerName = _playerName;
        scores = new List<float>();
    }

    public ScoreboardPlayer(string _playerName, List<float> _scores)
    {
        playerName = _playerName;
        scores = _scores;
    }

    public ScoreboardPlayer(string _playerName, float[] _scores)
    {
        playerName = _playerName;
        scores = _scores.ToList();
    }

    public ScoreboardPlayer(int _playerID, string _playerName)
    {
        playerID = _playerID;
        playerName = _playerName;
        scores = new List<float>();
    }

    public ScoreboardPlayer(int _playerID, string _playerName, List<float> _scores)
    {
        playerID = _playerID;
        playerName = _playerName;
        scores = _scores;
    }

    public ScoreboardPlayer(int _playerID, string _playerName, float[] _scores)
    {
        playerID = _playerID;
        playerName = _playerName;
        scores = _scores.ToList();
    }

    public void AddScore(float score)
    {
        scores.Add(score);
    }

    public void ClearScores()
    {
        scores = new List<float>();
    }

    public float GetBestScore(bool higherIsBetter)
    {
        if (higherIsBetter)
        {
            if (scores.Count <= 0)
            {
                return float.MinValue;
            }
            else
            {
                return scores.Max();
            }

        }
        else if (scores.Count <= 0)
        {
            return float.MaxValue;
        }
        else
        {
            return scores.Min();
        }
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }
}
