#pragma warning disable 0649
using UnityEngine;
using StateKraft;
using System.Collections.Generic;
using System.Collections;

//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/RangedAttackNexus")]
public class AiRangedAttackNexus : AiState
{
    [Header("RangedAttack")]
    [SerializeField] private string projectileTag;
    [SerializeField] private string animationTag;
    [SerializeField] private AnimationClip clip;
    [SerializeField] private float fireProjectileAt = 0.2f;
    [SerializeField] private float spawnProjetileAt = 0.1f;

    [Header("Behavior")]
    [SerializeField, Tooltip("Rotation toward our player")] private float RotationSpeed;
    [SerializeField] private float radius = 2f;
    [SerializeField] private LayerMask mask;

    //Properties
    private Vector3 firePoint => Ai.transform.position + Ai.transform.TransformDirection(Ai.transform.forward);

    //Cache
    private float timer;
    private bool shotFired;
    private bool turnedon;
    private ObjectPooler OP;
    private GameObject temp;


    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        OP = ObjectPooler.Instance;
    }

    public override void Enter()
    {
        timer = 0;
        turnedon = false;
        shotFired = false;
        Anime.SetTrigger(animationTag);
        if (Physics.CheckSphere(Ai.transform.position, radius, mask))
        {
            TransitionTo<AiAttack>();
        }

        
    }


    public override void StateUpdate()
    {
        LookAt(Ai.Nexus.transform.position);

        if (timer > clip.length)
            TransitionTo<AiWalking>();

        if (timer > spawnProjetileAt && !shotFired)
        {
            Vector3 heading = (NexusCrowdManager.Instance.transform.position + new Vector3(0f, 5f, 0f)) - Ai.FirePoint.position;
            temp = OP.SpawnFromPool(projectileTag, Ai.FirePoint.position, Quaternion.LookRotation(heading, Vector3.up),Ai.FirePoint);
            shotFired = true;
        }

        if (timer > fireProjectileAt && !turnedon)
        {
            temp.transform.parent = null;
            Vector3 heading = (NexusCrowdManager.Instance.transform.position + new Vector3(0f, 5f, 0f)) - Ai.FirePoint.position;
            temp.transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
            temp.GetComponent<ShooterProjectile>().ActivatThis();
            turnedon = true;
        }

        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        temp = null;
    }

    private void LookAt(Vector3 pos)
    {
        Vector3 lookPos = pos - Ai.transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        Ai.transform.rotation = Quaternion.Slerp(Ai.transform.rotation, rotation, Time.deltaTime * RotationSpeed);

    }


}
