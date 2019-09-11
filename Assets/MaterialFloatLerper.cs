#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class MaterialFloatLerper : MonoBehaviour
{
    [SerializeField] private string _paramaterName;
     [Header("Lerp in")]
    [SerializeField] private AnimationCurve _lerpInCurve;
    [SerializeField] private float _lerpInTime;
    [Header("Lerp out")]
    [SerializeField] private AnimationCurve _lerpOutCurve;
    [SerializeField] private float _lerpOutTime;

    private Renderer _renderer;
    private float _timer;
    private bool _lerping;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _timer = _lerpInTime + _lerpOutTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_lerping || PauseGame.IsPaused())
            return;

        float value = _timer <= _lerpInTime
            ? _lerpInCurve.Evaluate(_timer / _lerpInTime)
            : _lerpOutCurve.Evaluate((_timer - _lerpInTime) / _lerpOutTime);
        
        _renderer.material.SetFloat("_" + _paramaterName, value);

        _timer += Time.deltaTime;

        if (_timer >= _lerpInTime + _lerpOutTime)
        {
            _lerping = false;
            _timer = 0;
            _renderer.material.SetFloat("_" + _paramaterName, 0);
        }
            
    }

    [ContextMenu("Lerp")]
    public void LerpValue(bool instant = false)
    {
        if (instant || !_lerping)
        {
            _timer = 0;
            _lerping = true;
        }
         
    }
}
