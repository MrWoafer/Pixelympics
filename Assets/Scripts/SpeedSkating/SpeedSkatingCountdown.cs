using System;

using UnityEngine;
using UnityEngine.Events;

public class SpeedSkatingCountdown : MonoBehaviour
{
    public enum State
    {
        WaitingToStart,
        FalseStart,
        Marks,
        Set,
        Go,
    }

    [Header("Settings")]
    [Min(0f)]
    public double marksDuration = 1f;
    [Min(0f)]
    public double setDurationMin = 1f;
    [Min(0f)]
    public double setDurationMax = 1f;
    [Min(0f)]
    public double goTextDuration = 1f;

    private Stopwatch stopwatch;
    private CentreText text;

    public State state { get; private set; } = State.WaitingToStart;
    private double setDuration;

    private double marksToSetTime => marksDuration;
    private double setToGoTime => marksToSetTime + setDuration;
    private double goTextDisappearTime => setToGoTime + goTextDuration;

    public TimeSpan timeUntilGo => TimeSpan.FromSeconds(setToGoTime) - stopwatch.elapsed;

    public UnityEvent onGo { get; private set; } = new();

    private void Awake()
    {
        text = GetComponentInChildren<CentreText>();
        
        stopwatch = new Stopwatch();

        if (setDurationMax < setDurationMin)
        {
            throw new ArgumentOutOfRangeException($"{nameof(setDurationMax)} must be >= {nameof(setDurationMin)}.");
        }
    }

    private void Start()
    {
        ResetCountdown();
    }

    private void Update()
    {
        if (state == State.Marks && stopwatch.elapsed.TotalSeconds >= marksDuration)
        {
            state = State.Set;
            text.text = "Set";
        }
        else if (state == State.Set && stopwatch.elapsed.TotalSeconds >= setToGoTime)
        {
            state = State.Go;
            text.text = "Go!";
            onGo.Invoke();
        }
        else if (state == State.Go && text.text != "" && stopwatch.elapsed.TotalSeconds >= goTextDisappearTime)
        {
            text.text = "";
        }
    }

    public void StartCountdown()
    {
        setDuration = UnityEngine.Random.Range((float)setDurationMin, (float)setDurationMax);

        state = State.Marks;
        stopwatch.Start();
        text.text = "Marks";
    }

    public void ResetCountdown()
    {
        state = State.WaitingToStart;
        stopwatch.Reset();
        text.text = "";
    }

    public void FalseStart()
    {
        state = State.FalseStart;
        text.text = "False Start!";
    }
}
