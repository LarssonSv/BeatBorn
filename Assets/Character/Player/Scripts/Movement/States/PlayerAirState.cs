using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "States/PlayerMovement/AirState")]
public class PlayerAirState : PlayerState
{
    public float Gravity;
    public float TerminalVelocity;
    public float GroundCheckDistance = 0.01f;
    public float Friction;

    public override void Enter()
    {

    }

    public override void StateUpdate()
    {
        Velocity += Vector3.down * Gravity * Time.deltaTime;
        Vector3 frictionForce = Velocity.normalized * Friction * Time.deltaTime;    
        
        if (frictionForce.magnitude > Velocity.magnitude)
            Velocity = Vector3.zero;
        else
            Velocity -= frictionForce;


        if (GroundCheck(GroundCheckDistance))
            TransitionTo<PlayerGroundState>();
        
        if (Input.GetButtonDown("Dash"))
        {
            TransitionTo<PlayerDashState>();
        }

    }

    public override void Exit()
    {
       
    }
}
