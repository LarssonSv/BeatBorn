#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class EmissionBurster : MonoBehaviour
{
    [SerializeField] private float _maxEmission;
    [SerializeField] private float _decaySpeed;
    private float _minEmission;
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _particleEmission;
    
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleEmission = _particleSystem.emission;
        _minEmission = _particleEmission.rateOverTimeMultiplier;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Burst();
        
        if (_particleEmission.rateOverTimeMultiplier > 1f)
        {
            _particleEmission.rateOverTimeMultiplier = Mathf.Clamp(_particleEmission.rateOverTimeMultiplier - ( Time.deltaTime * _decaySpeed), _minEmission, _maxEmission) ;
        }
    }
    
    public void Burst(float emissionAmount = 100)
    {
        _particleEmission.rateOverTimeMultiplier += emissionAmount;
    }
    

}
