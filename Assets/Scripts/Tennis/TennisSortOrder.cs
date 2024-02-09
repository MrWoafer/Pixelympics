using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisSortOrder : MonoBehaviour
{
    private SpriteRenderer spr;

    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        spr.sortingOrder = (int)(-transform.position.z * 100f);
    }
}
