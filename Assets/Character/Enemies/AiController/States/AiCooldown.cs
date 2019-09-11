using UnityEngine;
using StateKraft;
//Auther: Simon

[CreateAssetMenu(menuName = "States/Ai/Cooldown")]
public class AiCooldown : AiState
{
    private AiState NextState;
    private float timer;

    public void SetTimer (AiState x, float z)
    {
        NextState = x;
        timer = z;
    }

    public override void Enter()
    {
       
    }

    public override void StateUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
            TransitionTo(NextState);
    }

    public override void Exit()
    {
        timer = 0;
    }
}
