using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeetConfig : MonoBehaviour
{
    [Header("Station Settings")]
    public int stationNum = 1;
    public bool pulled = false;
    public Vector3[] stationLocations;
    public int[] shotsPerStation;

    [Header("Player Settings")]
    public float keyboardHorizontalRotationSpeed = 50f;
    public float keyboardVerticalRotationSpeed = 30f;
    public float mouseHorizontalRotationSpeed = 50f;
    public float mouseVerticalRotationSpeed = 30f;
}
