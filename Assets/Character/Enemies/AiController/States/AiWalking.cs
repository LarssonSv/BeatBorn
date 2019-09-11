#pragma warning disable 0649
using UnityEngine;
using StateKraft;
using UnityEngine.AI;
//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/Walking")]
public class AiWalking : AiState
{

    [Header("Player Interaction:")]
    [SerializeField, Range(0f, 100f)] private float agroRadius = 5f;
    [SerializeField, Range(0f, 10f)] private float attackRange = 1f;

    [Header("Nexus Interaction:")]
    [SerializeField, Range(0f, 20f), Tooltip("From what distance should the Ai ignore the player.")] private float ignoreAgro = 10f;
    [SerializeField, Range(0f, 50)] private float nexusAttackRange = 1f;

    [Header("Misc:")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField, Range(0f, 30f), Tooltip("When should we start falling.")] private float groundRange = 2f;
    [SerializeField, Tooltip("Should be default for ground.")] private LayerMask mask;

    //Properties
    private bool foundPlayer => (PlayerTransform.position - Ai.transform.position).magnitude < agroRadius;
    private Vector3 currentTargetPos => Ai.CurrentSlot.pos;

    //Cache
    // private Vector3 heading;
    private float resetTimer = 0f;
    private bool waitForBeat = false;
    private bool goingForNexus;
    private bool disableAgent = true;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private PlayerEngine engine;
    [System.NonSerialized] public bool shouldFreeSlot = true;


    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        engine = FindObjectOfType<PlayerEngine>();
    }

    public override void Enter()
    {
        resetTimer = 0;
        waitForBeat = false;
        BeatManager.OnBeat += StartWalkOnBeat;
        shouldFreeSlot = true;
        disableAgent = true;
        Agent.enabled = true;
        GetDestination();
        Agent.SetDestination(currentTargetPos);
        Anime.ResetTrigger("HurtRight");
        Anime.SetTrigger("Idle");
    }

    public override void StateUpdate()
    {
        if (waitForBeat)
        {
            GetDestination();
            Agent.SetDestination(currentTargetPos);
            Anime.SetFloat("Velocity", Agent.velocity.magnitude / Agent.speed);


            if (engine.CurrentHP > 0)
            {
                if (Agent.hasPath && goingForNexus && (Vector3.ProjectOnPlane(currentTargetPos, Vector3.up) - Vector3.ProjectOnPlane(Agent.transform.position, Vector3.up)).magnitude < nexusAttackRange)
                {
                    if (Ai.EnemyType == Enemy.shooterAi)
                        TransitionTo<AiRangedAttackNexus>();
                    else
                        TransitionTo<AiCoreAttackState>();
                }
                else if (Agent.hasPath && !goingForNexus && (Vector3.ProjectOnPlane(currentTargetPos, Vector3.up) - Vector3.ProjectOnPlane(Agent.transform.position, Vector3.up)).magnitude < attackRange && Agent.remainingDistance < 0.1f)
                {
                    shouldFreeSlot = false;
                    TransitionTo<AiAttack>();
                }
                else if (Agent.hasPath && !goingForNexus && Vector3.Distance(Ai.transform.position, PlayerTransform.position) < attackRange)
                {
                    shouldFreeSlot = false;
                    TransitionTo<AiAttack>();
                }
            }
        }


        if (Agent.isOnOffMeshLink)
        {
            disableAgent = false;
            TransitionTo<AiFall>();
        }

        if(Agent.hasPath && Agent.velocity.magnitude == 0f)
        {
            if (resetTimer > 1f)
            {
                Agent.SetDestination(new Vector3(-33.17f, 0, -50.89f));
                resetTimer = 0;
            }
                
            else
                resetTimer += Time.deltaTime;
        }

    }

    public override void Exit()
    {
        if (shouldFreeSlot)
            AiSlotMachine.Instace.SetSlotFree(Ai.CurrentSlot.playerIndex);
        if (disableAgent)
            Agent.enabled = false;
        if (Ai.CurrentSlot.nexusIndex != -1)
            NexusCrowdManager.Instance.NpcSlots[Ai.CurrentSlot.nexusIndex].taken = false;

        BeatManager.OnBeat -= StartWalkOnBeat;
    }

    private void GetDestination()
    {
        if ((int)Ai.EnemyType <= 1)
        {
            if (foundPlayer)
            {
                float dist = Vector3.Distance(Ai.Nexus.transform.position, Ai.transform.position);
                if (dist > ignoreAgro)
                {
                    SetPlayerPos();
                }
                else
                {
                    SetNexusPos();
                }

            }
            else
            {
                SetNexusPos();
            }
        }
        else
        {
            SetNexusPos();
        }
    }

    private void SetNexusPos()
    {
        AiSlotMachine.Instace.SetSlotFree(Ai.CurrentSlot.playerIndex);

        if (Ai.CurrentSlot.nexusIndex == -1)
        {
            Ai.CurrentSlot = NexusCrowdManager.Instance.RequestSlot(Ai.transform.position);
        }

        goingForNexus = true;
    }

    public void StartWalkOnBeat()
    {
        waitForBeat = true;
        BeatManager.OnBeat -= StartWalkOnBeat;
    }

    private void SetPlayerPos()
    {
        if (engine.CurrentHP > 0)
        {

            if (Ai.CurrentSlot.playerIndex >= 0 && Ai.CurrentSlot.playerIndex <= AiSlotMachine.Instace.slots)
            {
                NexusCrowdManager.Instance.SetSlotFree(Ai.CurrentSlot.nexusIndex);
                AiSlotMachine.Instace.SetSlotFree(Ai.CurrentSlot.playerIndex);
                Ai.CurrentSlot = AiSlotMachine.Instace.RequestSlot(Ai.transform.position);
            }
            else
            {
                NexusCrowdManager.Instance.SetSlotFree(Ai.CurrentSlot.nexusIndex);
                //AiSlotMachine.Instace.SetSlotFree(Ai.CurrentSlot.playerIndex);
                NpcSlot temp = AiSlotMachine.Instace.RequestSlot(Ai.transform.position);
                if (temp.playerIndex == -1)
                {
                    SetNexusPos();
                }
                else
                {
                    Ai.CurrentSlot = temp;
                    goingForNexus = false;
                }
            }
        }
        else
        {
            SetNexusPos();
        }

    }


}


