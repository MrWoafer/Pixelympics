using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeetController : MonoBehaviour
{
    [Header("Settings")]
    public float deathCountdown = 20f;

    [Header("References")]
    public GameObject skeetModel;
    public ParticleSystem breakParticles;

    private bool broken = false;
    private bool goneReasonablyHigh = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (broken)
        {
            deathCountdown -= Time.deltaTime;
            if (deathCountdown <= 0f)
            {
                Destroy(gameObject);
            }
        }*/

        if (!goneReasonablyHigh && transform.position.y > 2f)
        {
            goneReasonablyHigh = true;
        }
    }

    public void Break()
    {
        broken = true;
        breakParticles.Play();
        skeetModel.SetActive(false);
        Destroy(gameObject, deathCountdown);
    }

    public bool IsBroken()
    {
        return broken;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!broken && collision.gameObject.tag == "Ground" && goneReasonablyHigh)
        {
            Break();
        }
    }
}
