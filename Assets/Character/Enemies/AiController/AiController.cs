#pragma warning disable 0649
using UnityEngine;
using System;
using StateKraft;
using UnityEngine.AI;
//Author: Simon

public class AiController : MonoBehaviour, IDamageable
{
    [Header("EnemyType:")]
    public Enemy EnemyType;

    [Header("Health Values:")]
    public float MaxHp = 100f;
    public float Hp;
    public Action OnDeath;
    public Action OnDamage;

    [Header("Navigation:")]
    public NpcSlot CurrentSlot;

    [Header("Knockback Values:")]
    public RhythmGrade GradeRequiredToKnockback;

    [Header("Components/refrences:")]
    public GameObject ChargeAttackVFX;
    public Animator Anime;
    public NavMeshAgent Agent;
    public Transform FirePoint;
    public BoxCollider AttackCollider;
    public static Transform Player;
    public NexusCrowdManager Nexus;

    [Header("System:")]
    public StateMachine Machine;

    [Header("Debug:")]
    [SerializeField] Vector3 gizmoOffset = Vector3.up;
    [SerializeField, Range(0f, 3f)] float gizmoSize = 0.1f;

    [SerializeField] private MaterialFloatLerper _hurtLerper;
    public Renderer Renderer;
    
    private void Awake()
    {
        Nexus = NexusCrowdManager.Instance;
        Anime = GetComponentInChildren<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        if (Player == null)
        {
            Player = FindObjectOfType<PlayerEngine>().transform;
        }
        Machine.Initialize(this);
        if(_hurtLerper == null)
            _hurtLerper = GetComponentInChildren<MaterialFloatLerper>();
    }

    public void Setup(Vector3 startPos)
    {
        CurrentSlot = Nexus.RequestSlot(startPos);
        Agent.enabled = true;
        Hp = MaxHp;
        Agent.Warp(startPos);
        Machine.TransitionTo<AiWalking>();
    }

    private void Update()
    {
        if (PauseGame.IsPaused())
            return;
      
        Machine.Update();
        CheckIfInBounds();
    }

    public void TakeDamage(int damage = 1, RhythmGrade grade = RhythmGrade.Miss, Transform attacker = null, float MissMultiplier = 1)
    {
        if(_hurtLerper)
            _hurtLerper.LerpValue(true);

        if (grade == RhythmGrade.Miss && MissMultiplier == 0)
            return;

        Hp -= grade == RhythmGrade.Miss ? damage * MissMultiplier : damage;
        OnDamage?.Invoke();
        if (Hp <= 0)
        {
            if ((int)grade > 0)
                PopUpPointsHandler.Instance.OnEnemyHitSpawn(transform.position, grade);
            Die();
        }
        else if (!(Machine.CurrentState is AiCoreAttackState))
        {
            if (CurrentSlot.playerIndex >= 0 && CurrentSlot.playerIndex <= 3)
            {
                Machine.GetState<AiWalking>().shouldFreeSlot = false;
            }

            if ((int)grade > (int)GradeRequiredToKnockback)
            {
                if(EnemyType != Enemy.shooterAi)
                    Machine.GetState<AiKnockbackState>().TriggerHeavyKnockBack = true;
            }
            if(EnemyType == Enemy.basicAi)
                Machine.TransitionTo<AiKnockbackState>();
            if((int)grade > 0)
                PopUpPointsHandler.Instance.OnEnemyHitSpawn(transform.position,grade);
        }
    }

    public void Die()
    {
        Player.GetComponent<AiSlotMachine>().SetSlotFree(CurrentSlot.playerIndex);
        if (CurrentSlot.nexusIndex != -1)
            Nexus.NpcSlots[CurrentSlot.nexusIndex].taken = false;
        Machine.TransitionTo<AiDeath>();
    }

    private void OnDrawGizmos()
    {
        if (Machine.CurrentState)
        {
            Gizmos.color = Machine.CurrentState.StateGizmoColor;
            Gizmos.DrawSphere(transform.position + gizmoOffset, gizmoSize);
        }
    }

    private void CheckIfInBounds()
    {
        if (transform.position.y < -100f)
        {
            Debug.Log("Johan och simon tänkte på denna bug haha");
            Die();
        }
    }

}

