#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class DashVFXManager : MonoBehaviour
{
    [SerializeField] private float _dashEffectTime;
    [SerializeField] private AnimationCurve _dashCurve;

    private Renderer _renderer;
    private float _timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        PlayerDashState.OnDash += DoDashStuff;
    }

    private void Update()
    {
        if(_timer > 0)
        
        _renderer.material.SetFloat("_Blur",_dashCurve.Evaluate( 1- (_timer / _dashEffectTime)));

        _timer -= Time.deltaTime;
    }

    private void DoDashStuff()
    {
        _timer = _dashEffectTime;
    }

}
