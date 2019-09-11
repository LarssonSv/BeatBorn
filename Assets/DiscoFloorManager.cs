using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscoFloorManager : MonoBehaviour
{
    public Material[] _floorRenderers;
    public float Speed;
    [Header("Damage")]
    public AnimationCurve LerpInCurve;
    public float LerpInTime;
    public AnimationCurve LerpOutCurve;
    public float LerpOutTime;

    float time = 0;

    private Color emissiveStartColor;
    
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Material material in _floorRenderers)
        {
            material.SetFloat("_Speed", 0);
            material.SetFloat("_Speed2", 0);
            material.SetFloat("_CoreDamage", 0f);
        }

        CoreHealth.Instance.OnTakingDamage += DoDamgeStuff;
        BeatManager.OnBeat += SetSpeed;
    }


    
    private void DoDamgeStuff()
    {
        if(time == 0)
            StartCoroutine(DamgeLerp());
    }

    private IEnumerator DamgeLerp()
    {
        time = 0;

        while (time < LerpInTime)
        {
            foreach (Material material in _floorRenderers)
                material.SetFloat("_CoreDamage", LerpInCurve.Evaluate(time / LerpInTime));
            

            time += Time.deltaTime;
            yield return null;
        }

        time = LerpOutTime;
        
        while (time > 0)
        {
            foreach (Material material in _floorRenderers)
                material.SetFloat("_CoreDamage", LerpOutCurve.Evaluate(time / LerpOutTime));
            

            time -= Time.deltaTime;
            yield return null;
        }
        
        foreach (Material material in _floorRenderers)
            material.SetFloat("_CoreDamage", 0f);
        time = 0;
    }
    
    private void SetSpeed()
    {
        foreach (Material material in _floorRenderers)
        {
              material.SetFloat("_Speed",  Speed);
              material.SetFloat("_Speed2",  Speed);
        }
          
        BeatManager.OnBeat -= SetSpeed;
    }

    private void OnDisable()
    {
        foreach (Material material in _floorRenderers)
        {
            material.SetFloat("_CoreDamage", 0);
            material.SetFloat("_Speed", 1);
            material.SetFloat("_Speed2", 1);
        }
  
    }
}
