using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMainMenu : MonoBehaviour
{
    [Header("Settings")]
    public float verticalSpeed = 10f;
    public int screenNumMax = 2;
    public int screenNumMin = -2;

    [Header("References")]
    public GameObject everything;
    public GameObject title;
    public GameObject title2;

    private Animator anim;

    public GameObject screen4;
    public GameObject screen6;

    private int screenNum = 1;
    private bool isTyping = false;
    private float screenShift = 100f;

    private List<int> stillCameraScenes = new List<int>(new int[] { -2, -1, 0 });

    // Start is called before the first frame update
    void Start()
    {
        everything.transform.position = new Vector3(0f, everything.transform.position.y, everything.transform.position.z);
        anim = GetComponent<Animator>();
        ChangeScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTyping)
        {
            if (Input.GetKeyDown("a") && screenNum > screenNumMin)
            {
                everything.transform.Translate(new Vector3(screenShift, 0f, 0f));
                screenNum -= 1;
                ChangeScreen();
            }
            if (Input.GetKeyDown("d") && screenNum < screenNumMax)
            {
                everything.transform.Translate(new Vector3(-screenShift, 0f, 0f));
                screenNum += 1;
                ChangeScreen();
            }
            if (Input.GetKey("s"))
            {
                if (screenNum == -1)
                {
                    screen4.transform.Translate(new Vector3(0f, verticalSpeed * Time.deltaTime, 0f));
                }
                if (screenNum == -2)
                {
                    screen6.transform.Translate(new Vector3(0f, verticalSpeed * Time.deltaTime, 0f));
                }
            }
            if (Input.GetKey("w"))
            {
                if (screenNum == -1)
                {
                    screen4.transform.Translate(new Vector3(0f, -verticalSpeed * Time.deltaTime, 0f));

                    if (screen4.transform.position.y < 1f)
                    {
                        screen4.transform.position = new Vector3(screen4.transform.position.x, 1f, screen4.transform.position.z);
                    }
                }
                if (screenNum == -2)
                {
                    screen6.transform.Translate(new Vector3(0f, -verticalSpeed * Time.deltaTime, 0f));

                    if (screen6.transform.position.y < 1f)
                    {
                        screen6.transform.position = new Vector3(screen6.transform.position.x, 1f, screen6.transform.position.z);
                    }
                }
            }
        }
    }
    
    private void ChangeScreen()
    {
        title.SetActive(false);
        title2.SetActive(false);

        if (screenNum == 1)
        {
            title.SetActive(true);
        }
        else if (screenNum == 2)
        {
            title2.SetActive(true);
        }
        /*
        if (screenNum != 4)
        {
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
        */
        ToggleAnimation();
    }

    public void Typing(bool typing)
    {
        isTyping = typing;
    }

    private void ToggleAnimation()
    {
        if (stillCameraScenes.Contains(screenNum))
        {
            anim.enabled = false;
            transform.position = new Vector3(0f, transform.position.y, transform.position.z);
            transform.rotation = Quaternion.identity;
        }
        else
        {
            anim.enabled = true;
        }
    }
}
