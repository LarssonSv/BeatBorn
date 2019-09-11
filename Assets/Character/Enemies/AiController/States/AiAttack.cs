#pragma warning disable 0649
using UnityEngine;
using StateKraft;
using System.Collections.Generic;
using System.Collections;

//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/Attack")]
public class AiAttack : AiState
{
    [Header("Attack Values:")]
    [SerializeField] private PlayerAttackState.AttackStruct[] attacks;
    [SerializeField] private float AttackRange = 2f;
    [SerializeField] private LayerMask mask;
    [SerializeField] private float attackCD = 0.3f;

    [Header("Behavior:")]
    [SerializeField, Tooltip("RotationSpeed towards target")] private float RoationSpeed;

    //Cache
    [System.NonSerialized] public int AttackIndex = 0;
    private Vector3 swordLastPos;
    private List<Transform> hitObjects;
    private bool comboFail;
    private bool wasInWindow;
    private Vector3 playerLastPos;
    private float timer = 0;
    private Vector3 halfExtends;
    private bool playedIdle = false;
    private bool waitForBeat = false;

    //Properties

    private PlayerAttackState.AttackStruct currentAttack => attacks[AttackIndex];

    private float currentClipLength => attacks[AttackIndex].Clip.length;
    private int currentFrame => (int)(timer * attacks[AttackIndex].Clip.frameRate);

    private bool inAttackWindow => currentFrame >= attacks[AttackIndex].attackWindow.x &&
                                   currentFrame <= attacks[AttackIndex].attackWindow.y;

    ParticleSystem[] chargeUpVFXs = new ParticleSystem[0];

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        hitObjects = new List<Transform>();
        halfExtends = new Vector3(AttackCollider.size.x * AttackCollider.transform.lossyScale.x,
            AttackCollider.size.y * AttackCollider.transform.lossyScale.y,
            AttackCollider.size.z * AttackCollider.transform.lossyScale.z);

        if (Ai.ChargeAttackVFX)
            chargeUpVFXs = Ai.ChargeAttackVFX.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem x in chargeUpVFXs)
        {
            x.Stop();
        }


    }


    public override void Enter()
    {
        waitForBeat = true;
        AttackIndex = Random.Range(0, attacks.Length);
        BeatManager.OnBeat += WaitedForBeat;
    }


    public override void StateUpdate()
    {
        if(!currentAttack.ShouldNotRotateInAttack)
            LookAtPlayer();

        if (currentAttack.RootMotion)
        {
            Ai.transform.rotation = Anime.transform.rotation;
            Ai.transform.position = Anime.transform.position;
            Anime.transform.localPosition = Vector3.zero;
        }

        if (waitForBeat)
            return;

        if (inAttackWindow)
            CheckForSwordCollsion();

        if (timer > currentClipLength && (PlayerTransform.position - Ai.transform.position).magnitude > AttackRange)
            TransitionTo<AiWalking>();
        else if (timer > currentClipLength && !playedIdle)
        {
            playedIdle = true;
            Anime.SetTrigger("Idle");
            foreach (ParticleSystem x in chargeUpVFXs)
            {
                x.Stop();
            }

        }

        else if (timer > (currentClipLength + attackCD))
            SetUpAttack();

        timer += Time.deltaTime;

    }

    private void SetUpAttack()
    {
        Agent.velocity = Vector3.zero;
        Anime.SetFloat("Velocity", 0f);
        Anime.SetTrigger(currentAttack.AttackTag);
        playedIdle = false;
        hitObjects.Clear();
        timer = 0;
        wasInWindow = false;

        foreach (ParticleSystem x in chargeUpVFXs)
        {
            x.Stop();
            x.Play();
        }

    }

    private void LookAtPlayer()
    {
        Vector3 lookPos = PlayerTransform.position - Ai.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        Ai.transform.rotation = Quaternion.Slerp(Ai.transform.rotation, rotation, Time.deltaTime * RoationSpeed);

    }

    private void CheckForSwordCollsion()
    {
        Collider[] hits = Physics.OverlapBox(swordLastPos, halfExtends, AttackCollider.transform.rotation, mask);
        swordLastPos = AttackCollider.transform.position;
        Debug.Log(currentFrame);
        foreach (Collider x in hits)
        {
            if (hitObjects.Contains(x.transform.root))
                continue;

            hitObjects.Add(x.transform.root);
            x.transform.root.GetComponent<IDamageable>()?.TakeDamage(currentAttack.Damage, RhythmGrade.Miss, Ai.transform);
            Debug.Log(x.transform.root.name);
            currentAttack.HitParticle.Play(x.transform.position,
            Quaternion.LookRotation((x.transform.position - PlayerTransform.position).normalized));
        }


    }

    private void WaitedForBeat()
    {
        SetUpAttack();
        waitForBeat = false;
        BeatManager.OnBeat -= WaitedForBeat;
    }

    public override void Exit()
    {
        if (currentAttack.RootMotion)
        {
            Ai.transform.rotation = Anime.transform.rotation;
            Ai.transform.position = Anime.transform.position;
            Anime.transform.localPosition = Vector3.zero;
        }
        Anime.SetTrigger("Idle");
        wasInWindow = false;
        waitForBeat = false;
        BeatManager.OnBeat -= WaitedForBeat;

        foreach (ParticleSystem x in chargeUpVFXs)
        {
            x.Stop();
        }
    }


}
