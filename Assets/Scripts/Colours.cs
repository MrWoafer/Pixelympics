using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colours
{
    public static readonly Color blue = Functions.Color255(71f, 90f, 255f);
    public static readonly Color green = Functions.Color255(16f, 197f, 44f);
    public static readonly Color purple = Functions.Color255(197f, 16f, 151f);
    public static readonly Color yellow = Functions.Color255(255f, 230f, 0f);

    public static Color GetColour(int characterNum)
    {
        switch (characterNum)
        {
            case 0: return blue;
            case 1: return green;
            case 2: return purple;
            case 3: return yellow;
            default: throw new System.Exception("Unknown character num: " + characterNum);
        }
    }
}
