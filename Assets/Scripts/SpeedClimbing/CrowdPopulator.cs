using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdPopulator : MonoBehaviour
{
    [Header("Settings")]
    public float xSpacing = 2f;
    public float xScalar = 0.1f;
    public float ySpacing = 1f;
    public float zSpacing = 1f;
    public int numPerRow = 20;
    public int numOfRows = 4;

    [Header("Colours")]
    public SkinColours skinColours;
    public TopColours topColours;
    public StripeColours stripeColours;

    [Header("References")]
    public GameObject crowdPersonPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int row = 0; row < numOfRows; row++)
        {
            for (int person = 0; person < numPerRow; person++)
            {
                GameObject crowdPerson = Instantiate(crowdPersonPrefab, transform.position + new Vector3(
                    (person - numPerRow / 2) * xSpacing + (row % 2 == 0 ? xSpacing / 2f : 0f) + (person - numPerRow / 2) * (person - numPerRow / 2 + 1) / 2f * xScalar,
                    ySpacing * row,
                    zSpacing * row),
                    Quaternion.identity);

                SpriteRenderer spr = crowdPerson.GetComponent<SpriteRenderer>();

                Color skinColour = skinColours.colours[Random.Range(0, skinColours.colours.Length)];

                int index = Random.Range(0, topColours.colours.Length);
                Color topColour = topColours.colours[index];
                Color stripeColour = stripeColours.colours[index];
                
                spr.material.SetColor("_SkinColour", skinColour);
                spr.material.SetColor("_TopColour", topColour);
                spr.material.SetColor("_StripeColour", stripeColour);

                crowdPerson.transform.parent = transform;
            }
        }
    }
}
