using System;

using UnityEngine;

public class SpeedSkatingRaceController : MonoBehaviour
{
    private enum State
    {
        WaitingToStart,
        Countdown,
        FalseStart,
        Race,
    }

    private SpeedSkatingConfig config;
    private SpeedSkatingCountdown countdown;
    private SpeedSkatingLapCounter lapCounter;
    private SpeedSkatingTrack track;
    private Stopwatch stopwatch;

    private State state = State.WaitingToStart;

    private SpeedSkatingPlayer[] players;

    private void Awake()
    {
        config = FindFirstObjectByType<SpeedSkatingConfig>();
        countdown = FindFirstObjectByType<SpeedSkatingCountdown>();
        lapCounter = FindFirstObjectByType<SpeedSkatingLapCounter>();
        track = FindFirstObjectByType<SpeedSkatingTrack>();

        stopwatch = new Stopwatch();

        players = GameObject.FindObjectsByType<SpeedSkatingPlayer>(FindObjectsSortMode.None);
        foreach (SpeedSkatingPlayer player in players)
        {
            player.onStart.AddListener(() => OnPlayerStart(player));
        }
    }

    private void Start()
    {
        countdown.onGo.AddListener(OnGo);
        lapCounter.onLapChanged.AddListener(OnLapChanged);

        ResetRace();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetRace();
        }
        if (Input.GetKeyDown(KeyCode.Space) && state == State.WaitingToStart)
        {
            StartRace();
        }
    }

    private void StartRace()
    {
        state = State.Countdown;
        countdown.StartCountdown();
    }

    private void ResetRace()
    {
        state = State.WaitingToStart;
        stopwatch.Reset();
        countdown.ResetCountdown();
        track.ResetTrack();
        lapCounter.ResetLaps();

        foreach (SpeedSkatingPlayer player in players)
        {
            player.ResetPlayer();
        }
    }

    private void OnGo()
    {
        state = State.Race;
        stopwatch.Start();
    }

    private void OnPlayerStart(SpeedSkatingPlayer player)
    {
        if (state == State.FalseStart)
        {
            return;
        }

        if (countdown.state == SpeedSkatingCountdown.State.Go)
        {
            Debug.Log($"{player.playerName} started {-countdown.timeUntilGo.TotalMilliseconds / 1000d:F3}s after Go.");
        }
        else
        {
            state = State.FalseStart;
            if (countdown.state == SpeedSkatingCountdown.State.WaitingToStart)
            {
                Debug.Log($"{player.playerName} had a false start! The countdown hadn't even started!");
            }
            else
            {
                Debug.Log($"{player.playerName} had a false start! They had {countdown.timeUntilGo.TotalMilliseconds / 1000d:F3}s left.");
            }
            countdown.FalseStart();
        }
    }

    private void OnLapChanged(SpeedSkatingPlayer player)
    {
        if (lapCounter.Laps(player) == config.numLaps)
        {
            Debug.Log($"{player.playerName} finished in {stopwatch.elapsed.ToString(@"mm\:ss\.ff")}");
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        TimeSpan time = stopwatch.started ? stopwatch.elapsed : TimeSpan.Zero;
        string text = time.ToString(@"mm\:ss\.ff");

        GUI.Label(
            new Rect(900, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
