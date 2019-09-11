#pragma warning disable 0649
using FMODUnity;
using UnityEngine;

public class PauseGame : MonoBehaviour {
    [EventRef] public string PauseSFX;

    private static bool paused = false;
    public static PauseGame Instance;


    public bool canPause = true;

    private FMOD.Studio.Bus musicBus;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MyMaster");
    }

    private void Update() {
        if(Input.GetButtonDown("Pause")) {
            if(!paused && canPause) {
                Pause();
            } else {
                UnPause();
            }
        }
    }

    public static void Pause() {
        if(MenuManager.Instance) {
            MenuManager.Instance.Create<PauseMenu>();
            MenuManager.Instance.OnAllMenusClosed += UnPause;
        }
        //RuntimeManager.PlayOneShot(Instance.PauseSFX);
        paused = true;
        Time.timeScale = 0;
        Instance.musicBus.setVolume(0f);

    }

    public static bool IsPaused() {
        return paused;
    }

    public static void UnPause() {
        Time.timeScale = 1;
        paused = false;
        //BeatManager.PauseMusic(false);
        if(MenuManager.Instance) {
            MenuManager.Instance.CloseAllMenus();
        }
        Instance.musicBus.setVolume(1f);
    }
}