using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using FMOD;
using FMODUnity;

public class EpilepsyScreen : MonoBehaviour
{
    [SerializeField] private float showTime = 4.0f;
    [SerializeField] private float fadeTime = 3.0f;
    private float timeHolder;
    private float timeHolder2;
    private bool IsPlayed = false;
    private bool IsPlayed2 = false;
    [SerializeField] private Canvas epilepsyCanvas;
    [SerializeField] private Canvas logoCanvas;
    [SerializeField] private PlayableDirector fadeIn;
    [SerializeField] private PlayableDirector fadeOut;
    [EventRef] public string DuckSFX;

    private void Start() {
        timeHolder = Time.time + showTime;
        timeHolder2 = Time.time + fadeTime;
        RuntimeManager.PlayOneShot(DuckSFX);
    }

    void Update()
    {
        if (timeHolder2 < Time.time && !IsPlayed2) {
            fadeOut.Play();
            IsPlayed2 = false;
        }
        if(timeHolder < Time.time && !IsPlayed) {
            epilepsyCanvas.enabled = false;
            logoCanvas.enabled = true;
            fadeIn.Play();
            IsPlayed = true;
        }
    }
}
