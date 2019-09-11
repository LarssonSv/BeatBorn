#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class VFXBeat : MonoBehaviour
{
    [SerializeField] private float _missLerpTime;
    [SerializeField] private float _onBeatLerpTime;
    [SerializeField] private AnimationCurve _missIntencityCurve;
    [SerializeField] private AnimationCurve _onBeatCurve;
    [SerializeField] private AnimationCurve _beatCurve;

    private VisualEffect _vfx;
    // Start is called before the first frame update
    void Start()
    {
        _vfx = GetComponent<VisualEffect>();
        PlayerAttackState.OnAttack += DoPower;
    }

    private void Update()
    {
        _vfx.SetFloat("BeatTime", _beatCurve.Evaluate(BeatManager.GetTimeToQuarterNote()));
        _vfx.SetFloat("Sync Intensity/Performance", StreakCounterUI.GetCurrentStreakPercentage());
    }

    private void DoPower(RhythmGrade grade)
    {
        if (grade == RhythmGrade.Miss)
        {
            StopCoroutine(MissLerp());
            StartCoroutine(MissLerp());
        }
        else
        {
            StopCoroutine(BeatLerp());
            StartCoroutine(BeatLerp());
        }
    }



    private IEnumerator BeatLerp()
    {
        float time = 0;

        while (time < _onBeatLerpTime)
        {
            _vfx.SetFloat("Blend - OnBeat", _onBeatCurve.Evaluate(time / _onBeatLerpTime));
            time += Time.deltaTime;
            yield return null;
        }
        _vfx.SetFloat("Blend - OnBeat", 0);
    }

    private IEnumerator MissLerp()
    {
        float time = 0;

        while (time < _missLerpTime)
        {
            _vfx.SetFloat("Blend - Offbeat", _missIntencityCurve.Evaluate(time / _missLerpTime));
            time += Time.deltaTime;
            yield return null;
        }
        _vfx.SetFloat("Blend - Offbeat", 0);
    }
}
