using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "States/PlayerMovement/Ground")]
public class PlayerGroundState : PlayerState
{
    public float acceleration;
    public float ExtraTurnFriction;
    public float MaxSpeed;
    public float Friction;
    public float JumpHeight = 5;
    [SerializeField] private float GroundCheckDist = 0.1f;
    public override void Enter()
    {

    }

    public override void StateUpdate()
    {
        if (Input.GetButtonDown("LightAttack") || Input.GetButtonDown("HeavyAttack"))
            TransitionTo<PlayerAttackState>();     
        else if (Input.GetButtonDown("Dash"))
        {
            TransitionTo<PlayerDashState>();
        }
        else if (player.CheckSyncPower())
        {
            return;
        }
        else if (!GroundCheck(GroundCheckDist))
        {
            TransitionTo<PlayerAirState>();
            return;
        }

        Velocity += Vector3.down * 0.5f;
        Velocity += GetInputRelativeToCamera() * Time.deltaTime * acceleration;
        AddFriction(GetInputRelativeToCamera(), Time.deltaTime);
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);
        PlayerAnimator.SetFloat("Velocity" , Velocity.magnitude / MaxSpeed);
    }

    
    private void AddFriction(Vector3 movementInput, float deltaTime)
    {
        float dot = Vector3.Dot(movementInput.normalized, Velocity.normalized);
        if (dot > 0.0f) dot = 0.0f;
        float extraFriciton = ExtraTurnFriction * Mathf.Abs(dot);
        
        Vector3 frictionForce = Velocity.normalized * (Friction + extraFriciton) * deltaTime;

        if (frictionForce.magnitude > Velocity.magnitude)
            Velocity = Vector3.zero;
        else
            Velocity -= frictionForce;
    }
    public override void Exit()
    {

    }
}
