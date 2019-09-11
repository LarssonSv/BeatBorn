#pragma warning disable 0649
using System;
using UnityEngine;
using UnityEngine.Experimental.VFX;
[Serializable]
public struct VFXStruct 
{
    [SerializeField] private VisualEffectAsset VFXGraph;
    [SerializeField] private GameObject VFXPrefab;
    [SerializeField] private GameObject ShurikenPrefab;

    public void Play(Vector3 posistion, Quaternion rotation, Transform parent = null)
    {   
        if(VFXGraph)
            ParticlePool.SpawnOneShot(VFXPrefab, VFXGraph, posistion, rotation, parent);
        if(ShurikenPrefab)
            ParticlePool.SpawnOneShot(ShurikenPrefab, posistion, rotation, parent);
    }
}
