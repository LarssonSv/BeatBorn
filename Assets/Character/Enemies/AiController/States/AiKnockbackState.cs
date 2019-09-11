using UnityEngine;
using StateKraft;
using FMODUnity;

//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/Knockback")]
public class AiKnockbackState : AiState
{
    [Header("Knockback Setup:")]
    public float KnockbackAmount = 4f;
    public float KnockbackTime = 0.25f;
    public float KnockBackTimeOnBeatMultiplier = 1.5f;

    [Header("Setup:")]
    [EventRef] public string KnockbackSound;
    public string KnockbackAnimationTrigger;

    private float timer = 0;
    private Vector3 ogpos;
    private Vector3 heading;
    private float newKnockbackTime;
    public bool TriggerHeavyKnockBack = false;

    public override void Enter()
    {
        Anime.SetTrigger("HurtRight");
        newKnockbackTime = KnockbackTime;
        if(Ai.EnemyType != Enemy.shooterAi)
            Agent.enabled = true;
        ogpos = Ai.transform.position;
        timer = 0f;
        RuntimeManager.PlayOneShotAttached(KnockbackSound, Ai.gameObject);
        //Ai.Anime.SetTrigger(KnockbackAnimationTrigger);

        if (TriggerHeavyKnockBack)
        {
            newKnockbackTime = newKnockbackTime * KnockBackTimeOnBeatMultiplier;
            TriggerHeavyKnockBack = false;
            Anime.ResetTrigger("HurtRight");
            Anime.SetBool("shock", true);
        }

    }

    public override void StateUpdate()
    {
        if (timer > newKnockbackTime)
            TransitionTo<AiWalking>();

        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        Agent.enabled = false;
        Anime.SetBool("shock", false);
    }
}
