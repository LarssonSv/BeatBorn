#pragma warning disable 0649
using UnityEngine;
using StateKraft;
using FMODUnity;
//Author: Simon

[CreateAssetMenu(menuName = "States/Ai/Death")]
public class AiDeath : AiState
{
    [SerializeField] private AnimationClip clip;
    [SerializeField] private string[] deathAnimationTags;
    [SerializeField] private float vfxLerpSlowdown = 2f;
    [SerializeField, Range(0f, 5f)] float deathDelay = 3f;
    [SerializeField] private int deathLayer;
    [SerializeField] private int aliveLayer;
    [SerializeField] private int ignoreLayer;
    [EventRef]
    [SerializeField] private string DeathSFX;
    
    private float timer;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
    }

    public override void Enter()
    {
        RuntimeManager.PlayOneShot(DeathSFX, Ai.transform.position);
        Anime.ResetTrigger("Idle");
        Anime.ResetTrigger("Attack");
        Anime.ResetTrigger("HurtRight");
        Anime.SetTrigger(deathAnimationTags[Random.Range(0 ,deathAnimationTags.Length)]);
        Ai.OnDeath?.Invoke();
        Ai.OnDeath = null;
        timer = 0;
        SetColliderRecursive(Ai.gameObject, deathLayer);
    }

    private void SetColliderRecursive(GameObject obj, int newLayer)
    {
        if (obj.layer == ignoreLayer) return;

        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetColliderRecursive(child.gameObject, newLayer);
        }
    }

    public override void StateUpdate()
    {
        Ai.Renderer.material.SetFloat("_Dissolve", (1 - (timer / vfxLerpSlowdown)));

        if (timer > deathDelay)
        {
            Ai.gameObject.SetActive(false);
            
        }

        timer += Time.deltaTime;
    }

    public override void Exit()
    {
        Ai.Renderer.material.SetFloat("_Dissolve", 1f);
        SetColliderRecursive(Ai.gameObject, aliveLayer);
        
        foreach(string tag in deathAnimationTags)
        {
            Anime.ResetTrigger(tag);
        }
    }
}
