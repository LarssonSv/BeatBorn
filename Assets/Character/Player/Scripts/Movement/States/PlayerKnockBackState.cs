using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
[CreateAssetMenu(menuName = "States/PlayerMovement/KnockbackState")]
public class PlayerKnockBackState : PlayerState
{
    public string KnockbackAnimationTrigger;
    public float KnockbackTime = 0.25f;
    [EventRef] public string KnockbackSound;
    private float timer;

    public override void Enter()
    {          
        timer = KnockbackTime;
        RuntimeManager.PlayOneShotAttached(KnockbackSound, player.gameObject);
        PlayerAnimator.SetTrigger(KnockbackAnimationTrigger);
        Velocity = Vector3.zero;
    }

    public override void StateUpdate()
    {
        if (timer <= 0)
            TransitionTo<PlayerIdleState>();

        timer -= Time.deltaTime;
        
        
        Vector3 movement = PlayerAnimator.transform.position - Position;
        Velocity =  movement / Time.deltaTime;
        Velocity += Vector3.down;
        PlayerAnimator.transform.position = Position;

    }

    public override void Exit()
    {
        PlayerAnimator.SetFloat("DamageDirectionX", 0);
        PlayerAnimator.SetFloat("DamageDirectionY", 0);
    }
}
