#pragma warning disable 0649
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;
using XInputDotNetPure;

public class BeatManager : MonoBehaviour {
    [SerializeField] private AnimationCurve rumbleCurve;

    [EventRef]
    [SerializeField] private string CurrentSong;
    [EventRef]
    [SerializeField] private string Metronom;
    private EventInstance AudioEvent;
    private ParameterInstance intensityParam;
    [SerializeField] private string ParamName;
    [SerializeField] private float CombatIntensity;
    [SerializeField] private BeatData beatData;
    [Header("Beat accuracy window")]
    public static Action OnBeat;
    public static BeatManager Instance;

    private PLAYBACK_STATE playbackState;
    private double beatTimer = 0;
    bool settingUp;

    private float trackTime;
    //Cache
    private float rumbleForce = 0;
    private float one = 1f;
    
    private void Awake() {
        if(Instance != null) {
            Destroy(this);
        }
        Instance = this;
    }

    private string songName;

    void Start() {
        RoundController.OnRoundStart += CheckForNewSong;
        //beatData.Setup();
    }

    private void CheckForNewSong(Round round)
    {
        if(Instance.CurrentSong == round.Song)
            return;
        
        AudioEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
  
        Instance.CurrentSong = round.Song;
        Setup();
    }

    public static RhythmGrade GetGrade()
    {
        return Instance.beatData.GetGrade();
    }

    public void Setup()
    {
        beatTimer = 0f;
        trackTime = 0f;
        settingUp = true;
        AudioEvent = RuntimeManager.CreateInstance(CurrentSong);
        AudioEvent.start();
        if (AudioEvent.getParameter(ParamName, out intensityParam) != FMOD.RESULT.OK) {
            Debug.LogError("Didn't find music parameter on: " + ParamName);
        }
    }

    public static void Play()
    { 
        if(!Instance)
            return;
        
        Instance.AudioEvent.getPlaybackState(out PLAYBACK_STATE state);
        if(state != PLAYBACK_STATE.PLAYING)
            Instance.Setup();
    }

    void Update()
    {
        AudioEvent.getPlaybackState(out PLAYBACK_STATE state);
        if(state != PLAYBACK_STATE.PLAYING)
        {
            GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
            return;
        }
            
        
        beatTimer += Time.unscaledDeltaTime;
        trackTime += Time.unscaledDeltaTime;
        if (settingUp)
        {
            AudioEvent.getTimelinePosition(out int pos);
            if (pos < 2000)
                return;
            beatTimer = (pos - 2000) * 0.001f;
            trackTime = (float)beatTimer;
            settingUp = false;
        }
        
        AudioEvent.getPlaybackState(out playbackState);
        if (playbackState != PLAYBACK_STATE.PLAYING) return;
        

        if (beatTimer >= 0.5f)
        {
            Debug.Log("Beat!");
            OnBeat?.Invoke();   
            beatTimer -= 0.5f;
        }
       


       
        if(Input.GetKeyDown(KeyCode.F)) {
            SetIntensity(CombatIntensity + 0.1f);
        }

        rumbleForce = rumbleCurve.Evaluate((one - (float)BeatManager.OffBeatPercent()));
        GamePad.SetVibration(PlayerIndex.One, rumbleForce, rumbleForce);
    }

    public void SetIntensity(float intensity) {
        CombatIntensity = intensity;
        intensityParam.setValue(CombatIntensity);
    }

    public void ResetIntensity() {
        CombatIntensity = 0;
        intensityParam.setValue(CombatIntensity);
    }

    
    public static bool IsSetup()
    {
        return !Instance.settingUp;
    }

    public static double OffBeatPercent() {
        return Instance.beatTimer / 0.5d;
    }

    public static float GetTimeToQuarterNote(int quarterNote = 1)
    {
        return (Instance.trackTime % (0.5f * quarterNote) / (0.5f * quarterNote));
    }
    
    
    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, EventInstance instance, IntPtr parameterPtr) {
        if(type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT) {
            Debug.Log("0");
            OnBeat?.Invoke();
        }
        Debug.Log("1");
        return FMOD.RESULT.OK;
    }

    private void OnDestroy() {
        StopMusic();
    }

    public static void StopMusic() {
        Instance.AudioEvent.setUserData(IntPtr.Zero);
        Instance.AudioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Instance.AudioEvent.release();
    }

    public void OnDisable()
    {
        Instance = null;
    }
}