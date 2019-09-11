#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

[CreateAssetMenu(menuName = "States/Ai/CoreAttack")]
public class AiCoreAttackState : AiState
{
    [SerializeField] private string coreAttackVFX = "VFXAiCoreAttack";
    [SerializeField] private float spawnVfxAt = 0.2f;
    [SerializeField] private float rotationSpeed = 6;
    [SerializeField] private int damageToCore = 1;
    [SerializeField] private int damageToSelf = 1;
    [SerializeField] private float damageFrequency = 0.5f;
    [SerializeField] private string AnimationTag;

    private ObjectPooler OP;
    private bool spawnVfx = false;
    private float timer;
    private GameObject vfx;
    private Vector3 heading;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        OP = ObjectPooler.Instance;
    }

    public override void Enter()
    {
        spawnVfx = false;
        Anime.SetTrigger(AnimationTag);
        Ai.StartCoroutine(RotateTowardsTarget());

        heading = (NexusCrowdManager.Instance.transform.position + new Vector3(0f, 6f, 0f)) - Ai.FirePoint.position;

        vfx = null;
    }

    public override void StateUpdate()
    {
        if(timer > spawnVfxAt && !spawnVfx)
        {
            vfx = OP.SpawnFromPool(coreAttackVFX, Ai.FirePoint.position, Quaternion.LookRotation(heading, Vector3.up));
            spawnVfx = true;
        }
            

        if (timer > damageFrequency)
        {
            timer = 0;
            Ai.TakeDamage(damageToSelf);

            CoreHealth.Instance.TakeDamage(damageToCore);
        }

        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        Ai.StopCoroutine(RotateTowardsTarget());
        Anime.ResetTrigger(AnimationTag);
        if (vfx != null)
        {
           
            vfx.transform.parent = null;
            vfx.SetActive(false);
            vfx = null;
            Debug.Log("disabled vfx");
        }
   
    }


    private IEnumerator RotateTowardsTarget()
    {
        Vector3 lookDirection = (new Vector3(NexusCrowdManager.Instance.transform.position.x, Ai.transform.position.y, NexusCrowdManager.Instance.transform.position.z) - Ai.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Ai.transform.up);

        while (Vector3.Dot(Ai.transform.forward, lookDirection) < 0.9f)
        {

            Ai.transform.rotation = Quaternion.Lerp(Ai.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }


}
