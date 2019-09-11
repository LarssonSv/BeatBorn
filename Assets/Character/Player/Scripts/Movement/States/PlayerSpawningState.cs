#pragma warning disable 0649

using UnityEngine;
using FMOD;
using FMODUnity;
using Debug = UnityEngine.Debug;

[CreateAssetMenu(menuName = "States/PlayerMovement/Spawning")]
public class PlayerSpawningState : PlayerState
{
    [EventRef] [SerializeField] private string spawnSound;
    [SerializeField] private AnimationClip clip;
    [SerializeField] private GameObject spawnVFX;
    [SerializeField] private float preSpawnDelay;
    [SerializeField] private float preVisualSpawnDelay;
    [SerializeField] private float dissolveSpeed;
    private Renderer rend;
    private Renderer swordRend;
    private Renderer ponyTailRend;
    private bool doingPreDelay;
    private float timer;
    private GameObject tempVFX;
    private float time;

    private float preSpawnVisualTimeHolder;
    private float visualSpawnTotalTime;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        rend = player.transform.GetChild(0).Find("SM_Player_Retop").GetComponent<Renderer>();
        swordRend = SwordTransform.GetComponent<Renderer>();
        ponyTailRend = player.transform.GetChild(0).Find("head").GetComponentInChildren<Renderer>();

        visualSpawnTotalTime = preSpawnDelay - preVisualSpawnDelay + clip.length;
    }

    public override void Enter()
    {
        rend.material.SetFloat("_Dissolve", 0);
        swordRend.material.SetFloat("_Dissolve", 0);
        ponyTailRend.material.SetFloat("_Dissolve", 0);
        Respawn();
        timer = 0;
        preSpawnVisualTimeHolder = 0;
        if (tempVFX == null)
            tempVFX = Instantiate(spawnVFX);
        
        tempVFX.gameObject.SetActive(true);
        tempVFX.transform.position = Position;
        PlayerAnimator.SetTrigger("Spawning");
        PlayerAnimator.speed = 0;
        player.OnPlayerRespawn?.Invoke();
        player.CurrentHP = player.MaxHP;
        doingPreDelay = true;
        time = player.InvincibilityTime;
        player.InvincibilityTime = 40000f;

    }

    public override void StateUpdate()
    {
        if(timer >= preVisualSpawnDelay || !doingPreDelay) {
            preSpawnVisualTimeHolder += Time.deltaTime;
            rend.material.SetFloat("_Dissolve", preSpawnVisualTimeHolder / visualSpawnTotalTime);
            swordRend.material.SetFloat("_Dissolve", preSpawnVisualTimeHolder / visualSpawnTotalTime);
            ponyTailRend.material.SetFloat("_Dissolve", preSpawnVisualTimeHolder / visualSpawnTotalTime);
        }

        if (doingPreDelay)
        {
            timer += Time.deltaTime;

            if (timer >= preSpawnDelay)
            {
                timer = 0;
                doingPreDelay = false;
                PlayerAnimator.speed = 1;
                RuntimeManager.PlayOneShot(spawnSound);
            }
            else
                return;
        }

        if(timer > clip.length)
            TransitionTo<PlayerIdleState>();

        rend.material.SetFloat("Vector1_6DD9F6DE", timer/clip.length);


        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        tempVFX.gameObject.SetActive(false);
        rend.material.SetFloat("Vector1_6DD9F6DE", 1f);
        PlayerAnimator.SetTrigger("ToIdle");
        player.InvincibilityTime = time;

    }

    public void Respawn()
    {
        if (!player.SpawnPoint)
            player.SpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");
       
        Vector3 point = player.SpawnPoint.transform.position;
 
        if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 100f))
            point.y = hit.point.y + 0.1f;
        
        player.transform.position = point;
        player.OnPlayerRespawn?.Invoke();
    }


}
