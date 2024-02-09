using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField]
    private float speed = 0.1f;
    private float stopPoint;
    [SerializeField]
    //private float fadeSpeed = 2f;
    //private float fadeSpeed = 0.02f;
    //private float fadeSpeed = 0.002f;
    private float fadeSpeed = 0.00002f;
    //private float opacity = 255f;
    private float opacity = 1f;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //stopPoint = Random.Range(-2.5f, -2.3f);
        stopPoint = Random.Range(-2.45f, -2.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > stopPoint)
        {
            transform.Translate(new Vector3(0f, -speed, 0f));
        }
        else if (opacity > 0f)
        {
            opacity -= fadeSpeed;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, opacity);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
