using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectInWater : MonoBehaviour
{
    [Header("Settings")]
    public bool useLocalYAsHeight = true;
    public float height = 0f;
    public bool useSpriteMask = true;

    [Header("References")]
    private SpriteRenderer spr;
    private GameObject reflection;
    private SpriteRenderer sprRef;
    private SpriteMask mask;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();

        reflection = new GameObject("Reflection");
        reflection.transform.parent = transform.parent;
        sprRef = reflection.AddComponent<SpriteRenderer>();

        sprRef.flipY = true;
        sprRef.sortingLayerName = "WaterReflection";
        sprRef.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        if (useSpriteMask)
        {
            sprRef.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            mask = (new GameObject("Reflection Mask")).AddComponent<SpriteMask>();
            mask.transform.parent = transform.parent;
            mask.transform.localScale = new Vector3(1f, 1f, 1f);

            Texture2D tex = new Texture2D(1, 1);
            tex.filterMode = FilterMode.Point;
            tex.SetPixel(0, 0, Color.white);
            mask.sprite = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector3(0.5f, 0.5f), 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        reflection.transform.localScale = transform.localScale;
        reflection.transform.localPosition = new Vector3(transform.localPosition.x, -GetHeight(), transform.localPosition.z);

        if (useSpriteMask)
        {
            mask.transform.localPosition = new Vector3(transform.localPosition.x, mask.transform.localScale.y / 2f, transform.localPosition.z);
        }

        sprRef.sprite = spr.sprite;
        sprRef.color = spr.color;
        sprRef.sortingOrder = spr.sortingOrder - 2;
    }

    private float GetHeight()
    {
        return useLocalYAsHeight ? transform.localPosition.y : height;
    }
}
