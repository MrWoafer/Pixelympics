using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreboardTileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image playerBox;
    [SerializeField]
    private Text playerText;
    [SerializeField]
    private Text[] scoreTexts;
    [SerializeField]
    private Text bestText;
    public Button playerButton;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPlayerName(string playerName)
    {
        playerText.text = playerName;
    }

    public void SetScoreValues(float[] scores, float bestScore, int resolution = 2, int scrollOffset = 0)
    {
        int i = 0;
        while (scrollOffset + i < scores.Length && i < scoreTexts.Length)
        {
            /// float.MinValue an float.MaxValue are used to denote a foul
            if (scores[scrollOffset + i] == float.MinValue || scores[scrollOffset + i] == float.MaxValue)
            {
                scoreTexts[i].text = "X";
            }
            /// OlympicsConfig.NotParticipateValue is used to denote not taking part in a race
            else if (scores[scrollOffset + i] == OlympicsConfig.NotParticipateValue(false) || scores[scrollOffset + i] == OlympicsConfig.NotParticipateValue(true))
            {
                scoreTexts[i].text = "--";
            }
            else
            {
                scoreTexts[i].text = scores[scrollOffset + i].ToString("n" + resolution.ToString());
            }
            i++;
        }

        if (i < scoreTexts.Length)
        {
            for (int j = i; j < scoreTexts.Length; j++)
            {
                scoreTexts[j].text = "--";
            }
        }

        if (OlympicsConfig.IsReservedScoreValue(bestScore))
        {
            bestText.text = "--";
        }
        else
        {
            bestText.text = bestScore.ToString("n" + resolution.ToString());
        }
    }

    public void SetDetails(string playerName, float[] scores, float bestScore, int resolution = 2, int scrollOffset = 0)
    {
        SetPlayerName(playerName);
        SetScoreValues(scores, bestScore, resolution: resolution, scrollOffset: scrollOffset);
    }

    public void SetDetails(OlympicsPlayer player, bool higherIsBetter, int resolution = 2, int scrollOffset = 0)
    {
        SetDetails(player.playerName, player.currentEventScores.ToArray(), player.GetBestScore(higherIsBetter), resolution: resolution, scrollOffset: scrollOffset);
    }

    public void SetColour(Color colour)
    {
        playerBox.color = colour;
    }
}
