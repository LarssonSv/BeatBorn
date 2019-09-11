using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSync : MonoBehaviour
{
    [Header("Emission")]
    [SerializeField] private AnimationCurve _emissionCurve;
    [SerializeField] private float _maxEmission;
    [SerializeField] private float _minEmission;

    [Header("Hit or miss")] 
    [SerializeField] private float _showTime = 0.3f;
    [SerializeField] private ParticleSystem.MinMaxGradient _missColor;
    [SerializeField] private ParticleSystem.MinMaxGradient _goodColor;
    [SerializeField] private ParticleSystem.MinMaxGradient _greatColor;
    [SerializeField] private ParticleSystem.MinMaxGradient _exelentColor;
    [SerializeField] private ParticleSystem.MinMaxGradient _superColor;
    
    private ParticleSystem _particleSystem;
    private ParticleSystem.EmissionModule _particleEmission;
    private ParticleSystem.ColorOverLifetimeModule _particleColor;
    
    private ParticleSystem.MinMaxGradient _defaultColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleEmission = _particleSystem.emission;
        _particleColor = _particleSystem.colorOverLifetime;
        SyncPowerManager.OnSyncPowerUpdate += SyncUpdate;
        PlayerAttackState.OnAttack += HitFeedback;
        _defaultColor = _particleColor.color;

    }

    private void SyncUpdate(float syncPercentage)
    {
        _particleEmission.rateOverTimeMultiplier = Mathf.Lerp( _minEmission, _maxEmission,_emissionCurve.Evaluate(syncPercentage));
    }


    private void HitFeedback(RhythmGrade grade)
    {
        switch (grade)
        {
            case RhythmGrade.Miss:
                _particleColor.color = _missColor;
                break;
            case RhythmGrade.Good:
                _particleColor.color = _goodColor;
                break;
            case RhythmGrade.Great:
                _particleColor.color = _greatColor;
                break;
            case RhythmGrade.Excellent:
                _particleColor.color = _exelentColor;
                break;
            case RhythmGrade.Super:
                _particleColor.color = _superColor;
                break;
        }
        
        StopCoroutine(ChangeColor());
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(_showTime);
        _particleColor.color = _defaultColor;
    }


}
