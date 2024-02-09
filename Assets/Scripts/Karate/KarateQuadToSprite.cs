using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarateQuadToSprite : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    private SpriteRenderer sprRenderer;
    private Renderer ren;

    // Start is called before the first frame update
    void Start()
    {
        sprRenderer = player.GetComponent<SpriteRenderer>();
        ren = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ren.material.mainTexture = sprRenderer.material.mainTexture;
    }
}
