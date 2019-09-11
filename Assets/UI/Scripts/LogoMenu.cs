using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMOD;
using FMODUnity;

public class LogoMenu : MonoBehaviour
{
    [EventRef] public string LogoSFX;
    [EventRef] public string ConfirmSFX;
    FMOD.Studio.EventInstance LogoSfxEvent;
    void Start()
    {
        LogoSfxEvent = RuntimeManager.CreateInstance(LogoSFX);
        Invoke("StartSFX", 3.3f);
    }

    void StartSFX()
    {
        LogoSfxEvent.start();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            RuntimeManager.PlayOneShot(ConfirmSFX);
            SceneManager.LoadScene("MainMenu");
            LogoSfxEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            LogoSfxEvent.release();
        }
    }
}
