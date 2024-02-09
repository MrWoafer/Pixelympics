using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KeirinGameController : MonoBehaviour
{
    [Header("References")]
    public GameObject configObj;
    private KeirinConfig config;
    public GameObject beatLightObj;
    private SpriteRenderer beatLight;
    public GameObject[] playerObjs;
    private KeirinPlayerController[] players;
    private int[] lastPlayerPresses;

    private float beatTime = 0f;
    private bool beatLightOn = false;
    private bool inBottomHalfOfTimer = false;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<KeirinConfig>();
        beatLight = beatLightObj.GetComponent<SpriteRenderer>();

        players = new KeirinPlayerController[playerObjs.Length];
        for (int i = 0; i < playerObjs.Length; i++)
        {
            players[i] = playerObjs[i].GetComponent<KeirinPlayerController>();
        }
        lastPlayerPresses = new int[players.Length];

        StartBeat();
    }

    // Update is called once per frame
    void Update()
    {
        beatTime -= Time.deltaTime;
        if (beatTime <= config.beatFrequency - config.beatLightDuration && beatLightOn)
        {
            BeatLightOff();
        }
        if (beatTime <= 0f)
        {
            Beat();
        }
        if (beatTime < config.beatFrequency / 2f && !inBottomHalfOfTimer)
        {
            HalfBeat();
        }
    }

    private void StartBeat()
    {
        beatTime = config.beatFrequency;
        BeatLightOff();
    }

    private void Beat()
    {
        HalfBeat();

        beatTime = config.beatFrequency;

        foreach (KeirinPlayerController i in players)
        {
            i.LoseSpeed();

            if (i.IsAI())
            {
                i.AIGainSpeed();
            }
        }

        BeatLightOn();
    }

    private void HalfBeat()
    {
        inBottomHalfOfTimer = !inBottomHalfOfTimer;
        for (int i = 0; i < lastPlayerPresses.Length; i++)
        {
            lastPlayerPresses[i]++;
        }
    }

    private void BeatLightOn()
    {
        beatLightOn = true;
        beatLight.color = config.beatLightOnColour;
    }
    private void BeatLightOff()
    {
        beatLightOn = false;
        beatLight.color = config.beatLightOffColour;
    }

    public float GetSpeedGain(int playerNum)
    {
        if (lastPlayerPresses[playerNum] >= 2)
        {
            lastPlayerPresses[playerNum] = 0;
            return config.maxSpeedGain * 2f * Mathf.Abs(beatTime - config.beatFrequency / 2f) / config.beatFrequency;
        }
        else if (lastPlayerPresses[playerNum] == 1 && inBottomHalfOfTimer)
        {
            lastPlayerPresses[playerNum] = 0;
            return config.maxSpeedGain * 2f * Mathf.Abs(beatTime - config.beatFrequency / 2f) / config.beatFrequency;
        }
        else
        {
            //Debug.Log("Penalty!");
            return -1f;
        }
    }

    public KeirinPlayerController[] GetPlayers()
    {
        return players;
    }
    public GameObject[] GetPlayerObjs()
    {
        return playerObjs;
    }

    public bool IsLaneFree(int lane, float distanceRoundTrack)
    {
        if (lane < 0 || lane > 3)
        {
            return false;
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetLane() == lane)
            {
                if (Mathf.Min(Mathf.Abs(distanceRoundTrack - players[i].GetDistanceOnTrack()), config.TrackLength(lane) - Mathf.Abs(distanceRoundTrack - players[i].GetDistanceOnTrack())) < config.aiMaxLaneChangeDistance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public int Ranking(int playerNum)
    {
        KeirinPlayerController[] playersRanked = players.OrderBy(x => x.GetDistanceOnTrack(0) + x.GetLaps() * config.TrackLength(0)).ToArray<KeirinPlayerController>();

        for (int i = 0; i < playersRanked.Length; i++)
        {
            if (playersRanked[i].playerNum == playerNum)
            {
                return playersRanked.Length - i;
            }
        }

        return -1;
    }
}
