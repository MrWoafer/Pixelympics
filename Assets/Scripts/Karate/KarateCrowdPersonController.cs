using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KarateCrowdPersonController : MonoBehaviour
{
    [Header("Cosmetics Settings")]
    public Color skinColour = new Color(0.9921f, 0.8549f, 0.749f);
    public Color hairColour = new Color(0.9803f, 0.8823f, 0.5019f);
    public Color topColour = new Color(0.3098f, 0.7294f, 0.9529f);
    public Color stripeColour = new Color(0.6313f, 0.8941f, 0.9411f);

    [Header("References")]
    public Animator anim;
    private SpriteRenderer sprRenderer;
    private KarateCrowdController controller;
    private AudioManager audioManager;
    public KarateConfig config;

    // Start is called before the first frame update
    void Awake()
    {
        sprRenderer = GetComponent<SpriteRenderer>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        config = GameObject.Find("Config").GetComponent<KarateConfig>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Create(KarateCrowdController controllerReference)
    {
        controller = controllerReference;

        skinColour = controllerReference.skinColours.colours[Random.Range(0, controllerReference.skinColours.colours.Length)];
        hairColour = controllerReference.skinColours.colours[Random.Range(0, controllerReference.hairColours.Length)];

        int index = Random.Range(0, controllerReference.topColours.colours.Length);
        topColour = controllerReference.topColours.colours[index];
        stripeColour = controllerReference.stripeColours.colours[index];

        UpdateCosmetics();
    }

    public void UpdateCosmetics()
    {
        sprRenderer.material.SetColor("_SkinColour", skinColour);
        sprRenderer.material.SetColor("_HairColour", hairColour);
        sprRenderer.material.SetColor("_TopColour", topColour);
        sprRenderer.material.SetColor("_StripeColour", stripeColour);
    }

    public IEnumerator Clap()
    {
        yield return new WaitForSeconds(Random.Range(controller.minClapWaitTime, controller.maxClapWaitTime));

        anim.SetTrigger("Clap");
    }
    private void SFXClap()
    {
        if (Functions.RandomBool(controller.clapChance))
        {
            int clapNum = Random.Range(0, controller.numOfClapSounds);
            //audioManager.Play("Clap" + clapNum, transform.position);
            audioManager.Play("Clap" + clapNum);
        }
    }

    public IEnumerator Idle()
    {
        yield return new WaitForSeconds(Random.Range(controller.minIdleWaitTime, controller.maxIdleWaitTime));

        anim.SetTrigger("Idle");
    }

    public IEnumerator Shock(float sfxProbability, float bigShockProbability = 0f, bool force = false)
    {
        if (Functions.RandomBool(controller.shockChance) || force)
        {
            yield return new WaitForSeconds(Random.Range(controller.minShockWaitTime, controller.maxShockWaitTime));

            anim.SetTrigger("Shock");

            if (Functions.RandomBool(sfxProbability))
            {
                if (Functions.RandomBool(bigShockProbability))
                {
                    int index = Random.Range(0, config.bigShockNum);
                    audioManager.Play("BigShock" + index);
                }
                else
                {
                    int index = Random.Range(0, config.shockNum);
                    audioManager.Play("Shock" + index);
                }
            }
        }
    }

    public IEnumerator Cheer(float clapProbability, float cheerSFXProbability)
    {
        yield return new WaitForSeconds(Random.Range(controller.minCheerWaitTime, controller.maxCheerWaitTime));

        if (Functions.RandomBool(clapProbability))
        {
            anim.SetTrigger("Clap");
        }
        else
        {
            anim.SetTrigger("Cheer");

            if (Functions.RandomBool(cheerSFXProbability))
            {
                SFXCheer();
            }
        }
    }
    private void SFXCheer()
    {
        if (Functions.RandomBool(controller.clapChance))
        {
            int clapNum = Random.Range(0, config.cheerNum);
            audioManager.Play("Cheer" + clapNum);
        }
    }
}
