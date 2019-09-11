#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.Experimental.Animations;
using UnityEngine.Experimental.VFX;

[CreateAssetMenu(menuName = "States/PlayerMovement/Dash")]
public class PlayerDashState : PlayerState
{
    [Header("Dash To Enemy Settings:")]
    [SerializeField] private float radius;
    [SerializeField] private LayerMask mask;

    [Header("Dash Settings:")]
    [SerializeField] private float dashTime;
    [SerializeField] private float acceleration;
    [SerializeField] private float MaxSpeed;
    private float friction => acceleration / MaxSpeed / Time.deltaTime;

    [SerializeField] private float gravity;
    [SerializeField] private static int numDashes = 3;
    [SerializeField] private float dashCD = 10f;

    [Header("Fmod Sound:")]
    [EventRef] public string DashSFX;

    private TrailRenderer dashVFX;
    private ParticleSystem dashChargeVFX;
    private Vector3 ogInput;
    private float timer = 0;
    private float[] cooldowns = new float[numDashes];
    private bool normalDash = false;
    private float startWidth;
    private List<Transform> hitObjects = new List<Transform>();
    private Collider[] hits;
    private Renderer rend;
    public static Action OnDash;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        dashVFX = player.DashVFX.GetComponent<TrailRenderer>();
        dashChargeVFX = player.dashElectricVFX;
        rend = player.transform.GetChild(0).Find("SM_Player_Retop").GetComponent<Renderer>();

        dashChargeVFX.Stop();
        dashVFX.enabled = false;
        startWidth = dashVFX.startWidth;

        for (int i = 0; i < numDashes; i++)
        {
            cooldowns[i] = 10f;
        }
    }

    public override void Enter()
    {
        if (!isOffColldown())
        {
            TransitionTo<PlayerIdleState>();
            return;
        }

        if(BeatManager.GetGrade() != RhythmGrade.Miss)
            SyncPowerManager.KeepTheBeatGoing();

        StreakCounterUI.DashTimerReset();
        RuntimeManager.PlayOneShot(DashSFX);
        normalDash = false;
        timer = 0;
        hitObjects.Clear();
        ogInput = GetInputRelativeToCamera();
        Debug.Log(ogInput);
        if (ogInput.magnitude <= 0.01f)
        {
            ogInput = PlayerAnimator.transform.forward;
            Debug.Log("Small man");
        }
        ogInput = ogInput.normalized;
        
        PlayerAnimator.SetBool("inDash", true);

        for (int i = numDashes - 1; i >= 0; i--)
        {
            if (cooldowns[i] > dashCD)
            {
                normalDash = true;
                cooldowns[i] = 0f;
                break;
            }

        }
        
        OnDash?.Invoke();
        dashVFX.enabled = true;
        dashChargeVFX.Play();
    }

    public override void StateUpdate()
    {
        if (normalDash)
            NormalDash();

        if (Input.GetButtonDown("HeavyAttack") || Input.GetButtonDown("LightAttack"))
            TransitionTo<PlayerAttackState>();
        
        dashVFX.startWidth = startWidth * (1 - timer / dashTime);
    }

    private void NormalDash()
    {
        Velocity = ogInput * acceleration;
        Velocity += Vector3.down * gravity;

//        Velocity = player.ApplyNormalForce(Velocity); TODO: Add physics

        if (timer > dashTime)
        {
            TransitionTo<PlayerGroundState>();
        }

        if(timer > dashTime / 2f)
        {
            if (Physics.CheckSphere(player.transform.position, radius, mask, QueryTriggerInteraction.Ignore))
            {
                TransitionTo<PlayerGroundState>();
            }
        }


        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        dashVFX.Clear();
        dashVFX.startWidth = startWidth;
        dashVFX.enabled = false;
        player.StartCoroutine("DelayedDashVFXStop");
        PlayerAnimator.SetBool("inDash", false);
        PlayerAnimator.SetTrigger("ToIdle");
        //rend.material.SetVector();
    }

    public bool isOffColldown()
    {
        bool cd = false;
        for (int i = numDashes - 1; i >= 0; i--)
        {
            if (cooldowns[i] > dashCD)
            {
                cd = true;
                break;
            }

        }

        return cd;
    }

    public void Cooldowns()
    {
        for (int i = 0; i < numDashes; i++)
        {
            if (cooldowns[i] > dashCD)
            {
                continue;
            }
            else
            {
                cooldowns[i] += Time.deltaTime;
                break;
            }
        }
    }


}
