using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

[CreateAssetMenu(menuName = "States/PlayerMovement/DeathState")]
public class PlayerDeathState : PlayerState
{
    [EventRef] public string PlayerDeathSFX;
    public AnimationClip clip;
    public string DeathAnimationTag;
    private float respawnTimer;
    private Renderer rend;

    FMOD.Studio.EventInstance deathSnap;
    FMOD.Studio.ParameterInstance snapIntensity;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        rend = player.transform.GetChild(0).Find("SM_Player_Retop").GetComponent<Renderer>();
        deathSnap = FMODUnity.RuntimeManager.CreateInstance("snapshot:/PlayerDeathSnap");
        deathSnap.getParameter("DeathSnap", out snapIntensity);
        deathSnap.start();
    }

    public override void Enter()
    {
        RuntimeManager.PlayOneShotAttached(PlayerDeathSFX, player.gameObject);
        snapIntensity.setValue(100f);
        PlayerAnimator.SetFloat("Velocity", 0);
        PlayerAnimator.SetTrigger("Death");
        Velocity = Vector3.zero;
        PlayerAnimator.SetTrigger(DeathAnimationTag);
        respawnTimer = 0f;
        player.OnPlayerDeath?.Invoke();
        StreakCounterUI.CollectDeath();
        
    }

    public override void StateUpdate()
    {
        rend.material.SetFloat("Vector1_6DD9F6DE", (1 - (respawnTimer/clip.length)));


        if (respawnTimer > clip.length)
        {
            TransitionTo<PlayerSpawningState>();
            snapIntensity.setValue(0f);
            StreakCounterUI.DeathResetStreak();
            SyncPowerManager.DeathResetSync();
        }
        respawnTimer += Time.deltaTime;

    }
    
}
