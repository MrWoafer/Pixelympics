using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarateCrowdController : MonoBehaviour
{
    [Header("Settings")]
    public float xSpacing = 2f;
    public float xScalar = 0.1f;
    public float ySpacing = 1f;
    public float zSpacing = 1f;
    public int numPerRow = 20;
    public int numOfRows = 4;

    [Header("People Settings")]
    public float minIdleWaitTime = 0f;
    public float maxIdleWaitTime = 0.5f;
    public float minClapWaitTime = 0f;
    public float maxClapWaitTime = 0.5f;
    public float minShockWaitTime = 0f;
    public float maxShockWaitTime = 0.2f;
    public float shockChance = 0.1f;
    public float minCheerWaitTime = 0f;
    public float maxCheerWaitTime = 0.3f;
    public float cheerSFXChance = 0.1f;

    [Header("Sound Settings")]
    public int numOfClapSounds = 8;
    public float clapChance = 0.1f;
    public float shockSFXChance = 0.1f;

    [Header("Colours")]
    //public Color[] skinColours;
    public Color[] hairColours;
    //public Color[] topColours;
    //public Color[] stripeColours;
    public SkinColours skinColours;
    public TopColours topColours;
    public StripeColours stripeColours;

    [Header("References")]
    public GameObject crowdPrefab;
    public GameObject configObj;
    private KarateConfig config;
    private KarateCrowdPersonController[] crowd;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        config = configObj.GetComponent<KarateConfig>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        crowd = new KarateCrowdPersonController[numPerRow * numOfRows];

        for (int row = 0; row < numOfRows; row++)
        {
            for (int person = 0; person < numPerRow; person++)
            {
                GameObject crowdPerson = Instantiate(crowdPrefab, transform.position + new Vector3(
                    (person - numPerRow / 2) * xSpacing + (row % 2 == 0 ? xSpacing / 2f : 0f) + (person - numPerRow / 2) * (person - numPerRow / 2 + 1) / 2f * xScalar,
                    ySpacing * row, 
                    zSpacing * row),
                    Quaternion.identity);

                crowdPerson.GetComponent<KarateCrowdPersonController>().Create(GetComponent<KarateCrowdController>());

                crowd[row * numPerRow + person] = crowdPerson.GetComponent<KarateCrowdPersonController>();
            }
        }
    }

    public void Clap()
    {
        foreach (KarateCrowdPersonController person in crowd)
        {
            StartCoroutine(person.Clap());
        }
    }

    public void Idle()
    {
        foreach (KarateCrowdPersonController person in crowd)
        {
            StartCoroutine(person.Idle());
        }
    }

    public void Shock(float sfxProbability, float bigShockProbability = 0f, bool force = false)
    {
        /*for (int i = 0; i < config.shockNum; i++)
        {
            audioManager.Play("Shock" + i);
        }*/
        foreach (KarateCrowdPersonController person in crowd)
        {
            StartCoroutine(person.Shock(sfxProbability, bigShockProbability, force));
        }
    }

    public void Cheer(float cheerSFXProbability)
    {
        foreach (KarateCrowdPersonController person in crowd)
        {
            StartCoroutine(person.Cheer(0.6f, cheerSFXProbability));
        }
    }
}
