#pragma warning disable 0649
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class CoreHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 10;
    [SerializeField] private float TimeForCoreDamageLoop = 0.6f;
    [EventRef] public string CoreDrainSFX;
    [SerializeField] private EventInstance DrainCoreEvent;
    [SerializeField] private ParameterInstance LoopingPrameter;
    [SerializeField] private Material emissiveMat;
    [SerializeField] private Material[] coreFloors;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private GameObject deathDestroyCoreObject;
    [SerializeField] private GameObject CoreAmbHolder;

    readonly private string blendCache = "_blendDamage";
    readonly private string colorCache = "Color_9CC22BDB";

    private int currentHealth;
    private bool isDead = false;
    private float lastTimeOfDamage;
    private bool playingSound;
    private MaterialFloatLerper[] materialLerpers;
    private Animator coreAnimator;

    [SerializeField] private float cameraShakeIntensity = 0.1f;
    [SerializeField] private float cameraShakeTime = 0.1f;

    private MaterialFloatLerper matLerper;
    private static CoreHealth instanceHolder;
    public static CoreHealth Instance
    {
        get
        {
            if (!instanceHolder)
                instanceHolder = FindObjectOfType<CoreHealth>();

            if (!instanceHolder)
                Debug.LogWarning("There is no Nexus health script in the scene");

            return instanceHolder;
        }
    }

    public Action coreDied;
    public Action OnTakingDamage;
    private void Start()
    {
     
        coreAnimator = GetComponentInChildren<Animator>();
        materialLerpers = GetComponentsInChildren<MaterialFloatLerper>();

        currentHealth = health;
        if (!DrainCoreEvent.isValid())
        {
            DrainCoreEvent = RuntimeManager.CreateInstance(CoreDrainSFX);
            if (DrainCoreEvent.getParameter("isDraining", out LoopingPrameter) != FMOD.RESULT.OK)
            {
                Debug.LogError("Didn't find music parameter on: " + LoopingPrameter);
            }
        }
        emissiveMat.SetFloat("_CoreDamage", 0f);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Die();
        }
#endif

        if (!playingSound)
            return;

        if (Time.time - lastTimeOfDamage > TimeForCoreDamageLoop)
        {
            LoopingPrameter.setValue(0);
            playingSound = false;
        }
    }

    public void TakeDamage(int damage = 1, RhythmGrade grade = RhythmGrade.Miss, Transform attacker = null, float MissMultiplier = 1)
    {
        currentHealth -= damage;
        lastTimeOfDamage = Time.time;
        HUDManager.Instance.UpdateNexusHPUI(health, currentHealth);
        OnTakingDamage?.Invoke();
        if (materialLerpers != null)
        {
            foreach (MaterialFloatLerper lerper in materialLerpers)
                lerper.LerpValue();
        }

        StopCoroutine(ChangeColor());
        StartCoroutine(ChangeColor());

        if (!playingSound)
        {
            DrainCoreEvent.start();
            playingSound = true;
        }
        coreAnimator.SetTrigger("TookDamage");
        coreAnimator.SetFloat("CoreHealth", 1 - (float)currentHealth / health);
        LoopingPrameter.setValue(1);

        CameraShake.Shake(cameraShakeIntensity, cameraShakeTime);

        if (currentHealth <= 0)
        {
            Die();
            isDead = true;
        }
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        coreDied?.Invoke();
        coreAnimator.SetTrigger("Died");
        Instantiate(deathVFX, transform.position + new Vector3(0f, 7.5f, 0f), transform.rotation, transform);
        deathDestroyCoreObject.SetActive(false);
        Destroy(CoreAmbHolder);

        AiController[] temp = FindObjectsOfType<AiController>();
        foreach (AiController x in temp)
        {
            x.gameObject.SetActive(false);
        }

    }

    IEnumerator ChangeColor()
    {
        emissiveMat.SetFloat("_CoreDamage", 1f);
        yield return new WaitForSeconds(1.5f);
        emissiveMat.SetFloat("_CoreDamage", 0f);

    }

    private void OnDisable()
    {
        emissiveMat.SetFloat("_CoreDamage", 0f);
    }
}