#pragma warning disable 0649
using UnityEngine;
using StateKraft;
//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/Fall")]
public class AiFall : AiState
{
    [SerializeField, Tooltip("What layers should be groundchecked")] LayerMask mask;
    [SerializeField, Range(0f, 6f), Tooltip("Range for raycast groundcheck")] float range = 2f;
    [SerializeField, Range(0f, 4f), Tooltip("Stun Enemy")] float stun = 1f;

    private float timer = 0;

    public override void Enter()
    {
        Agent.enabled = true;
        Anime.SetBool("Falling", true);
        timer = 0;
    }

    public override void StateUpdate()
    {
        if(Physics.Raycast(Ai.transform.position, Vector3.down, mask))
        {
            Anime.SetBool("Falling", false);
        }

        if (!Agent.isOnOffMeshLink)
        {
            Agent.velocity = Vector3.zero;
            timer += Time.deltaTime;
            if(timer > stun)
            {
                TransitionTo<AiWalking>();
            }

        }
    }

    public override void Exit()
    {
        Agent.enabled = false;
        Anime.SetBool("Falling", false);
    }
}
