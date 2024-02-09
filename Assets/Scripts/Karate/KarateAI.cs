using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class KarateAI : MonoBehaviour
{
    [Header("Settings")]
    public KaratePlaystyles playstyle = KaratePlaystyles.Neutral;

    [Header("Secret Settings")]
    //public int correctionCountIndividual = 1;
    //public int correctionCountTotal = 3;
    public float baseBasicBlockProbability = 0.6f;
    public float baseComboBlockProbability = 0.6f;
    public float blockProbabilityIndex = 0.3f;
    public int learnFactor = 6;

    [Header("References")]
    private KaratePlayerController player;
    private KarateConfig config;
    private Animator anim;
    private KarateRefereeController referee;
    private KaratePlayerController opponent;
    private Utils utils;
    
    private Queue<string> queue = new Queue<string>();

    ///              High    Medium    Low
    ///            + ----------------------
    /// Round kick |   .        .       .
    /// Punch      |   .        .       .
    /// Side kick  |   .        .       .
    private int[,] opponentsFollowUps = new int[3,4];

    /// Round kick | Punch | Side kick | Spin hook | Knee | Front kick | High side kick | Elbow
    ///     .      |   .   |     .     |     .     |   .  |      .     |       .        |   .
    private int[] opponentsReactions = new int[8];
    private string decision = "";

    private float attackCountdown = 0f;
    private bool defended = false;

    private int actionID = 0;

    private float waitTime = 0f;
    private bool waiting = false;

    private bool comboed = false;
    private bool isFollowUp = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<Utils>();
        utils = GetComponent<Utils>();

        player = GetComponent<KaratePlayerController>();
        config = GameObject.Find("Config").GetComponent<KarateConfig>();
        anim = GetComponent<Animator>();

        referee = GameObject.Find("Referee").GetComponent<KarateRefereeController>();
        opponent = player.opponentObj.GetComponent<KaratePlayerController>();

        //attackCountdown = 1f;
        attackCountdown = Random.Range(0.3f, 0.6f);

        /// Initialise arrays to work with Functions.WeightedRand() so they all have a chance of being used.
        for (int i = 0; i < opponentsReactions.Length; i++)
        {
            opponentsReactions[i] = learnFactor;
        }
        /*for (int i = 0; i < opponentsFollowUps.GetLength(0); i++)
        {
            for (int j = 0; j < opponentsFollowUps.GetLength(1); j++)
            {
                opponentsFollowUps[i, j] = 1;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isAI)
        {
            if (!referee.HasIntervened() && referee.HasStartedMatch())
            {
                if (playstyle == KaratePlaystyles.Aggressive)
                {
                    if (player.IsReady() && player.MovementsToOpponent() <= 2)
                    {
                        //defended = false;
                    }
                    if (QueueIsFree() && opponent.IsAttacking() && !defended && opponent.IsInSwingPhase())
                    {
                        Debug.Log("Under attack!");

                        string attackName = ProperAttackName(opponent.CurrentAnimClipName());

                        if (BlockAttack(attackName))
                        {
                            QueueClear();

                            if (attackName == "RoundKick")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Duck 0.7");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "Punch")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Block 0.6");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "SideKick")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("JumpBack");
                                QueueAdd("IsReady");
                                QueueAdd("Wait 0.5");
                                QueueAdd("JumpForward");
                                QueueAdd("IsReady");
                                QueueAdd("Defended");
                                //LogQueue();
                            }
                            else if (attackName == "SpinHook")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Duck 0.7");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "Knee")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Duck 0.7");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "FrontKick")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Block 0.7");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "HighSideKick")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Duck 0.7");
                                QueueAdd("Defended");
                            }
                            else if (attackName == "Elbow")
                            {
                                QueueAdd("IsReady");
                                QueueAdd("Block 0.7");
                                QueueAdd("Defended");
                            }
                        }
                        else if (QueueIsEmpty())
                        {
                            QueueAdd("Wait 0.5");
                            QueueAdd("Defended");
                        }

                        defended = true;
                    }
                    else if (player.IsReady() && player.MovementsToOpponent() > 2 && QueueIsEmpty())
                    {
                        if ((opponent.transform.position.x > transform.position.x && player.direction == 1f) || (opponent.transform.position.x < transform.position.x && player.direction == -1f))
                        {
                            QueueAdd("JumpForward");
                        }
                        else
                        {
                            QueueAdd("JumpBack");
                        }
                    }
                    else if (player.IsReady() && QueueIsFree())
                    {
                        attackCountdown -= Time.deltaTime;

                        if (attackCountdown <= 0f)
                        {
                            int attempts = 0;
                            int attackIndex;
                            bool attacked = false;
                            while (!attacked && attempts < 100)
                            {
                                attackIndex = Functions.WeightedRand(opponentsReactions);

                                if (attackIndex == 0 && Functions.WeightedRand(opponentsReactions) == DecisionToReactionIndex("RoundKick"))
                                {
                                    if (player.MovementsToOpponent() <= 2 && player.MovementsToOpponent() > 0)
                                    {
                                        QueueAdd("RoundKick");
                                        decision = "RoundKick";
                                        attacked = true;
                                    }
                                }
                                else if (attackIndex == 1 && Functions.WeightedRand(opponentsReactions) == DecisionToReactionIndex("Punch"))
                                {
                                    if (player.MovementsToOpponent() == 1)
                                    {
                                        QueueAdd("Punch");
                                        decision = "Punch";
                                        attacked = true;
                                    }
                                    else if (player.MovementsToOpponent() == 2)
                                    {
                                        QueueAdd("JumpForward");
                                        QueueAdd("IsReady");
                                        QueueAdd("Punch");
                                        decision = "Punch";
                                        attacked = true;
                                    }
                                }
                                else if (attackIndex == 2 && Functions.WeightedRand(opponentsReactions) == DecisionToReactionIndex("SideKick"))
                                {
                                    if (player.MovementsToOpponent() <= 1)
                                    {
                                        QueueAdd("SideKick");
                                        decision = "SideKick";
                                        attacked = true;
                                    }
                                    else if (player.MovementsToOpponent() <= 1 && Functions.RandomBool(0.5f))
                                    {
                                        QueueAdd("JumpForward");
                                        QueueAdd("IsReady");
                                        QueueAdd("SideKick");
                                        decision = "SideKick";
                                        attacked = true;
                                    }
                                }

                                attempts++;
                            }

                            //attackCountdown = 0.5f;
                            attackCountdown = Random.Range(0.5f, 0.7f);
                        }
                    }
                }
            }
            else
            {
                QueueClear();
                defended = false;
            }


            /// Act based on the queue
            string action = QueuePeek();
            
            bool doneAction = false;

            if (action == "IsReady" && player.IsReady())
            {
                QueueRemove();
                //doneAction = true;
                action = QueuePeek();
            }
            if (action == "Defended")
            {
                defended = false;
                QueueRemove();
                doneAction = true;
            }
            else if (Regex.IsMatch(action, "Wait [0-9]+(.[0-9]+)?"))
            {
                if (waiting)
                {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0f)
                    {
                        waiting = false;
                        QueueRemove();
                        doneAction = true;
                    }
                }
                else
                {
                    waitTime = float.Parse(action.Replace("Wait ", ""));
                    waiting = true;
                }
            }
            else if (action == "JumpBack")
            {
                JumpBack();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "JumpForward")
            {
                JumpForward();
                QueueRemove();
                doneAction = true;
            }
            else if (Regex.IsMatch(action, "Duck [0-9]+(.[0-9]+)?"))
            {
                if (waiting)
                {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0f)
                    {
                        waiting = false;
                        QueueRemove();
                        doneAction = true;
                    }
                }
                else
                {
                    waitTime = float.Parse(action.Replace("Duck ", ""));
                    Duck(waitTime);
                    waiting = true;
                }
            }
            else if (Regex.IsMatch(action, "Block [0-9]+(.[0-9]+)?"))
            {
                if (waiting)
                {
                    waitTime -= Time.deltaTime;
                    if (waitTime <= 0f)
                    {
                        waiting = false;
                        QueueRemove();
                        doneAction = true;
                    }
                }
                else
                {
                    waitTime = float.Parse(action.Replace("Block ", ""));
                    Block(waitTime);
                    waiting = true;
                }
            }
            else if (action == "RoundKick" && player.IsReady())
            {
                player.RoundKick();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "Punch" && player.IsReady())
            {
                player.Punch();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "SideKick" && player.IsReady())
            {
                player.SideKick();
                QueueRemove();
                doneAction = true;
            }
            if (action == "SpinHook")
            {
                player.RoundKickToSpinFrontKick();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "Knee")
            {
                player.PunchToKneeGrab();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "FrontKick")
            {
                player.PunchToFrontKick();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "HighSideKick")
            {
                player.SideKickToHighSideKick();
                QueueRemove();
                doneAction = true;
            }
            else if (action == "Elbow")
            {
                player.SideKickToElbow();
                QueueRemove();
                doneAction = true;
            }


            if (doneAction)
            {
                LogQueue();
                actionID++;
            }
        }
    }

    private void QueueAdd(string action)
    {
        queue.Enqueue(action);
        LogQueue();
    }
    private void QueueClear()
    {
        queue.Clear();
    }
    private string QueuePop()
    {
        return queue.Dequeue();
    }
    private string QueuePeek()
    {
        return !QueueIsEmpty() ? queue.Peek() : "";
    }
    private void QueueRemove()
    {
        queue.Dequeue();
    }
    private bool QueueIsEmpty()
    {
        return queue.Count == 0;
    }
    private void LogQueue()
    {
        string msg = "";
        foreach (string i in queue)
        {
            msg += i + ", ";
        }
        msg = msg.TrimEnd(' ', ',');
        Debug.Log(msg);
    }
    private bool QueueIsFree()
    {
        return QueueIsEmpty() || Regex.IsMatch(QueuePeek(), "(Wait|Duck|Block) [0-9]+(.[0-9]+)?");
    }

    public void Landed()
    {
        opponentsReactions[DecisionToReactionIndex(decision)] += 1;

        if (!comboed)
        {
            Combo();
        }
        else
        {
            FollowUp();
        }
    }

    public void Blocked()
    {

    }

    private int DecisionToReactionIndex(string decisionString)
    {
        switch (decisionString)
        {
            case "RoundKick":
                return 0;
            case "Punch":
                return 1;
            case "SideKick":
                return 2;
            case "SpinHook":
                return 3;
            case "Knee":
                return 4;
            case "FrontKick":
                return 5;
            case "HighSideKick":
                return 6;
            case "Elbow":
                return 7;
            default:
                return -1;
        }
    }

    private void JumpBack()
    {
        player.SetNotReady();
        anim.SetTrigger("JumpBack");
    }
    private void JumpForward()
    {
        player.SetNotReady();
        anim.SetTrigger("JumpForward");
    }

    private void Duck(float time)
    {
        StartCoroutine(DuckCoroutine(time));
    }
    IEnumerator DuckCoroutine(float time)
    {
        player.DuckStart();
        yield return new WaitForSeconds(time);
        player.DuckEnd();
        defended = false;
    }

    private void Block(float time)
    {
        StartCoroutine(BlockCoroutine(time));
    }
    IEnumerator BlockCoroutine(float time)
    {
        player.BlockStart();
        yield return new WaitForSeconds(time);
        player.BlockEnd();
        defended = false;
    }

    private string ProperAttackName(string name)
    {
        switch (name)
        {
            case "RoundKickToSpinFrontKick":
                return "SpinHook";
            case "PunchToKneeGrab":
                return "Knee";
            case "PunchToFrontKick":
                return "FrontKick";
            case "SideKickToHighSideKick":
                return "HighSideKick";
            case "SideKickToElbow":
                return "Elbow";
            default:
                return name;
        }
    }

    private bool BlockAttack(string attackName)
    {
        if (attackName == "RoundKick")
        {
            if (opponent.GetBasicAttacks() <= 2)
            {
                return Functions.RandomBool(baseBasicBlockProbability);
            }
            else
            {
                return Functions.RandomBool(baseBasicBlockProbability + (1 - baseBasicBlockProbability) * Mathf.Pow(opponent.GetRoundKicks() / opponent.GetBasicAttacks(), blockProbabilityIndex));
            }
        }
        else if (attackName == "Punch")
        {
            if (opponent.GetBasicAttacks() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseBasicBlockProbability + (1 - baseBasicBlockProbability) * Mathf.Pow(opponent.GetPunches() / opponent.GetBasicAttacks(), blockProbabilityIndex));
            }
        }
        else if (attackName == "SideKick")
        {
            if (opponent.GetBasicAttacks() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseBasicBlockProbability + (1 - baseBasicBlockProbability) * Mathf.Pow(opponent.GetSideKicks() / opponent.GetBasicAttacks(), blockProbabilityIndex));
            }
        }
        else if (attackName == "SpinHook")
        {
            if (opponent.GetCombos() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseComboBlockProbability + (1 - baseComboBlockProbability) * Mathf.Pow(opponent.GetSpinHooks() / opponent.GetCombos(), blockProbabilityIndex));
            }
        }
        else if (attackName == "Knee")
        {
            if (opponent.GetCombos() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseComboBlockProbability + (1 - baseComboBlockProbability) * Mathf.Pow(opponent.GetKnees() / opponent.GetCombos(), blockProbabilityIndex));
            }
        }
        else if (attackName == "FrontKick")
        {
            if (opponent.GetCombos() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseComboBlockProbability + (1 - baseComboBlockProbability) * Mathf.Pow(opponent.GetFrontKicks() / opponent.GetCombos(), blockProbabilityIndex));
            }
        }
        else if (attackName == "HighSideKick")
        {
            if (opponent.GetCombos() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseComboBlockProbability + (1 - baseComboBlockProbability) * Mathf.Pow(opponent.GetHighSideKicks() / opponent.GetCombos(), blockProbabilityIndex));
            }
        }
        else if (attackName == "Elbow")
        {
            if (opponent.GetCombos() <= 2)
            {
                return Functions.RandomBool(0.5f);
            }
            else
            {
                return Functions.RandomBool(baseComboBlockProbability + (1 - baseComboBlockProbability) * Mathf.Pow(opponent.GetElbows() / opponent.GetCombos(), blockProbabilityIndex));
            }
        }
        else
        {
            return false;
        }
    }

    private void Combo()
    {
        int attempts = 0;
        int attackIndex;
        bool attacked = false;
        while (!attacked && attempts < 100)
        {
            attackIndex = Functions.WeightedRand(opponentsReactions);

            if (decision == "RoundKick")
            {
                if (attackIndex == 3 && player.MovementsToOpponent() == 1)
                {
                    QueueAdd("Wait 0.1");
                    QueueAdd("SpinHook");
                    decision = "SpinHook";
                    attacked = true;
                    comboed = true;
                }
            }
            else if (decision == "Punch")
            {
                if (attackIndex == 4)
                {
                    QueueAdd("Wait 0.1");
                    QueueAdd("Knee");
                    decision = "Knee";
                    attacked = true;
                    comboed = true;
                    isFollowUp = true;
                }
                else if (attackIndex == 5)
                {
                    QueueAdd("Wait 0.1");
                    QueueAdd("FrontKick");
                    decision = "FrontKick";
                    attacked = true;
                    comboed = true;
                    isFollowUp = true;
                }
            }
            else if (decision == "SideKick")
            {
                if (attackIndex == 6)
                {
                    QueueAdd("Wait 0.1");
                    QueueAdd("HighSideKick");
                    decision = "HighSideKick";
                    attacked = true;
                    comboed = true;
                    isFollowUp = true;
                }
                else if (attackIndex == 7)
                {
                    QueueAdd("Wait 0.1");
                    QueueAdd("Elbow");
                    decision = "Elbow";
                    attacked = true;
                    comboed = true;
                    isFollowUp = true;
                }
            }

            attempts++;
        }
    }

    private void FollowUp()
    {
        int attackIndex = Random.Range(0, 3);

        if (attackIndex == 0)
        {
            if (player.MovementsToOpponent() <= 2 && player.MovementsToOpponent() > 0)
            {
                QueueAdd("IsReady");
                QueueAdd("RoundKick");
                decision = "RoundKick";
                comboed = false;
            }
        }
        else if (attackIndex == 1)
        {
            if (player.MovementsToOpponent() == 1)
            {
                QueueAdd("IsReady");
                QueueAdd("Punch");
                decision = "Punch";
                comboed = false;
            }
            else if (player.MovementsToOpponent() == 2)
            {
                QueueAdd("IsReady");
                QueueAdd("JumpForward");
                QueueAdd("IsReady");
                QueueAdd("Punch");
                decision = "Punch";
                comboed = false;
            }
        }
        else if (attackIndex == 2)
        {
            if (player.MovementsToOpponent() <= 1)
            {
                QueueAdd("IsReady");
                QueueAdd("SideKick");
                decision = "SideKick";
                comboed = false;
            }
            else if (player.MovementsToOpponent() == 2)
            {
                QueueAdd("IsReady");
                QueueAdd("JumpForward");
                QueueAdd("IsReady");
                QueueAdd("SideKick");
                decision = "SideKick";
                comboed = false;
            }
        }
    }
}
