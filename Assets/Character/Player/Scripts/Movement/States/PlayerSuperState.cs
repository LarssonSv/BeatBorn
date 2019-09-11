using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEditor.VFX;
using UnityEngine.Experimental.Animations;
using UnityEngine.Experimental.VFX;


[CreateAssetMenu(menuName = "States/PlayerMovement/Super")]
public class PlayerSuperState : PlayerState
{
    [System.Serializable]
    public struct SuperStruct
    {
        [Header("Animation"), Tooltip("This is the name of the animation state in the animator and the name of the trigger as well")]
        public string AnimationTag;
        [Tooltip("This clip determines the time for a animation")] public AnimationClip Clip;

        [Header("Attack")]
        [Range(0f, 1f)] public float SyncPercentage;
        public int Damage;
        public float AttackTime;
        public Vector2 minMaxRadius;
        public LayerMask mask;

        [Header("Feedback")]
        public VFXStruct HitParticle;
        public GameObject ShurikenPrefab;

        [Header("Sound")]
        [EventRef] public string AttackSFX;
        [EventRef] public string HitSFX;
    }

    [SerializeField] private string vfxTag = "VFXUlti";
    [SerializeField] private LayerMask mask;
    [SerializeField] private float radius;
    [SerializeField] public SuperStruct[] Supers;

    private SuperStruct currentSuper => Supers[index];

    private List<GameObject> toBeDisabled = new List<GameObject>();
    private int index;
    private int skipBeats = 0;
    private Collider[] toDamage;

    FMOD.Studio.EventInstance ultSnap;
    FMOD.Studio.ParameterInstance ultSnapParam;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        ultSnap = FMODUnity.RuntimeManager.CreateInstance("snapshot:/UltSnap");
        ultSnap.getParameter("UltSnapParam", out ultSnapParam);
        ultSnap.start();
    }



    public override void Enter()
    {
        skipBeats = 0;
        CameraController.Instance.ChangeStateToIdle();
        toBeDisabled.Clear();
        BeatManager.OnBeat += EnterOnBeat;
        for (int i = Supers.Length - 1; i <= 0; i--)
        {
            if (SyncPowerManager.CurrentSyncProcent() >= Supers[i].SyncPercentage)
            {
                index = i;
                SyncPowerManager.UseSyncPower(Supers[i].SyncPercentage);
                break;
            }
        }
        Velocity = Vector3.zero;
    }

    public override void StateUpdate()
    {

    }

    private void EnterOnBeat()
    {
        PlayerAnimator.SetTrigger(currentSuper.AnimationTag);
        ultSnapParam.setValue(100f);
        RuntimeManager.PlayOneShot(currentSuper.AttackSFX);
        toDamage = Physics.OverlapSphere(player.transform.position, radius, mask);
        BeatManager.OnBeat -= EnterOnBeat;
        BeatManager.OnBeat += DoDamage;

        foreach (Collider x in toDamage)
        {
            AiController brain = x.transform.root.GetComponent<AiController>();
            if (brain?.EnemyType == Enemy.basicAi)
            {
                brain.Machine.GetState<AiKnockbackState>().TriggerHeavyKnockBack = true;
                brain.Machine.TransitionTo<AiKnockbackState>();
            }
            toBeDisabled.Add(ObjectPooler.Instance.SpawnFromPool(vfxTag, x.transform.root.position, x.transform.root.rotation, x.transform.root));
        }

    }


    private void DoDamage()
    {
        if (skipBeats < 2)
        {
            if(skipBeats == 1)
            {
                RuntimeManager.PlayOneShot(currentSuper.HitSFX);
                ultSnapParam.setValue(0f);
                foreach (Collider x in toDamage)
                {
                    x.transform.root.GetComponent<IDamageable>().TakeDamage(currentSuper.Damage);
                }
            }
            skipBeats++;

            return;
        }
            

        
        BeatManager.OnBeat -= DoDamage;
        TransitionTo<PlayerIdleState>();

    }

    public override void Exit()
    {
        player.StartCoroutine(DelayedDestruction());

    }

    IEnumerator DelayedDestruction()
    {
        yield return new WaitForSeconds(3f);
        foreach (GameObject y in toBeDisabled)
        {
            y.transform.parent = null;
            y.SetActive(false);
        }

    }


}
