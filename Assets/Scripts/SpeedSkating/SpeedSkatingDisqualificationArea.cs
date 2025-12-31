using UnityEngine;

public class SpeedSkatingDisqualificationArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpeedSkatingPlayer player = collision.GetComponent<SpeedSkatingPlayer>();
        if (player is null)
        {
            return;
        }

        Debug.Log($"Player {player.playerNum} disqualified!");
    }
}
