using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisBall : MonoBehaviour
{
    [Header("Ball Settings")]
    

    [Header("Shadow Settings")]
    public float sunHeight = 100f;

    [Header("References")]
    public GameObject shadow;
    private SpriteRenderer shadowSpr;
    private TennisConfig config;
    private SphereCollider collider;

    /// <summary>
    /// The time since the ball was last hit.
    /// </summary>
    private float t;
    /// <summary>
    /// The point from which the tennis ball was hit.
    /// </summary>
    private Vector3 hitPoint;
    /// <summary>
    /// Initial speed of the tennis ball.
    /// </summary>
    private Vector3 u;
    /// <summary>
    /// The current velocity of the tennis ball.
    /// </summary>
    private Vector3 vel;

    private bool bouncedHighEnough = true;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<TennisConfig>();

        shadowSpr = shadow.GetComponent<SpriteRenderer>();
        shadowSpr.sortingOrder = -10000;

        collider = GetComponent<SphereCollider>();

        //Hit(new Vector3(0f, 5f, 10f));
        //Hit(config.hitSpeed, config.hitAngleHorizontal, config.hitAngleVertical);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateShadow();
    }

    private void UpdateShadow()
    {
        shadow.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        shadow.transform.localScale = (new Vector3(2f, 2f, 2f)) * (sunHeight * transform.localScale.x / 2f) / (sunHeight - transform.position.y);
    }

    public void Hit(Vector3 velocity)
    {
        t = 0f;
        u = velocity;
        hitPoint = transform.position;
    }

    public void Hit(float speed, float angleH, float angleV)
    {
        Hit( speed * (new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleV) * Mathf.Sin(Mathf.Deg2Rad * angleH), Mathf.Sin(Mathf.Deg2Rad * angleV), Mathf.Cos(Mathf.Deg2Rad * angleV) * Mathf.Cos(Mathf.Deg2Rad * angleH))) );
    }

    private void UpdateMovement()
    {
        t += Time.deltaTime * config.ballSpeed;
        transform.position = new Vector3(hitPoint.x + u.x * t - 0.5f * config.drag * t * t, hitPoint.y + u.y * t - 0.5f * config.g * t * t, hitPoint.z + u.z * t - 0.5f * config.drag * t * t);
        vel = new Vector3(u.x - config.drag * t, u.y - config.g * t, u.z - config.drag * t);

        if (transform.position.y <= 0.147f && bouncedHighEnough)
        {
            bouncedHighEnough = false;
            Hit(new Vector3(vel.x * config.bounceHorizontalDampingFactor, -vel.y * config.bounceVerticalDampingFactor, vel.z * config.bounceHorizontalDampingFactor));
        }
        if (!bouncedHighEnough && transform.position.y > 0.147f)
        {
            bouncedHighEnough = true;
        }

        /*if (transform.position.z >= 10f)
        {
            Hit(config.hitSpeed, 180f + config.hitAngleHorizontal, config.hitAngleVertical);
        }*/

        if (transform.position.y < 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.DrawWireSphere(transform.TransformPoint(collider.center), collider.radius);
        }
        catch
        {

        }
    }
}
