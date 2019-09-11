using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXGradeLerp : MonoBehaviour
{
    [SerializeField] private string _valueName;
    [SerializeField] private bool _allButThis;
    [SerializeField] private RhythmGrade _grade;

    [SerializeField] private float _lerpInTime;
    [SerializeField] private AnimationCurve _lerpInCurve;
    [SerializeField] private float _lerpOutTime;
    [SerializeField] private AnimationCurve _lerpOutCurve;

    private Renderer _renderer;
    private float _timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        PlayerAttackState.OnAttack += ShowFeedback;
    }

    private void ShowFeedback(RhythmGrade grade)
    {
        StopCoroutine(LerpValue());
        _renderer.material.SetFloat(_valueName, 0);
        
        if(_grade != grade && ! _allButThis || _grade == grade && _allButThis)
            return;
        
     
        StartCoroutine(LerpValue());

    }

    private IEnumerator LerpValue()
    {
        _timer = 0;

        while (_timer <= _lerpInTime)
        {
            _renderer.material.SetFloat(_valueName, _lerpInCurve.Evaluate(_timer / _lerpInTime));
            _timer += Time.deltaTime;
            yield return null;
        }

        _timer = 0;

        while (_timer <= _lerpOutTime)
        {
            _renderer.material.SetFloat(_valueName, _lerpOutCurve.Evaluate(_timer / _lerpOutTime));
            _timer += Time.deltaTime;
            yield return null;
        }
        
        _renderer.material.SetFloat(_valueName, 0);

        
    }
    
}
