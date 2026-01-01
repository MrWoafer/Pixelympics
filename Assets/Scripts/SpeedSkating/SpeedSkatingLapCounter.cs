using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

public class SpeedSkatingLapCounter : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<SpeedSkatingPlayer> onLapChanged = new();

    private Dictionary<SpeedSkatingPlayer, int> laps = new Dictionary<SpeedSkatingPlayer, int>();
    
    private void Awake()
    {
        ResetLaps();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpeedSkatingPlayer player = collision.GetComponent<SpeedSkatingPlayer>();
        if (player is null)
        {
            return;
        }

        if (collision.attachedRigidbody.linearVelocity.x > 0f)
        {
            laps[player]++;
            onLapChanged.Invoke(player);
        }
        else if (collision.attachedRigidbody.linearVelocity.x < 0f)
        {
            laps[player]--;
            onLapChanged.Invoke(player);
        }
    }

    public int Laps(SpeedSkatingPlayer player) => laps[player];

    public void ResetLaps()
    {
        laps = new Dictionary<SpeedSkatingPlayer, int>();
        foreach (SpeedSkatingPlayer player in GameObject.FindObjectsByType<SpeedSkatingPlayer>(FindObjectsSortMode.None))
        {
            laps.Add(player, 0);
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = "Laps:\n";
        foreach (SpeedSkatingPlayer player in laps.Keys.OrderBy(p => p.playerNum))
        {
            text += $"{player.playerName}: {laps[player]}\n";
        }

        GUI.Label(
            new Rect(400, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
