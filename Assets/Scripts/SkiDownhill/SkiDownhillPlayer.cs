using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiDownhillPlayer : MonoBehaviour
{
    private PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName = "Test";
    private int playerID;
    public int playerNum = 0;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;
    public Color32 colour = new Color32(79, 148, 231, 255);

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "s";
    public string button5 = "e";

    [Header("Ski Info")]
    [SerializeField]
    private float angle = 0f;
    private float nearestMajorAngle
    {
        get
        {
            return NearestMajorAngle(angle);
        }
    }
    [SerializeField]
    private float downSpeed = 0f;
    [SerializeField]
    private float sideSpeed = 0f;
    private float speed
    {
        get
        {
            return Mathf.Sqrt(downSpeed * downSpeed + sideSpeed * sideSpeed);
        }
    }
    private float speedAngle
    {
        get
        {
            if (sideSpeed == 0f)
            {
                return 0f;
            }
            else if (sideSpeed > 0f)
            {
                return 90f - Mathf.Rad2Deg * Mathf.Atan2(downSpeed, sideSpeed);
            }
            else
            {
                return -90f - Mathf.Rad2Deg * Mathf.Atan2(-downSpeed, -sideSpeed);
            }
        }
    }
    [SerializeField]
    private bool isSkidding = false;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer sprRen;
    [SerializeField]
    private Animator anim;
    private SkiDownhillConfig config;
    [SerializeField]
    private Sprite[] spriteAnglesCrouch;
    [SerializeField]
    private Sprite[] spriteAnglesNormal;
    [SerializeField]
    private ParticleSystem snowParticlesPrefab;
    [SerializeField]
    private Transform snowSprayPoint0;
    [SerializeField]
    private Transform snowSprayPoint45;
    [SerializeField]
    private Transform snowSprayPoint90;
    [SerializeField]
    private Transform contactPointsContainer;
    private Dictionary<string,Transform[]> contactPoints = new Dictionary<string, Transform[]>();
    private List<Transform> currentContactPoints = new List<Transform>();
    private float currentContactAngle;
    private List<ParticleSystem> snowParticles = new List<ParticleSystem>();
    [SerializeField]
    private ParticleSystem snowTrail;

    private bool eligibleForRecord;

    private bool hasFinishedPushOff = true;

    private bool isCrouching
    {
        get
        {
            return Input.GetKey(button4) && !isBraking;
        }
    }
    private bool isBraking
    {
        get
        {
            return Input.GetKey(button3);
        }
    }

    private DirectionLR accelDir;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<SkiDownhillConfig>();

        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings != null)
        {
            playerID = playerSettings.playerIDs[playerNum];
            isAI = playerSettings.isAI[playerNum];
            playerName = playerSettings.names[playerNum];
            difficulty = playerSettings.difficulty[playerNum];
        }

        if (difficulty == Difficulty.Easy)
        {
            eligibleForRecord = false;
        }
        else if (difficulty == Difficulty.Medium)
        {
            eligibleForRecord = false;
        }
        else
        {
            eligibleForRecord = true;
        }
        if (playerName == "Test")
        {
            eligibleForRecord = false;
        }
        if (config.disableRecordEligibility)
        {
            eligibleForRecord = false;
        }

        for (int i = 0; i < 5; i++)
        {
            float currentAngle = new float[] { 0f, 22f, 45f, 67f, 90f }[i];

            Transform contactPointParent = contactPointsContainer.Find(currentAngle.ToString() + " - Bottom");

            Transform[] contactPointsForThisAngle = new Transform[contactPointParent.childCount];

            for (int j = 1; j <= contactPointsForThisAngle.Length; j++)
            {
                contactPointsForThisAngle[j - 1] = contactPointParent.Find(j.ToString());
            }

            contactPoints.Add(currentAngle.ToString() + " - Bottom", contactPointsForThisAngle);

            if (currentAngle != 0f && currentAngle != 90f)
            {
                contactPointParent = contactPointsContainer.Find(currentAngle.ToString() + " - Top");

                contactPointsForThisAngle = new Transform[contactPointParent.childCount];

                for (int j = 1; j <= contactPointsForThisAngle.Length; j++)
                {
                    contactPointsForThisAngle[j - 1] = contactPointParent.Find(j.ToString());
                }
            }

            contactPoints.Add(currentAngle.ToString() + " - Top", contactPointsForThisAngle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFinishedPushOff)
        {
            if (Input.GetKey(button1))
            {
                angle -= (isCrouching ? config.rotationSpeedCrouch : config.rotationSpeedNormal) * Time.deltaTime;
            }
            if (Input.GetKey(button2))
            {
                angle += (isCrouching ? config.rotationSpeedCrouch : config.rotationSpeedNormal) * Time.deltaTime;
            }

            angle = Functions.RoundToRange(angle, -config.maxAngle, config.maxAngle);

            /*if (nearestMajorAngle == 0f)
            {
                downSpeed += (isCrouching ? config.speedGainCrouch : config.speedGainNormal) * Time.deltaTime;
            }
            else if (nearestMajorAngle <= 22f)
            {
                downSpeed += (isCrouching ? config.speedGainCrouch : config.speedGainNormal) * 0.9f * Time.deltaTime;
            }
            else if (nearestMajorAngle <= 45f)
            {
                downSpeed += (isCrouching ? config.speedGainCrouch : config.speedGainNormal) * 0.8f * Time.deltaTime;
            }
            else if (nearestMajorAngle <= 67f)
            {
                downSpeed += (isCrouching ? config.speedGainCrouch : config.speedGainNormal) * 0.5f * Time.deltaTime;
            }*/
            downSpeed += (isCrouching ? config.speedGainCrouch : config.speedGainNormal) * Functions.Cosd(angle) * Time.deltaTime;

            if (Mathf.Abs(angle - speedAngle) > config.turningSpeedExchangeAngleTolerance)
            {
                if (angle >= 0f)
                {
                    if (speedAngle < angle)
                    {
                        //downSpeed -= config.turningSpeedExchangeRate * Time.deltaTime;
                        //sideSpeed += config.turningSpeedExchangeRate * config.turningSpeedLossScalar * Time.deltaTime;

                        downSpeed -= config.turningSpeedExchangeRate * config.turningDownSpeedLossScalar * Time.deltaTime;
                        sideSpeed += config.turningSpeedExchangeRate * config.turningSideSpeedLossScalar * Time.deltaTime;

                        accelDir = DirectionLR.right;
                    }
                    else if (speedAngle > angle)
                    {
                        //downSpeed -= config.turningSpeedExchangeRate * config.turningSpeedLossScalar * Time.deltaTime;
                        //sideSpeed -= config.turningSpeedExchangeRate * Time.deltaTime;

                        downSpeed += config.turningSpeedExchangeRate * config.turningDownSpeedLossScalar * Time.deltaTime;
                        sideSpeed -= config.turningSpeedExchangeRate * config.turningSideSpeedLossScalar * Time.deltaTime;

                        accelDir = DirectionLR.left;
                    }
                }
                else
                {
                    if (speedAngle > angle)
                    {
                        //downSpeed -= config.turningSpeedExchangeRate * Time.deltaTime;
                        //sideSpeed -= config.turningSpeedExchangeRate * config.turningSpeedLossScalar * Time.deltaTime;

                        downSpeed -= config.turningSpeedExchangeRate * config.turningDownSpeedLossScalar * Time.deltaTime;
                        sideSpeed -= config.turningSpeedExchangeRate * config.turningSideSpeedLossScalar * Time.deltaTime;

                        accelDir = DirectionLR.left;
                    }
                    else if (speedAngle < angle)
                    {
                        //downSpeed -= config.turningSpeedExchangeRate * config.turningSpeedLossScalar * Time.deltaTime;
                        //sideSpeed += config.turningSpeedExchangeRate * Time.deltaTime;

                        downSpeed += config.turningSpeedExchangeRate * config.turningDownSpeedLossScalar * Time.deltaTime;
                        sideSpeed += config.turningSpeedExchangeRate * config.turningSideSpeedLossScalar * Time.deltaTime;

                        accelDir = DirectionLR.right;
                    }
                }
            }

            float skidDeceleration = config.SkidDeceleration(angle, speedAngle);

            if (isBraking)
            {
                if (Mathf.Abs(angle - speedAngle) > config.breakAngleTolerance)
                {
                    skidDeceleration += config.breakDeceleration;
                }
            }

            if (Mathf.Abs(nearestMajorAngle) >= 67f)
            {
                downSpeed -= skidDeceleration * Time.deltaTime;
            }
            else if (Mathf.Abs(nearestMajorAngle) <= 22f)
            {
                sideSpeed -= Mathf.Sign(sideSpeed) * skidDeceleration * Time.deltaTime;
            }
            else
            {
                downSpeed -= skidDeceleration / 2f * Time.deltaTime;
                sideSpeed -= Mathf.Sign(sideSpeed) * skidDeceleration / 2f * Time.deltaTime;
            }

            if (skidDeceleration > 0f)
            {
                if (currentContactAngle != nearestMajorAngle)
                {
                    currentContactAngle = nearestMajorAngle;
                    currentContactPoints = new List<Transform>();

                    string topOrBottom = Mathf.Sign(angle) == (float)accelDir ? "Bottom" : "Top";
                    if (nearestMajorAngle == 0f || Mathf.Abs(nearestMajorAngle) == 90f)
                    {
                        topOrBottom = "Bottom";
                    }

                    foreach (Transform contactPoint in contactPoints[Mathf.Abs(currentContactAngle).ToString() + " - " + topOrBottom])
                    {
                        Transform transform = new GameObject().transform;

                        transform.position = contactPoint.position;
                        transform.eulerAngles = contactPoint.eulerAngles;
                        transform.parent = contactPoint.parent;

                        currentContactPoints.Add(transform);
                    }

                    foreach (Transform contactPoint in currentContactPoints)
                    {
                        if (nearestMajorAngle == 0f)
                        {
                            contactPoint.localPosition = new Vector3(contactPoint.localPosition.x * -Mathf.Sign(speedAngle), contactPoint.localPosition.y, contactPoint.localPosition.z);
                        }
                        else
                        {
                            contactPoint.localPosition = new Vector3(contactPoint.localPosition.x * Mathf.Sign(angle), contactPoint.localPosition.y, contactPoint.localPosition.z);
                        }
                    }

                    foreach (ParticleSystem particles in snowParticles)
                    {
                        particles.Stop();
                        Destroy(particles.gameObject, 1f);
                    }

                    snowParticles.Clear();

                    for (int i = 0; i < currentContactPoints.Count; i++)
                    {
                        ParticleSystem particles = Instantiate(snowParticlesPrefab, currentContactPoints[i].position, snowParticlesPrefab.transform.rotation, transform);

                        snowParticles.Add(particles);

                        float speedScalar = 0.95f;

                        var main = particles.main;
                        var emin = particles.emission;
                        var shape = particles.shape;

                        if (topOrBottom == "Bottom")
                        {
                            if (angle >= 0f)
                            {
                                shape.rotation = new Vector3(90f, nearestMajorAngle - 90f, 0f);
                            }
                            else
                            {
                                shape.rotation = new Vector3(90f, nearestMajorAngle + 90f, 0f);
                            }
                        }
                        else
                        {
                            if (angle >= 0f)
                            {
                                shape.rotation = new Vector3(90f, nearestMajorAngle + 90f, 0f);
                            }
                            else
                            {
                                shape.rotation = new Vector3(90f, nearestMajorAngle - 90f, 0f);
                            }
                        }

                        if (currentContactAngle == 0f)
                        {
                            //main.startSpeed = sideSpeed * speedScalar;
                            //particles.transform.localEulerAngles = new Vector3(-140f, -60f * Mathf.Sign(speedAngle), 0f);

                            shape.rotation = new Vector3(90f, 70f * Mathf.Sign(sideSpeed), 0f);

                            //speedScalar = 1f;
                        }
                        else if (Mathf.Abs(currentContactAngle) == 22f)
                        {
                            //main.startSpeed = Mathf.Sqrt(sideSpeed * sideSpeed * 1.5f + downSpeed * downSpeed * 0.5f) * speedScalar;
                            //particles.transform.localEulerAngles = new Vector3(-140f, -30f * Mathf.Sign(speedAngle), 0f);
                        }
                        else if (Mathf.Abs(currentContactAngle) == 45f)
                        {
                            //main.startSpeed = Mathf.Sqrt(sideSpeed * sideSpeed + downSpeed + downSpeed) * speedScalar;
                            //particles.transform.localEulerAngles = new Vector3(-150f, -20f * Mathf.Sign(speedAngle), 0f);
                        }
                        else if (Mathf.Abs(currentContactAngle) == 67f)
                        {
                            //main.startSpeed = Mathf.Sqrt(sideSpeed * sideSpeed * 0.5f + downSpeed * downSpeed * 1.5f) * speedScalar;
                            //particles.transform.localEulerAngles = new Vector3(-150f, -10f * Mathf.Sign(speedAngle), 0f);
                        }
                        else if (Mathf.Abs(currentContactAngle) == 90f)
                        {
                            //main.startSpeed = downSpeed * speedScalar;
                            //particles.transform.localEulerAngles = new Vector3(-160f, 0f, 0f);

                            shape.rotation = new Vector3(130f, 0f, 0f);
                        }

                        main.startSpeed = speed * speedScalar;
                        //particles.transform.localEulerAngles = new Vector3(speedAngle + 90f, -90f, 0f);

                        emin.rateOverTime = Mathf.Min(5f + skidDeceleration / config.maxSkidDeceleration * 500f, 200f);

                        particles.Play();
                    }
                }

                if (!isSkidding)
                {
                    isSkidding = true;

                    foreach (ParticleSystem particles in snowParticles)
                    {
                        particles.Play();
                    }
                }
            }
            else
            {
                if (isSkidding)
                {
                    isSkidding = false;
                    foreach (ParticleSystem particles in snowParticles)
                    {
                        particles.Stop();
                    }
                }
            }

            if (isSkidding && Mathf.Sign(sideSpeed) != (float)accelDir)
            {
                /*if (Mathf.Abs(nearestMajorAngle) <= 22f)
                {
                    snowParticles.transform.position = snowSprayPoint0.position;
                }
                else if (Mathf.Abs(nearestMajorAngle) <= 67f)
                {
                    snowParticles.transform.position = snowSprayPoint45.position;
                }
                else
                {
                    snowParticles.transform.position = snowSprayPoint90.position;
                }

                snowParticles.transform.localPosition = new Vector3(snowParticles.transform.localPosition.x * -Mathf.Sign(speedAngle),
                    snowParticles.transform.localPosition.y, snowParticles.transform.localPosition.z);

                float particleRate = 400f * skidDeceleration / config.maxSkidDeceleration + 200f;

                var emin = snowParticles.emission;
                emin.rateOverTime = new ParticleSystem.MinMaxCurve(particleRate);

                var vel = snowParticles.velocityOverLifetime;
                vel.x = new ParticleSystem.MinMaxCurve(sideSpeed * 1f, sideSpeed * 1.8f);
                vel.y = new ParticleSystem.MinMaxCurve(-downSpeed * 0.1f, -downSpeed * 0.3f);*/
            }
        }

        downSpeed = Functions.RoundToRange(downSpeed, config.minDownSpeed, config.maxDownSpeed);
        sideSpeed = Functions.RoundToRange(sideSpeed, -config.maxSideSpeed, config.maxSideSpeed);

        transform.position += new Vector3(sideSpeed * Time.deltaTime, -downSpeed * Time.deltaTime, 0f);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y / 1000f);

        var trailEmin = snowTrail.emission;
        if (downSpeed >= sideSpeed)
        {
            trailEmin.rateOverTime = 20f * downSpeed / config.maxDownSpeed;
        }
        else
        {
            trailEmin.rateOverTime = 20f * sideSpeed / config.maxSideSpeed;
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (hasFinishedPushOff)
        {
            anim.enabled = false;

            if (angle < -0f)
            {
                sprRen.flipX = true;
            }
            else
            {
                sprRen.flipX = false;
            }

            if (isCrouching)
            {
                sprRen.sprite = spriteAnglesCrouch[Mathf.RoundToInt(Mathf.Abs(angle) / config.maxAngle * (spriteAnglesCrouch.Length - 1))];
            }
            else
            {
                sprRen.sprite = spriteAnglesNormal[Mathf.RoundToInt(Mathf.Abs(angle) / config.maxAngle * (spriteAnglesNormal.Length - 1))];
            }
        }
    }

    private float NearestMajorAngle(float angle)
    {
        int index = Mathf.RoundToInt(Mathf.Abs(angle) / config.maxAngle * (spriteAnglesCrouch.Length - 1));
        switch (index)
        {
            case 0: return 0f;
            case 1: return 22f * Mathf.Sign(angle);
            case 2: return 45f * Mathf.Sign(angle);
            case 3: return 67f * Mathf.Sign(angle);
            case 4: return 90f * Mathf.Sign(angle);
            default: throw new System.Exception("Unexpected angle index: " + index);
        }
    }
}
