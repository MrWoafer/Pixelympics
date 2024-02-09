using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcheryArrowController : MonoBehaviour
{
    [Header("Sounds")]
    public AudioSource thud;

    [Header("References")]
    public ParticleSystem sparkParticles;
    private Rigidbody rb;
    private GameObject player;
    //public GameObject sphere;
    private ArcheryConfig config;

    private bool fired = false;
    private bool landed = false;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fired && !landed)
        {
            transform.eulerAngles = new Vector3(Mathf.Rad2Deg * Mathf.Atan(-rb.velocity.y / Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z)), transform.eulerAngles.y, transform.eulerAngles.z);
        }
        if (Mathf.Sqrt((transform.position.x - player.transform.position.x) * (transform.position.x - player.transform.position.x) + (transform.position.y - player.transform.position.y) * (transform.position.y - player.transform.position.y)
            + (transform.position.z - player.transform.position.z) * (transform.position.z - player.transform.position.z)) > 500f)
        {
            player.GetComponent<ArcheryPlayerController>().ArrowHit(-1f);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!landed)
        {
            landed = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;

            //ParticleSystem sparks = Instantiate(sparkParticles, transform.position, Quaternion.LookRotation(-transform.forward));
            ParticleSystem sparks = Instantiate(sparkParticles, GetTipCoord(), Quaternion.LookRotation(-transform.forward));
            sparks.Play();
            Destroy(sparks.gameObject, 2f);

            //Instantiate(sphere, GetTipCoord(), Quaternion.identity);

            if (collision.transform.root.tag == "Target")
            {
                thud.Play();
                float distance = collision.transform.root.gameObject.GetComponent<ArcheryTargetController>().GetDistance(GetTipCoord());
                //Debug.Log("Distance: " + distance);
                player.GetComponent<ArcheryPlayerController>().ArrowHit(distance);
            }
            else
            {
                player.GetComponent<ArcheryPlayerController>().ArrowHit(-1f);
            }

            config.LerpChangeWind(1f);
        }
    }

    public void Fire(float speed, GameObject shotByPlayer, ArcheryConfig configReference)
    {
        rb.velocity = transform.forward * speed;
        fired = true;

        player = shotByPlayer;
        config = configReference;
    }

    private Vector3 GetTipCoord()
    {
        float length = 8.9f * transform.localScale.z;
        return new Vector3(
            Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.y) * length * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.x),
            -length * Mathf.Sin(Mathf.Deg2Rad * transform.eulerAngles.x),
            Mathf.Cos(-Mathf.Deg2Rad * transform.eulerAngles.y) * length * Mathf.Cos(Mathf.Deg2Rad * transform.eulerAngles.x)
            ) + transform.position;
    }

    public void FixedUpdate()
    {
        if (fired && !landed)
        {
            transform.position += config.GetWindDirection() * config.windSpeed * Time.fixedDeltaTime;
        }
    }
}
