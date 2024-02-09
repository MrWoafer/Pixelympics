using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeachVolleyballGameController : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode resetButton = KeyCode.Space;

    [Header("References")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject ballPrefab;
    public GameObject ballConfig;

    private int roundNum = 0;

    public void Start()
    {
        roundNum = 1;
    }

    public void Update()
    {
        if (Input.GetKeyDown(resetButton))
        {
            ResetGame();
        }
    }

    public void ResetGame()
    {
        roundNum++;

        player1.transform.position = new Vector3(-8, -3, 0);
        player2.transform.position = new Vector3(-4, -3, 0);
        player3.transform.position = new Vector3(4, -3, 0);
        player4.transform.position = new Vector3(8, -3, 0);

        Destroy(GameObject.Find("Ball"));
        Destroy(GameObject.Find("Ball(Clone)"));
        GameObject ball = Instantiate(ballPrefab);
        ball.GetComponent<BallController>().configObj = ballConfig;
        ball.GetComponent<BallController>().UpdateConfigReference();
        if (roundNum % 2 == 1)
        {
            player1.GetComponent<PlayerController>().isServing = true;
            player4.GetComponent<PlayerController>().isServing = false;
            ball.transform.position = new Vector3(-7.35f, -2.83f, 0f);
            ball.GetComponent<BallController>().serverObj = player1;
        }
        else
        {
            player4.GetComponent<PlayerController>().isServing = true;
            player1.GetComponent<PlayerController>().isServing = false;
            ball.transform.position = new Vector3(7.35f, -2.83f, 0f);
            ball.GetComponent<BallController>().SetDirection(-1);
            ball.GetComponent<BallController>().serverObj = player4;
        }

        player1.GetComponent<PlayerController>().ballObj = ball;
        player2.GetComponent<PlayerController>().ballObj = ball;
        player3.GetComponent<PlayerController>().ballObj = ball;
        player4.GetComponent<PlayerController>().ballObj = ball;
        ballConfig.GetComponent<BeachVolleyballConfig>().ballObj = ball;

        player1.GetComponent<PlayerController>().ResetPlayer();
        player2.GetComponent<PlayerController>().ResetPlayer();
        player3.GetComponent<PlayerController>().ResetPlayer();
        player4.GetComponent<PlayerController>().ResetPlayer();
        ballConfig.GetComponent<BeachVolleyballConfig>().ResetConfig();
    }
}
