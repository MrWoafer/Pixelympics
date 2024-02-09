using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SkiJumper : MonoBehaviour
{
    public PlayerSettingsScript playerSettings;

    [Header("Player Settings")]
    public string playerName;
    public int playerNum;
    public bool isAI = false;
    public Difficulty difficulty = Difficulty.Hard;

    [Header("Controls")]
    public string button1 = "a";
    public string button2 = "d";
    public string button3 = "w";
    public string button4 = "e";

    [Header("Info")]
    public bool canDie = true;
    public bool dead = false;

    [Header("References")]
    public Transform skiFront;
    public Transform skiBack;
    public CircleCollider2D skiFrontCollider;
    public CircleCollider2D skiBackCollider;
    public CircleCollider2D headCollider;
    public Collider2D rampCollider;
    public Collider2D rampStartCollider;
    public Collider2D fullSkiCollider;
    private Rigidbody2D rb;
    private SkiJumpConfig config;
    public GameObject spriteObj;
    public Text backOnGroundText;
    public Text frontOnGroundText;
    public Text bodyOnGroundText;
    public ParticleSystem bloodParticles;
    public GameObject deadObj;
    public CameraFollow camFollow;
    public CapsuleCollider2D deadCollider;
    public ParticleSystem snowParticlesSkiBack;
    public ParticleSystem snowParticlesSkiFront;
    public ParticleSystem snowParticlesDead;
    private ParticleSystem.EmissionModule snowParticlesSkiBackEmission;
    private ParticleSystem.EmissionModule snowParticlesSkiFrontEmission;
    private ParticleSystem.EmissionModule snowParticlesDeadEmission;
    public CircleCollider2D[] deadBodyColliders;
    private ParticleSystem snowParticlesDead2;
    private ParticleSystem.EmissionModule snowParticlesDead2Emission;
    public GameObject ski1;
    public GameObject ski2;
    private Animator anim;
    public Text distanceText;
    private OlympicsController olympicsController;

    private bool eligibleForRecord;

    private float angularVelocityOffset = 0f;

    private bool landed = false;
    private float jumpDistance;

    private bool skiBackOnSnow = false;
    private bool skiFrontOnSnow = false;
    private bool deadBodyOnSnow = false;
    private Vector3 snowDeadCollisionPoint;
    private Vector3 snowDeadCollisionNormal;

    private bool started = false;
    private bool startedRamp = false;
    private bool offRamp = false;
    private bool pastHillEnd = false;

    private float distanceToFinish;
    private float finishStartingVelocity;
    private float timeTillFinish;
    private float finishDeceleration;
    private float finishT;

    private float aiStartT;
    private float aiRampAngle;
    private float aiAirAngle;
    private float aiLandingAngle;
    private bool aiAngleGoneBelow = true;

    // Start is called before the first frame update
    void Start()
    {
        config = GameObject.Find("Config").GetComponent<SkiJumpConfig>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        //rb.constraints = RigidbodyConstraints2D.FreezeAll;

        try
        {
            playerSettings = GameObject.Find("PlayerSettings").GetComponent<PlayerSettingsScript>();
        }
        catch
        {

        }

        if (playerSettings != null)
        {
            isAI = playerSettings.isAI[playerNum];
            playerName = playerSettings.names[playerNum];
            difficulty = playerSettings.difficulty[playerNum];
        }

        if (difficulty == Difficulty.Olympic)
        {
            eligibleForRecord = true;
        }
        else if (difficulty == Difficulty.Hard)
        {
            eligibleForRecord = true;
        }
        else
        {
            eligibleForRecord = false;
        }
        if (config.disableRecordEligibility)
        {
            eligibleForRecord = false;
        }

        //RotateVelocity(config.windVelocity, 0f);

        //rb.centerOfMass = rb.centerOfMass;
        //headCollider.density = 0f;
        //headCollider.isTrigger = false;

        //deadCollider = deadObj.GetComponent<CapsuleCollider2D>();
        deadObj.SetActive(false);
        spriteObj.SetActive(true);

        snowParticlesSkiBackEmission = snowParticlesSkiBack.emission;
        snowParticlesSkiFrontEmission = snowParticlesSkiFront.emission;
        snowParticlesDeadEmission = snowParticlesDead.emission;

        snowParticlesDead2 = Instantiate(snowParticlesDead, transform);
        snowParticlesDead2Emission = snowParticlesDead2.emission;

        Debug.Log("Ski Jump WR: " + PlayerPrefs.GetFloat("Ski Jump Record", 100f).ToString("n3") + "m");
        Debug.Log("Ski Jump Worst Record: " + PlayerPrefs.GetFloat("Ski Jump Worst Record", 100f).ToString("n3") + "m. Held by: " + PlayerPrefs.GetString("Ski Jump Worst Record Holder", ""));
        Debug.Log(playerName + "'s PB: " + PlayerPrefs.GetFloat("Ski Jump PB " + playerName, 0f).ToString("n3") + "m");

        rampStartCollider.enabled = true;
        rampCollider.enabled = false;
        fullSkiCollider.enabled = true;

        if (isAI)
        {
            aiStartT = config.aiStartTime;

            aiRampAngle = GetAIRampAngle();
            aiAirAngle = GetAIAirAngle();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Time.deltaTime: " + Time.deltaTime);
            Debug.Log("Time.fixedDeltaTime: " + Time.fixedDeltaTime);
        }

        if (!dead)
        {
            if (!isAI)
            {
                if (Input.GetKeyDown(button3))
                {
                    if (SkiBackIsGrounded() || SkiFrontIsGrounded())
                    {
                        Debug.Log("Jump!");
                        rb.AddForce(Vector2.up * config.jumpForce, ForceMode2D.Impulse);
                    }
                }

                if (!started)
                {
                    if (Input.GetKeyDown(button4))
                    {
                        Debug.Log("Push!");
                        started = true;
                        //rb.constraints = RigidbodyConstraints2D.None;
                        rb.AddForce(transform.right * config.startPushForce, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    if (Input.GetKey(button1))
                    {
                        //rb.angularVelocity = 0f;
                        //angularVelocityOffset += config.torque;
                        //rb.AddForceAtPosition(skiFront.up * config.torque * Time.fixedDeltaTime, skiFront.position);
                        //RotateAboutPoint(spriteObj, skiBack.position, config.torque * Time.deltaTime);
                        //Recentre();

                        Rotate(config.backwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, config.backwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, config.backwardAirTorque * Time.deltaTime / 0.007f / 60f);
                        //RotateVelocity(config.backwardGroundVelocity, config.backwardAirVelocity);
                    }
                    else if (Input.GetKeyUp(button1))
                    {
                        //RotateVelocity(-config.backwardGroundVelocity, -config.backwardAirVelocity);
                    }
                    if (Input.GetKey(button2))
                    {
                        //rb.angularVelocity = 0f;
                        //angularVelocityOffset -= config.torque;
                        //rb.AddForceAtPosition(skiBack.up * config.torque * Time.fixedDeltaTime, skiBack.position);
                        //RotateAboutPoint(spriteObj, skiFront.position, -config.torque * Time.deltaTime);
                        //Recentre();

                        Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                        //RotateVelocity(-config.forwardGroundVelocity, -config.forwardAirVelocity);
                    }
                    else if (Input.GetKeyUp(button2))
                    {
                        //RotateVelocity(config.forwardGroundVelocity, config.forwardAirVelocity);
                    }
                }
            }
            else
            {
                if (!started)
                {
                    aiStartT -= Time.deltaTime;
                    if (aiStartT <= 0f)
                    {
                        Debug.Log("Push!");
                        started = true;
                        rb.AddForce(transform.right * config.startPushForce, ForceMode2D.Impulse);
                    }
                }
                else
                {
                    aiStartT -= Time.deltaTime;
                    if (transform.position.x < config.rampEndX && aiStartT < -config.aiAfterPushWaitTime)
                    {
                        if (Functions.ModAngle(transform.eulerAngles.z) > aiRampAngle)
                        {
                            Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                        }
                        else
                        {
                            aiRampAngle = GetAIRampAngle();
                        }
                    }
                    else if (transform.position.x > config.rampEndX)
                    {
                        /*if (aiAngleGoneBelow && Functions.ModAngle(rb.rotation) > aiAirAngle)
                        {
                            if (rb.angularVelocity > 0f && Functions.ModAngle(rb.rotation) > aiAirAngle)
                            {
                                Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                            }
                            else if (rb.angularVelocity < 0f && Functions.ModAngle(rb.rotation) > aiAirAngle + config.aiAirAngleTolerance)
                            {
                                Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                            }
                            else if (rb.angularVelocity < 0f && Functions.ModAngle(rb.rotation) < aiAirAngle + config.aiAirAngleTolerance)
                            {
                                aiAngleGoneBelow = false;
                            }
                        }
                        if (rb.rotation < aiAirAngle && !aiAngleGoneBelow && rb.angularVelocity > 0f)
                        {
                            aiAngleGoneBelow = true;
                        }
                        else
                        {
                            aiAirAngle = GetAIAirAngle();
                        }*/

                        if (rb.angularVelocity <= config.aiAngularVelocityTolerance && Functions.ModAngle(rb.rotation) > aiAirAngle + config.aiAirAngleTolerance)
                        {
                            Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                        }
                        else if(rb.angularVelocity > config.aiAngularVelocityTolerance && Functions.ModAngle(rb.rotation) > aiAirAngle - config.aiAirAngleTolerance)
                        {
                            Rotate(-config.forwardGroundTorqueSkiBack * Time.deltaTime / 0.007f / 60f, -config.forwardGroundTorqueSkiFront * Time.deltaTime / 0.007f / 60f, -config.forwardAirTorque * Time.deltaTime / 0.007f / 60f);
                        }
                        aiAirAngle = GetAIAirAngle();
                        if (Functions.ModAngle(rb.rotation) < 0f)
                        {
                            aiAirAngle = GetAIAirAngle();
                        }
                    }
                }
            }
        }

        if (skiBack.position.x > config.rampStartX && !startedRamp)
        {
            startedRamp = true;
            rampStartCollider.enabled = false;
            rampCollider.enabled = true;
            fullSkiCollider.enabled = false;
        }
        if (skiBack.position.x > config.rampEndX && !offRamp)
        {
            offRamp = true;
            anim.SetTrigger("Fly");
            Debug.Log((2f * rb.velocity.magnitude).ToString("n5") + "m/s");
        }
        if(transform.position.x > config.hillEndX && !pastHillEnd)
        {
            pastHillEnd = true;
            distanceToFinish = config.finishX - transform.position.x;
            finishStartingVelocity = rb.velocity.x;

            finishDeceleration = rb.velocity.x * rb.velocity.x / 2f / distanceToFinish;
            timeTillFinish = rb.velocity.x / finishDeceleration;
            finishT = 0f;
        }

        /*if (Input.GetKeyDown(button3))
        {
            Debug.Log("Jump!");
            rb.AddForce(Vector2.up * config.jumpForce * 3f, ForceMode2D.Impulse);
        }*/

        if (skiBack.position.x > config.rampStartX)
        {
            if (transform.position.x < config.rampCurveStartX && !dead)
            {
                float d1 = Vector3.Distance(rampCollider.ClosestPoint(skiBack.transform.position), skiBack.transform.position);
                float d2 = Vector3.Distance(rampCollider.ClosestPoint(skiFront.transform.position), skiFront.transform.position);

                float d = Mathf.Clamp(d1 + d2, 0f, 0.5f);

                rb.drag = Mathf.Pow(d, 2f) / 2f;
            }
            else if (transform.position.x > config.rampEndX && !dead)
            {
                //rb.drag = config.GetDrag(rb.rotation);
                //rb.drag = config.GetDrag(Mathf.Clamp(rb.rotation, -45f, 45f));
                rb.drag = config.GetDrag(Mathf.Clamp(Functions.ModAngle(rb.rotation), -45f, 45f));

                if (CheckIfLanded() && !landed)
                {
                    Land();
                }
                else if (landed)
                {
                    rb.drag = 0f;
                }
            }

            if (dead)
            {
                snowParticlesDeadEmission.rateOverTime = Mathf.Clamp(6f * Mathf.Pow(rb.velocity.magnitude, 1.3f), 0f, 5000f);
                snowParticlesDead.transform.position = snowDeadCollisionPoint;
                snowParticlesDead.transform.eulerAngles = new Vector3(-90f, 90f, -90f);
                snowParticlesDead.transform.LookAt(snowParticlesDead.transform.position + snowDeadCollisionNormal);

                snowParticlesDead2Emission.rateOverTime = Mathf.Clamp(6f * Mathf.Pow(rb.velocity.magnitude, 1.6f), 0f, 5000f);
                snowParticlesDead2.transform.eulerAngles = new Vector3(-90f, 90f, -90f);

                //snowParticlesDeadEmission.rateOverTime = 0f;
                //ParticleSystem.Burst burst = snowParticlesDeadEmission.GetBurst(0);
                //burst.count = 6f * Mathf.Pow(rb.velocity.magnitude, 1.6f);

                if (DeadBodyIsOnSnow())
                {
                    CircleCollider2D furthestRight = null;
                    CircleCollider2D furthestLeft = null;
                    foreach (CircleCollider2D col in deadBodyColliders)
                    {
                        Collider2D[] cols = Physics2D.OverlapCircleAll(col.transform.position, col.radius);

                        foreach (Collider2D col2 in cols)
                        {
                            if (col2.tag == "Landing")
                            {
                                //PlayParticles(snowParticlesDead, col.transform.position, new Vector3(-90f, 90f, -90f));

                                if (furthestRight == null || col.transform.position.x > furthestRight.transform.position.x)
                                {
                                    furthestRight = col;
                                }
                                if (furthestLeft == null || col.transform.position.x < furthestRight.transform.position.x)
                                {
                                    furthestLeft = col;
                                }
                            }
                        }
                    }
                    if (furthestRight != null)
                    {
                        //PlayParticles(snowParticlesDead, furthestRight.transform.position, new Vector3(-90f, 90f, -90f));
                        snowParticlesDead.transform.position = furthestRight.transform.position;

                        if (furthestLeft != furthestRight)
                        {
                            snowParticlesDead2.transform.position = furthestLeft.transform.position;
                            //snowParticlesDead2.Play();
                        }
                        else
                        {
                            snowParticlesDead2.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        }
                    }
                }

                if (DeadBodyIsOnSnow() && !deadBodyOnSnow)
                {
                    deadBodyOnSnow = true;

                    //Debug.Log("OH MY GOSH! There's a dead body on the snow!");

                    snowParticlesDead.Play();
                    //snowParticlesDead2.Play();

                    /// Make the dead body no longer able to collide with the skis.
                    ski1.layer = 17;
                    ski2.layer = 17;
                }
                else
                {
                    if (!DeadBodyIsOnSnow() && deadBodyOnSnow)
                    {
                        deadBodyOnSnow = false;

                        snowParticlesDead.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        snowParticlesDead2.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }

                    if (transform.position.x > config.rampEndX && rb.angularVelocity < 60f)
                    {
                        //Rotate(0f, 0f, config.windTorque / 6f * Time.fixedDeltaTime);
                    }
                }
            }

            backOnGroundText.text = "Back: " + (SkiBackIsGrounded() ? "ground" : "air");
            frontOnGroundText.text = "Front: " + (SkiFrontIsGrounded() ? "ground" : "air");
            bodyOnGroundText.text = "Body: " + (DeadBodyIsOnSnow() ? "ground" : "air");

            //snowParticlesSkiBack.transform.position = skiBack.position;

            snowParticlesSkiBackEmission.rateOverTime = Mathf.Pow(rb.velocity.magnitude, 1.4f);
            if (SkiBackIsOnSnow() && !skiBackOnSnow)
            {
                skiBackOnSnow = true;
                snowParticlesSkiBack.Play();
            }
            else if (!SkiBackIsOnSnow() && skiBackOnSnow)
            {
                skiBackOnSnow = false;
                snowParticlesSkiBack.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }

            snowParticlesSkiFrontEmission.rateOverTime = Mathf.Pow(rb.velocity.magnitude, 1.4f);
            if (SkiFrontIsOnSnow() && !skiFrontOnSnow && !skiBackOnSnow)
            {
                skiFrontOnSnow = true;
                snowParticlesSkiFront.Play();
            }
            else if ((!SkiFrontIsOnSnow() || skiBackOnSnow) && skiFrontOnSnow)
            {
                skiFrontOnSnow = false;
                snowParticlesSkiFront.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if (!landed)
        {
            distanceText.text = (2f * (skiBack.position.x - config.rampEndX)).ToString("n3") + "m";
        }
        else
        {
            distanceText.text = jumpDistance.ToString("n3") + "m";
        }
    }

    void FixedUpdate()
    {
        //rb.angularVelocity -= angularVelocityOffset;
        //angularVelocityOffset = 0f;

        

        /*if (Input.GetKeyDown(button1))
        {
            RotateAboutPoint(skiFront.position, 45f);
        }
        if (Input.GetKeyDown(button2))
        {
            RotateAboutPoint(skiFront.position, -45f);
        }*/

        //angularVelocityOffset += config.windTorque;

        //rb.angularVelocity += angularVelocityOffset;

        //rb.AddForceAtPosition(skiFront.up * config.windTorque, skiFront.position);
        //rb.velocity = Vector2.up * 3f;

        if (skiBack.position.x > config.rampStartX)
        {
            if (!dead && !pastHillEnd)
            {
                //Rotate(config.windTorque * Time.fixedDeltaTime, config.windTorque * Time.fixedDeltaTime, 0f);
                Rotate(config.windTorque * Time.fixedDeltaTime, config.windTorque * Time.fixedDeltaTime, config.offRampWindTorque * Time.fixedDeltaTime);
            }

            if (HeadHasHit() && canDie && !dead)
            {
                Die();
            }
        }

        if (pastHillEnd && !dead)
        {
            finishT += Time.fixedDeltaTime;
            //rb.velocity = new Vector2(Mathf.Lerp(finishStartingVelocity, 0f, (transform.position.x - config.hillEndX) / distanceToFinish), rb.velocity.y);
            //rb.velocity = new Vector2(rb.velocity.x - rb.velocity.x * rb.velocity.x / 2f / (config.finishX - transform.position.x) * Time.fixedDeltaTime, rb.velocity.y);
            //rb.velocity = new Vector2(finishStartingVelocity - finishT * finishDeceleration, rb.velocity.y);

            if (finishStartingVelocity * finishStartingVelocity - 2f * finishDeceleration * (transform.position.x - config.hillEndX) >= 0f)
            {
                rb.velocity = new Vector2(Mathf.Sqrt(finishStartingVelocity * finishStartingVelocity - 2f * finishDeceleration * (transform.position.x - config.hillEndX)), rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }
    }

    private static void RotateAboutPoint(GameObject obj, Vector3 point, float angle)
    {
        RotateAboutPoint(obj, new Vector2(point.x, point.y), angle);
    }
    private static void RotateAboutPoint(GameObject obj, Vector2 point, float angle)
    {
        //Vector2 vel = new Vector2(rb.velocity.x, rb.velocity.y);

        /*angle *= Mathf.Deg2Rad;
        float halfAngle = angle / 2f;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);

        float length = Vector2.Distance(pos, point) * Mathf.Sqrt(2f - 2f * Mathf.Cos(angle));

        Vector2 newPos = pos + new Vector2(transform.up.x * Mathf.Cos(halfAngle) - transform.up.y * Mathf.Sin(halfAngle), transform.up.x * Mathf.Sin(halfAngle) + transform.up.y * Mathf.Cos(halfAngle)).normalized * length;

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);*/

        Vector2 pos = new Vector2(obj.transform.position.x, obj.transform.position.y);

        if (pos == point)
        {
            throw new System.ArgumentException("Rotation point must not be equal to the position of the object.");
        }

        pos -= point;
        pos = Matrix2x2.RotationMatrixDegrees(angle) * pos;
        pos += point;

        obj.transform.position = pos;
        obj.transform.eulerAngles += new Vector3(0f, 0f, angle);

        //rb.velocity = vel;
    }

    private void Recentre()
    {
        Vector3 spritePosition = spriteObj.transform.position;
        transform.position = spritePosition;
        spriteObj.transform.position = spritePosition;
    }

    private bool SkiBackIsGrounded()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(skiBack.position, skiBackCollider.radius);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Ramp" || col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    private bool SkiBackIsOnSnow()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(skiBack.position, skiBackCollider.radius);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    private bool SkiFrontIsGrounded()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(skiFront.position, skiFrontCollider.radius);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Ramp" || col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    private bool SkiFrontIsOnSnow()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(skiFront.position, skiFrontCollider.radius);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    /*private void Rotate(float groundTorqueSkiBack, float groundTorqueSkiFront, float airTorque)
    {
        if (Functions.AreSameSign(groundTorqueSkiBack, groundTorqueSkiFront, airTorque))
        {
            bool frontGrounded = SkiFrontIsGrounded();
            bool backGrounded = SkiBackIsGrounded();
            if (groundTorque > 0f)
            {
                if (backGrounded)
                {
                    rb.AddForceAtPosition(skiFront.up * groundTorque, skiFront.position);
                }
                else if (frontGrounded)
                {
                    rb.AddForceAtPosition(-skiBack.up * groundTorque, skiBack.position);
                }
                else
                {
                    rb.AddTorque(airTorque);
                }
            }
            else if (groundTorque < 0f)
            {
                if (frontGrounded)
                {
                    rb.AddForceAtPosition(-skiBack.up * groundTorque, skiBack.position);
                }
                else if (backGrounded)
                {
                    rb.AddForceAtPosition(skiFront.up * groundTorque, skiFront.position);
                }
                else
                {
                    rb.AddTorque(airTorque);
                }
            }
        }
        else
        {
            throw new System.Exception("groundTorques and airTorque must both be of the same sign (or at least one zero). groundTorqueSkiBack: " + groundTorqueSkiBack + "; groundTorqueSkiFront: " + groundTorqueSkiFront + "; airTorque: " + airTorque);
        }
    }*/

    private void Rotate(float groundTorqueSkiBack, float groundTorqueSkiFront, float airTorque)
    {
        if (Functions.AreSameSign(groundTorqueSkiBack, groundTorqueSkiFront, airTorque))
        {
            bool frontGrounded = SkiFrontIsGrounded();
            bool backGrounded = SkiBackIsGrounded();

            if (backGrounded)
            {
                if (groundTorqueSkiBack > 0f)
                {
                    rb.AddForceAtPosition(skiFront.up * groundTorqueSkiBack, skiFront.position);
                }
                else if (frontGrounded)
                {
                    rb.AddForceAtPosition(skiBack.up * -groundTorqueSkiFront, skiBack.position);
                }
                else
                {
                    rb.AddForceAtPosition(skiFront.up * groundTorqueSkiBack, skiFront.position);
                }
            }
            else if (frontGrounded)
            {
                if (groundTorqueSkiFront < 0f)
                {
                    rb.AddForceAtPosition(skiBack.up * -groundTorqueSkiFront, skiBack.position);
                }
                else if (backGrounded)
                {
                    rb.AddForceAtPosition(skiFront.up * groundTorqueSkiBack, skiFront.position);
                }
                else
                {
                    rb.AddForceAtPosition(-skiBack.up * groundTorqueSkiFront, skiBack.position);
                }
            }
            else
            {
                rb.AddTorque(airTorque);
            }
        }
        else
        {
            throw new System.Exception("groundTorques and airTorque must both be of the same sign (or at least one zero). groundTorqueSkiBack: " + groundTorqueSkiBack + "; groundTorqueSkiFront: " + groundTorqueSkiFront + "; airTorque: " + airTorque);
        }
    }

    private void RotateVelocity(float groundAngularVelocityChange, float airAngularVelocityChange)
    {
        if (groundAngularVelocityChange * airAngularVelocityChange >= 0f)
        {
            bool frontGrounded = SkiFrontIsGrounded();
            bool backGrounded = SkiBackIsGrounded();
            if (groundAngularVelocityChange > 0f)
            {
                if (backGrounded)
                {
                    rb.AddForceAtPosition(skiFront.up * groundAngularVelocityChange, skiFront.position, ForceMode2D.Impulse);
                }
                else if (frontGrounded)
                {
                    rb.AddForceAtPosition(-skiBack.up * groundAngularVelocityChange, skiBack.position, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddTorque(airAngularVelocityChange, ForceMode2D.Impulse);
                }
            }
            else if (groundAngularVelocityChange < 0f)
            {
                if (frontGrounded)
                {
                    rb.AddForceAtPosition(-skiBack.up * groundAngularVelocityChange, skiBack.position, ForceMode2D.Impulse);
                }
                else if (backGrounded)
                {
                    rb.AddForceAtPosition(skiFront.up * groundAngularVelocityChange, skiFront.position, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddTorque(airAngularVelocityChange, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            throw new System.Exception("groundTorque and airTorque must both be of the same sign (or at least one zero). groundTorque: " + groundAngularVelocityChange + "; airTorque: " + airAngularVelocityChange);
        }
    }

    private void Die()
    {
        dead = true;
        Debug.Log("Dead!");

        BroadcastScore(float.MinValue);

        bloodParticles.Play();

        deadObj.SetActive(true);
        spriteObj.SetActive(false);

        rb.velocity = rb.velocity;
        rb.angularVelocity = rb.angularVelocity;

        ski1.SetActive(true);
        //ski1.transform.parent = null;
        //ski1.GetComponent<Rigidbody2D>().velocity = rb.velocity;
        //ski1.GetComponent<Rigidbody2D>().angularVelocity = rb.angularVelocity;

        if (ski2 != null)
        {
            ski2.SetActive(true);
            //ski2.transform.parent = null;
        }
    }

    private bool CheckIfLanded()
    {
        return GetLandingPoint() != Vector3.zero;
    }

    private Vector3 GetLandingPoint()
    {
        Collider2D[] cols1 = Physics2D.OverlapCircleAll(skiBack.position, skiBackCollider.radius);

        foreach (Collider2D col in cols1)
        {
            if (col.tag == "Landing")
            {
                return skiBack.transform.position;
            }
        }
        
        Collider2D[] cols2 = Physics2D.OverlapCircleAll(skiFront.position, skiFrontCollider.radius);

        foreach (Collider2D col in cols2)
        {
            if (col.tag == "Landing")
            {
                return skiFront.transform.position;
            }
        }

        return Vector3.zero;
    }

    private void Land()
    {
        anim.SetTrigger("Ski");

        GameObject landingPoint = new GameObject("Landing Point");
        landingPoint.transform.position = GetLandingPoint();

        landed = true;
        /// Double as the player object is only 1m in scale, so doubling makes him ~human height.
        jumpDistance = (landingPoint.transform.position.x - config.rampEndX) * 2f;
        Debug.Log("Jump Distance: " + jumpDistance.ToString("n3") + "m");

        if (!dead)
        {
            BroadcastScore(jumpDistance);

            if (jumpDistance > PlayerPrefs.GetFloat("Ski Jump PB " + playerName, 0f) && eligibleForRecord)
            {
                Debug.Log(playerName + " got a new PB!");
                PlayerPrefs.SetFloat("Ski Jump PB " + playerName, jumpDistance);
            }

            if (jumpDistance > PlayerPrefs.GetFloat("Ski Jump Record", 100f) && eligibleForRecord)
            {
                PlayerPrefs.SetFloat("Ski Jump Record", jumpDistance);
                Debug.Log("New Record!");
            }
            else if (jumpDistance < PlayerPrefs.GetFloat("Ski Jump Worst Record", 100f) && jumpDistance > 0f && eligibleForRecord)
            {
                PlayerPrefs.SetFloat("Ski Jump Worst Record", jumpDistance);
                PlayerPrefs.SetString("Ski Jump Worst Record Holder", playerName);
                Debug.Log("New Worst Record!");
            }
        }
    }


    public bool HeadHasHit()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(headCollider.transform.position, headCollider.radius);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Ramp" || col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    private bool DeadBodyIsOnSnow()
    {
        Collider2D[] cols = Physics2D.OverlapCapsuleAll(deadObj.transform.TransformPoint(Functions.Vector2To3(deadCollider.offset)), deadCollider.size, deadCollider.direction, deadObj.transform.eulerAngles.z);

        foreach (Collider2D col in cols)
        {
            if (col.tag == "Landing")
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Landing")
        {
            ContactPoint2D[] contacts = col.contacts;
            contacts = contacts.OrderBy(x => -x.point.x).ToArray();
            snowDeadCollisionPoint = contacts[0].point;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Landing")
        {
            ContactPoint2D[] contacts = col.contacts;
            contacts = contacts.OrderBy(x => -x.point.x).ToArray();
            snowDeadCollisionPoint = contacts[0].point;
            snowDeadCollisionNormal = Matrix2x2.RotationMatrixDegrees(config.snowParticlesAngle) * contacts[0].normal;
        }
    }

    private void PlayParticles(ParticleSystem particles, Vector3 position, Vector3 rotation)
    {
        ParticleSystem parts = Instantiate(particles, position, Quaternion.Euler(rotation.x, rotation.y, rotation.z)).GetComponent<ParticleSystem>();
        parts.Play();
        Destroy(parts.gameObject, parts.main.duration);
    }

    private float GetAIRampAngle()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinRampAngle, config.aiOlympicMaxRampAngle);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinRampAngle, config.aiHardMaxRampAngle);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinRampAngle, config.aiMediumMaxRampAngle);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinRampAngle, config.aiEasyMaxRampAngle);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    private float GetAIAirAngle()
    {
        if (difficulty == Difficulty.Olympic)
        {
            return Random.Range(config.aiOlympicMinAirAngle, config.aiOlympicMaxAirAngle);
        }
        else if (difficulty == Difficulty.Hard)
        {
            return Random.Range(config.aiHardMinAirAngle, config.aiHardMaxAirAngle);
        }
        else if (difficulty == Difficulty.Medium)
        {
            return Random.Range(config.aiMediumMinAirAngle, config.aiMediumMaxAirAngle);
        }
        else if (difficulty == Difficulty.Easy)
        {
            return Random.Range(config.aiEasyMinAirAngle, config.aiEasyMaxAirAngle);
        }
        else
        {
            throw new System.Exception("Unknown difficulty: " + difficulty);
        }
    }

    public void SetOlympicsController(OlympicsController controller)
    {
        olympicsController = controller;
    }

    public void BroadcastScore(float score)
    {
        if (olympicsController != null)
        {
            olympicsController.RecordScore(score);
        }
    }
}
