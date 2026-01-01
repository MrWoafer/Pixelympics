using System;

using UnityEngine;

public class Stopwatch
{
    private double? startTime = null;

    public bool started => startTime.HasValue;
    private double seconds => started ? Time.timeAsDouble - startTime.Value : throw new InvalidOperationException($"{nameof(Stopwatch)} must have been started in order to read the time.");
    public TimeSpan elapsed => TimeSpan.FromSeconds(seconds);

    public void Start()
    {
        if (started)
        {
            throw new InvalidOperationException($"{nameof(Stopwatch)} has already been started.");
        }

        startTime = Time.timeAsDouble;
    }

    public void Reset()
    {
        startTime = null;
    }
}