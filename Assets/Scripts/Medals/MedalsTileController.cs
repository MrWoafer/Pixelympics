using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedalsTileController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Image playerBox;
    [SerializeField]
    private Text playerText;
    [SerializeField]
    private Text goldText;
    [SerializeField]
    private Text silverText;
    [SerializeField]
    private Text bronzeText;
    [SerializeField]
    private Text totalText;
    [HideInInspector]
    public MedalController medalController;

    public int rowNum;

    private float hoverTime = 0f;
    private bool hovering = false;
    private bool shownTooltip = false;
    private int columnHoverNum;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hovering)
        {
            hoverTime += Time.deltaTime;

            if (hoverTime >= medalController.tooltipWaitTime && !shownTooltip)
            {
                shownTooltip = true;
                medalController.ShowTooltip(rowNum, columnHoverNum);
            }
        }
    }

    public void SetPlayerName(string playerName)
    {
        playerText.text = playerName;
    }

    public void SetMedalValues(int _golds, int _silvers, int _bronzes, int _total)
    {
        goldText.text = _golds.ToString();
        silverText.text = _silvers.ToString();
        bronzeText.text = _bronzes.ToString();

        totalText.text = _total.ToString();
    }

    public void SetDetails(string playerName, int _golds, int _silvers, int _bronzes, int _total)
    {
        SetPlayerName(playerName);
        SetMedalValues(_golds, _silvers, _bronzes, _total);
    }

    public void SetDetails(OlympicsPlayer player)
    {
        SetDetails(player.playerName, player.golds, player.silvers, player.bronzes, player.totalPoints);
    }

    public void SetRowNum(int _rowNum)
    {
        rowNum = _rowNum;
    }

    public void MouseEnter(int columnNum)
    {
        //Debug.Log("Mouse enter column " + columnNum);

        columnHoverNum = columnNum;
        hovering = true;
        shownTooltip = false;
    }

    public void MouseExit()
    {
        //Debug.Log("Mouse exit");

        hovering = false;
        hoverTime = 0f;
        shownTooltip = false;

        medalController.HideTooltip();
    }

    public float GetColumnXCoord(int columnNum)
    {
        switch (columnNum)
        {
            case 0: return goldText.transform.position.x;
            case 1: return silverText.transform.position.x;
            case 2: return bronzeText.transform.position.x;
            default: throw new System.Exception("Unknown column num: " + columnNum);
        }
    }

    public float GetYCoord()
    {
        return goldText.transform.position.y;
    }

    public void SetColour(Color colour)
    {
        playerBox.color = colour;
    }
}
