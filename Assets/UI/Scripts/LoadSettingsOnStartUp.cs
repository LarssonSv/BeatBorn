using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class LoadSettingsOnStartUp : MonoBehaviour {
    private FMOD.Studio.Bus musicBus;
    private FMOD.Studio.Bus sfxBus;

    public int StartUpThreatLevel { get; set; }

    void Start() {
        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
        musicBus.setVolume(SaveManager.LoadSetting("MusicVolume"));
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfxBus.setVolume(SaveManager.LoadSetting("SoundEffectVolume"));
        QualitySettings.SetQualityLevel((int)SaveManager.LoadSetting("Quality"), true);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
        PlayerPrefs.SetInt("ThreatLevel", 0);
    }

    private void SceneLoaded(Scene arg0, LoadSceneMode arg1) {
        if(StartUpThreatLevel > 0) {
            StartCoroutine(StopIntroCinematic());
        }
    }

    private IEnumerator StopIntroCinematic() {
        yield return null;
        PlayableDirector director = FindObjectOfType<PlayableDirector>();
        if(director) {
            director.Stop();
        }
    }
}