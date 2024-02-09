using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OlympicsConfig 
{
    private static readonly float foulValueHigherIsBetter = float.MinValue;
    private static readonly float foulValueHigherIsNotBetter = float.MaxValue;

    private static readonly float foulOrderingValueHigherIsBetter = float.MinValue / 4f;
    private static readonly float foulOrderingValueHigherIsNotBetter = float.MaxValue / 4f;

    private static readonly float didNotParticipateValueHigherIsBetter = float.MinValue / 2f;
    private static readonly float didNotParticipateValueHigherIsNotBetter = float.MaxValue / 2f;

    public static float FoulValue(bool higherIsBetter)
    {
        return higherIsBetter ? foulValueHigherIsBetter : foulValueHigherIsNotBetter;
    }
    public static float FoulValue(string eventName)
    {
        return FoulValue(IsHigherBetter(eventName));
    }

    public static float FoulOrderingValue(bool higherIsBetter)
    {
        return higherIsBetter ? foulOrderingValueHigherIsBetter : foulOrderingValueHigherIsNotBetter;
    }
    public static float FoulOrderingValue(string eventName)
    {
        return FoulOrderingValue(IsHigherBetter(eventName));
    }

    public static float NotParticipateValue(bool higherIsBetter)
    {
        return higherIsBetter ? didNotParticipateValueHigherIsBetter : didNotParticipateValueHigherIsNotBetter;
    }
    public static float NotParticipateValue(string eventName)
    {
        return NotParticipateValue(IsHigherBetter(eventName));
    }

    public static bool IsReservedScoreValue(float value)
    {
        return value == foulValueHigherIsBetter || value == foulValueHigherIsNotBetter || value == foulOrderingValueHigherIsBetter || value == foulOrderingValueHigherIsNotBetter ||
            value == didNotParticipateValueHigherIsBetter || value == didNotParticipateValueHigherIsNotBetter;
    }

    public static int GetNumOfPlayers(string eventName)
    {
        switch (eventName)
        {
            case "100m": return 4;
            case "200m": return 4;
            case "400m": return 4;
            case "Hurdles": return 4;
            case "Rowing": return 4;
            case "Javelin": return 1;
            case "Hammer": return 1;
            case "Long Jump": return 1;
            case "Triple Jump": return 1;
            case "High Jump": return 1;
            case "Pole Vault": return 1;
            case "Weightlifting": return 1;
            case "Archery": return 1;
            case "Skeet": return 1;
            case "Speed Climbing": return 2;
            case "Ski Jump": return 1;
            case "Karate": return 2;
            case "Wrestling": return 2;
            case "Shot Put": return 1;
            case "100m Freestyle": return 4;
            case "Alpine Skiing": return 4;
            default: throw new System.Exception("Unknown event: " + eventName);
        }
    }

    public static bool IsHigherBetter(string eventName)
    {
        switch (eventName)
        {
            case "100m": return false;
            case "200m": return false;
            case "400m": return false;
            case "Hurdles": return false;
            case "Rowing": return false;
            case "Javelin": return true;
            case "Hammer": return true;
            case "Long Jump": return true;
            case "Triple Jump": return true;
            case "High Jump": return true;
            case "Pole Vault": return true;
            case "Weightlifting": return true;
            case "Archery": return true;
            case "Skeet": return true;
            case "Speed Climbing": return false;
            case "Ski Jump": return true;
            case "Karate": return true;
            case "Wrestling": return true;
            case "Shot Put": return true;
            case "100m Freestyle": return false;
            case "Alpine Skiing": return false;
            default: throw new System.Exception("Unknown event: " + eventName);
        }
    }

    public static int GetResolution(string eventName)
    {
        switch (eventName)
        {
            case "100m": return 2;
            case "200m": return 2;
            case "400m": return 2;
            case "Hurdles": return 2;
            case "Rowing": return 2;
            case "Javelin": return 2;
            case "Hammer": return 2;
            case "Long Jump": return 2;
            case "Triple Jump": return 2;
            case "High Jump": return 0;
            case "Pole Vault": return 0;
            case "Weightlifting": return 0;
            case "Archery": return 0;
            case "Skeet": return 0;
            case "Speed Climbing": return 3;
            case "Ski Jump": return 2;
            case "Karate": return 0;
            case "Wrestling": return 0;
            case "Shot Put": return 2;
            case "100m Freestyle": return 2;
            case "Alpine Skiing": return 2;
            default: throw new System.Exception("Unknown event: " + eventName);
        }
    }

    public static float GetDefaultRecord(string eventName)
    {
        switch (eventName)
        {
            case "Shot Put": return 10f;
            case "100m Freestyle": return 60f;
            case "Alpine Skiing": return 120f;
            default: throw new System.Exception("Unknown event: " + eventName);
        }
    }
}
