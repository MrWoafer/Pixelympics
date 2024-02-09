using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisPlayer : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public float direction = 1f;
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";
    public string button6 = "q";

    [Header("References")]
    public GameObject spriteObj;
    private SpriteRenderer spr;
    public GameObject shadowObj;
    private SpriteRenderer sprShadow;
    private TennisConfig config;
    private Animator anim;

    private bool hitWindow = false;
    private int hitStage = 0;

    private bool right;
    private bool left;
    private bool up;
    private bool down;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<TennisConfig>();

        anim = GetComponent<Animator>();
        spr = spriteObj.GetComponent<SpriteRenderer>();
        sprShadow = shadowObj.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        right = false;
        left = false;
        up = false;
        down = false;
        if (Input.GetKey(button3))
        {
            up = true;
        }
        if (Input.GetKey(button4))
        {
            down = true;
        }
        if (Input.GetKey(button1))
        {
            left = true;
        }
        if (Input.GetKey(button2))
        {
            right = true;
        }

        if (!hitWindow)
        {
            Vector3 movementVector = Vector3.zero;
            if (up)
            {
                movementVector += new Vector3(0f, 0f, direction);
            }
            if (down)
            {
                movementVector += new Vector3(0f, 0f, -direction);
            }
            if (left)
            {
                movementVector += new Vector3(-direction, 0f, 0f);
            }
            if (right)
            {
                movementVector += new Vector3(direction, 0f, 0f);
            }
            if (Input.GetKeyDown(button5))
            {
                anim.SetTrigger("Hit");

                HitWindowOpen();
            }

            if (movementVector != Vector3.zero)
            {
                transform.position += movementVector / movementVector.magnitude * config.movementSpeed * Time.deltaTime;
            }
        }

        if (hitWindow)
        {
            Collider[] hits = Physics.OverlapSphere(GetHitboxCentre(), GetHitBoxRadius());

            foreach (Collider col in hits)
            {
                if (col.tag == "Ball")
                {
                    Debug.Log("Whack!");

                    TennisBall ball = col.GetComponent<TennisBall>();

                    float angleH;
                    if (right)
                    {
                        angleH = config.hitAnglesHorizontal[1];
                    }
                    else if (left)
                    {
                        angleH = -config.hitAnglesHorizontal[1];
                    }
                    else
                    {
                        angleH = config.hitAnglesHorizontal[0];
                    }

                    if (direction == -1)
                    {
                        angleH += 180f;
                    }

                    float angleV;
                    float speed;
                    if (down)
                    {
                        angleV = config.hitAnglesVertical[1];
                        speed = config.hitSpeeds[1];
                    }
                    else if (up)
                    {
                        angleV = config.hitAnglesVertical[2];
                        speed = config.hitSpeeds[2];
                    }
                    else
                    {
                        angleV = config.hitAnglesVertical[0];
                        speed = config.hitSpeeds[0];
                    }

                    ball.Hit(speed, angleH, angleV);
                }
            }
        }

        sprShadow.sprite = spr.sprite;
    }

    public void HitWindowOpen()
    {
        hitWindow = true;
        hitStage = 0;
    }
    public void HitWindowClosed()
    {
        hitWindow = false;
    }

    public void IncreaseHitStage()
    {
        hitStage++;
    }

    public void OnDrawGizmos()
    {
        if (hitWindow)
        {
            Gizmos.DrawWireSphere(GetHitboxCentre(), GetHitBoxRadius());
        }
    }

    public Vector3 GetHitboxCentre()
    {
        return transform.position + new Vector3(0.3f, 0f, 0f);
    }

    public float GetHitBoxRadius()
    {
        return 0.4f;
    }
}
