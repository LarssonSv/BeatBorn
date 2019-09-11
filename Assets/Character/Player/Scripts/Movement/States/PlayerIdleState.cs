using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "States/PlayerMovement/IdleState")]
public class PlayerIdleState : PlayerState
{  
    public override void Enter()
    {
        PlayerAnimator.SetTrigger("ToIdle");
        PlayerAnimator.SetFloat("Velocity", 0f);
    }

    public override void StateUpdate()
    {
        if (Input.GetButtonDown("LightAttack") || Input.GetButtonDown("HeavyAttack"))
            TransitionTo<PlayerAttackState>();
        else if (Velocity.magnitude > 0 || DirectionInput.magnitude > 0)
            TransitionTo<PlayerGroundState>();
        else if (!GroundCheck())
            TransitionTo<PlayerAirState>();
        else
            player.CheckSyncPower();
    }

    public override void Exit()
    {
       
    }
}
