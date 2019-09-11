using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class BeatFeedbackSFX : MonoBehaviour
{
    [EventRef] public string OnBeatNoCombatSFX;
    [EventRef] public string OffBeatNoCombatSFX;

    FMOD.Studio.EventInstance musicSnap;
    FMOD.Studio.ParameterInstance Intense;


    private void Start()
    {
        PlayerAttackState.OnAttack += HappensOnAttack;
        musicSnap = FMODUnity.RuntimeManager.CreateInstance("snapshot:/TakeDamageSnap");
        musicSnap.getParameter("MusicSnapIn", out Intense);
        musicSnap.start();
    }


    private void HappensOnAttack(RhythmGrade grade)
    {   
        switch (grade)
        {
            case RhythmGrade.Miss:
                Intense.setValue(100f);
                RuntimeManager.PlayOneShot(OffBeatNoCombatSFX);
                Invoke("ResetSnap", 0.3f);
                break;
            case RhythmGrade.Good:
                RuntimeManager.PlayOneShot(OnBeatNoCombatSFX);
                break;
            case RhythmGrade.Great:
                RuntimeManager.PlayOneShot(OnBeatNoCombatSFX);
                break;
            case RhythmGrade.Excellent:
                RuntimeManager.PlayOneShot(OnBeatNoCombatSFX);
                break;
            case RhythmGrade.Super:
                RuntimeManager.PlayOneShot(OnBeatNoCombatSFX);
                break;
        }
    }

    private void ResetSnap()
    {
        Intense.setValue(0f);
    }
}
