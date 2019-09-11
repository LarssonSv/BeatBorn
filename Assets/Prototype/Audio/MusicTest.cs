using FMOD.Studio;
using FMODUnity;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class MusicTest : MonoBehaviour {
    [EventRef]
    [SerializeField] private string Event = "event:/Music/Wave1Music";
    private EventInstance AudioEvent;
    private ParameterInstance intensityParam;
    [SerializeField] private string ParamName = "ComboIntensity";
    [SerializeField] private float CombatIntensity;

    [Range(0f, 1f), SerializeField] private float beatAccuracyWindow = 0.4f;
    private float timeSinceLastBeat = 0;
    private float timeLastBeatHappenedGlobal = 0;
    private bool canBeat = false;

    private static Action OnBeat;

    private static MusicTest instance;

    private void Awake() {
        instance = this;
    }

    void Start() {
        AudioEvent = RuntimeManager.CreateInstance(Event);
        AudioEvent.setCallback(new EVENT_CALLBACK(BeatEventCallback), EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        AudioEvent.start();

        if(AudioEvent.getParameter(ParamName, out intensityParam) != FMOD.RESULT.OK) {
            Debug.LogError("Didn't find music parameter on: " + ParamName);
        }

        OnBeat += BeatHappened;
        timeLastBeatHappenedGlobal = Time.time;
    }

    void Update() {
        //if(Input.GetKeyDown(KeyCode.M)) {
        //    ButtonPressed();
        //}
        //if(Input.GetKeyDown(KeyCode.F)) {
        //    AddIntensity(0.1f);
        //}
    }

    private void AddIntensity(float intensity) {
        CombatIntensity += intensity;
        intensityParam.setValue(CombatIntensity);
    }

    public void ButtonPressed() {
        if(!canBeat) {
            return;
        }
        float t = Time.time - timeLastBeatHappenedGlobal;
        float diff = t / timeSinceLastBeat;
        if(diff <= beatAccuracyWindow) {
            canBeat = false;
            print("Success!");
        } else {
            print("Time between beats: " + timeSinceLastBeat + ". Procent to succeed: " + beatAccuracyWindow + ". Off by: " + OffBeatProcent() + " procent");
        }
    }

    private void BeatHappened() {
        timeSinceLastBeat = Time.time - timeLastBeatHappenedGlobal;
        timeLastBeatHappenedGlobal = Time.time;
        canBeat = true;
    }

    private float OffBeat() {
        float t = Time.time - timeLastBeatHappenedGlobal;
        float diff = t / timeSinceLastBeat;
        return timeSinceLastBeat * diff;

    }

    private float OffBeatProcent() {
        float t = Time.time - timeLastBeatHappenedGlobal;
        float diff = t / timeSinceLastBeat;
        float result = diff - beatAccuracyWindow;
        return result;
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    private static FMOD.RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, EventInstance instance, IntPtr parameterPtr) {
        //IntPtr timelineInfoPtr;
        //FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
        //if(result != FMOD.RESULT.OK) {
        //    Debug.LogError("Timeline Callback error: " + result);
        //    return result;
        //}
        if(type == EVENT_CALLBACK_TYPE.TIMELINE_BEAT) {
            //TIMELINE_MARKER_PROPERTIES parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));

            //GCHandle gch = GCHandle.Alloc(parameter.name);
            //IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(gch.ToString().ToCharArray(), 0);
            //int nameLen = 0;
            //while(Marshal.ReadByte(ptr, nameLen) != 0) ++nameLen;
            //byte[] buffer = new byte[nameLen];
            //Marshal.Copy(ptr, buffer, 0, buffer.Length);
            //string name = Encoding.UTF8.GetString(buffer, 0, nameLen);

            print("Beat happened");

            OnBeat?.Invoke();
            //gch.Free();
        }
        return FMOD.RESULT.OK;
    }

    private void OnDestroy() {
        AudioEvent.setUserData(IntPtr.Zero);
        AudioEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioEvent.release();

        OnBeat -= BeatHappened;
    }
}