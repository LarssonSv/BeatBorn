using UnityEngine;
using StateKraft;
using UnityEngine.AI;
//Author: Simon

public class AiState : State
{
    protected AiController Ai;

    //Components
    protected NavMeshAgent Agent => Ai.Agent;
    protected Animator Anime => Ai.Anime;
    protected Transform PlayerTransform => AiController.Player;
    protected BoxCollider AttackCollider => Ai.AttackCollider;

    public override void Initialize(object owner)
    {
        Ai = (AiController)owner;
    }


}
