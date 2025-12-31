using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SpeedSkatingLapCounter : MonoBehaviour
{
    Dictionary<SpeedSkatingPlayer, int> laps = new Dictionary<SpeedSkatingPlayer, int>();

    private void Start()
    {
        foreach (SpeedSkatingPlayer player in GameObject.FindObjectsByType<SpeedSkatingPlayer>(FindObjectsSortMode.None))
        {
            laps.Add(player, 0);
        }
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
        }
        else if (collision.attachedRigidbody.linearVelocity.x < 0f)
        {
            laps[player]--;
        }
    }

    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.black;

        string text = "";
        foreach (SpeedSkatingPlayer player in laps.Keys.OrderBy(p => p.playerNum))
        {
            text = text + $"{player.playerNum}: {laps[player]}\n";
        }

        GUI.Label(
            new Rect(400, 10, 250, 20),
            text,
            guiStyle
        );
    }
}
