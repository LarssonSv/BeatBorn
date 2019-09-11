#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class VFXSizePulseToMusic : MonoBehaviour
{
    [SerializeField] private AnimationCurve _sizeCurve;
    [SerializeField] private int _quarterNote = 2;
    private ParticleSystem _particleSystem;
    private ParticleSystem.SizeOverLifetimeModule _sizeOverLifetime;
    
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
         _sizeOverLifetime = _particleSystem.sizeOverLifetime;
    }

    // Update is called once per frame
    void Update()
    {
        _sizeOverLifetime.sizeMultiplier  = _sizeCurve.Evaluate(BeatManager.GetTimeToQuarterNote(_quarterNote));
    }
}
