using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class SpawnIndicator : MonoBehaviour
{
    [SerializeField] private float activeTime = 10f;
    [SerializeField] private float lerpColorTime = 10f;
    [SerializeField] private AnimationCurve colorCurve;
    private Renderer rend;
    private Renderer SpawnGlow;
    private float startOpacity;
    private float startDoorColor;
    private bool playedSound = false;
    [EventRef] [SerializeField] private string doorActiveSound;

    float timer = 0;
    void Start()
    {
        rend = GetComponent<Renderer>();
        SpawnGlow = transform.GetChild(0).GetComponent<Renderer>();
        startDoorColor = rend.material.GetFloat("_GateGlow");
        startOpacity = SpawnGlow.material.GetFloat("_Opacity");
        SpawnGlow.material.SetFloat("_Opacity", 0);
    }

    public void StartIndication()
    {
        StopCoroutine(StartTimer());
        StartCoroutine(StartTimer());
    }


    IEnumerator StartTimer()
    {
        if(!rend)
        {
            Debug.Log("NO enemy-spawn shader set");
            yield break;
        }

        timer = 0;
        float lerpValue = 0;
        while (timer <= lerpColorTime)
        {
            lerpValue = colorCurve.Evaluate(timer / lerpColorTime);
            rend.material.SetFloat("_GateGlow", Mathf.Lerp(startDoorColor, 0, lerpValue));
            SpawnGlow.material.SetFloat("_Opacity", Mathf.Lerp(0, startOpacity, lerpValue));

            timer += Time.deltaTime;
            yield return null;
        }

        if (!playedSound)
        {
            BeatManager.OnBeat += PlaySound;
        }
       

        yield return new WaitForSeconds(activeTime);



        if (rend)
        {
            rend.material.SetFloat("_GateGlow", startDoorColor);
            SpawnGlow.material.SetFloat("_Opacity", 0f);
        }
        playedSound = false;
    }


    public void PlaySound()
    {
        RuntimeManager.PlayOneShotAttached(doorActiveSound, gameObject);
        playedSound = true;
        BeatManager.OnBeat -= PlaySound;
    }

}
