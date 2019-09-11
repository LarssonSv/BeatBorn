#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class SyncPowerManager : MonoBehaviour {
    private BeatManager beatManager;
    [SerializeField] private float maxSyncPower = 10;
    [SerializeField] private float syncPowerPerDamage = 0.5f;
    [SerializeField] private float missPenalty = 5;
    [SerializeField] private float syncDecayPerSecond = 0.5f;
    [SerializeField] private float preDecayTime = 0.5f;
    [SerializeField] private AnimationCurve syncCurve;
    [SerializeField] private VisualEffect vfxSyncGain;

    private float currentSyncAmount = 0;
    private static SyncPowerManager instance;
    public static Action<float> OnSyncPowerUpdate;
    private float lastRhythm;

    private void Awake() {
        if(instance) {
            Destroy(this);
        }
        instance = this;
    }

    private void Start() {
        PlayerEngine pe = FindObjectOfType<PlayerEngine>();
        beatManager = BeatManager.Instance;
    }

    private void Update()
    {
        if(currentSyncAmount <= 0 || Time.time - lastRhythm <= preDecayTime)
        currentSyncAmount = Mathf.Clamp(currentSyncAmount - Time.deltaTime * syncDecayPerSecond, ((int)(currentSyncAmount / (maxSyncPower /4)) * (maxSyncPower / 4)), maxSyncPower);
        OnSyncPowerUpdate?.Invoke(CurrentSyncProcent());
    }

    public static void AddSyncPower(float damage) {
        instance.lastRhythm = Time.time;
        float newSyncpower = instance.currentSyncAmount;
        newSyncpower += damage * instance.syncPowerPerDamage *
                        instance.syncCurve.Evaluate(instance.currentSyncAmount / instance.maxSyncPower);
        instance.currentSyncAmount = Mathf.Clamp(newSyncpower, 0, instance.maxSyncPower);
        OnSyncPowerUpdate?.Invoke(CurrentSyncProcent());
        instance.AddMusicIntensity();
        if(instance.vfxSyncGain && PlayerAttackState.currentAttackGrade != RhythmGrade.Miss) {
            instance.vfxSyncGain.SendEvent("OnBeat");
        }
    }

    public static void KeepTheBeatGoing()
    {
        instance.lastRhythm = Time.time;
    }
    
    private void AddMusicIntensity() {
        beatManager.SetIntensity(currentSyncAmount / maxSyncPower);
    }

    public static float CurrentSyncAmount() {
        return instance.currentSyncAmount;
    }

    public static float CurrentSyncProcent() {
        return instance.currentSyncAmount / instance.maxSyncPower;
    }

    public static float MaxSyncPower() {
        return instance.maxSyncPower;
    }

    public static bool IsSyncMeterFull() {
        return instance.currentSyncAmount >= instance.maxSyncPower;
    }

    public static void UseSyncPower(float percent) {
        instance.currentSyncAmount -= instance.maxSyncPower * percent;
        Debug.Log("The sync to remove: " + instance.maxSyncPower * percent);
        OnSyncPowerUpdate?.Invoke(CurrentSyncProcent());
        instance.beatManager.SetIntensity(instance.currentSyncAmount / instance.maxSyncPower);
    }

    public static void SetSyncPower(float x)
    {
        instance.currentSyncAmount = x;
    }

    public static void Miss()
    {
        instance.lastRhythm = 0;
        instance.currentSyncAmount = Mathf.Clamp(instance.currentSyncAmount - instance.missPenalty, ((int)(instance.currentSyncAmount / (instance.maxSyncPower / 4)) * (instance.maxSyncPower / 4)), instance.maxSyncPower);
    }

    public static void DeathResetSync()
    {
        instance.currentSyncAmount = instance.currentSyncAmount / 2;
        instance.beatManager.SetIntensity(0f);
    }
}