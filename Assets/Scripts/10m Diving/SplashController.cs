using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    private float t = 0f;
    private bool splash = false;

    [Header("References")]
    public GameObject playerObj;
    private DivingController player;

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<DivingController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (t < 0f)
        {
            splash = false;
            transform.position = new Vector3(0f, -10f, 0f);
        }
    }

    public void FixedUpdate()
    {
        if (splash)
        {
            t -= Time.fixedDeltaTime;
        }
    }

    public void Splash(float angle)
    {
        splash = true;
        t = 1f;
        //transform.position = new Vector3(-0.1f, 2f, 3f);
        float scale = Functions.RoundToRange(angle / 10f + 1f, 1f, 4f);
        transform.localScale = new Vector3(scale, scale, 1f);
        //transform.position = new Vector3(-0.1f, scale / 2f, 3f);
        transform.position = new Vector3(DivingConfig.jumpSpeedR * player.GetTimeToFall() + DivingConfig.diverStartX, scale / 2f, 3f);
    }
}
