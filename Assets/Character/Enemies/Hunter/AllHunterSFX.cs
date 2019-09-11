using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AllHunterSFX : MonoBehaviour
{
    [EventRef] public string TargetingSFX;
    [EventRef] public string HunterWalkSFX;
    [EventRef] public string HunterMeleeSFX;
    [EventRef] public string HunterRunSFX;

    public void Targeting()
    {
        RuntimeManager.PlayOneShotAttached(TargetingSFX, gameObject);
    }

    public void Melee()
    {
        RuntimeManager.PlayOneShotAttached(HunterMeleeSFX, gameObject);
    }

    public void Walk()
    {
        RuntimeManager.PlayOneShotAttached(HunterWalkSFX, gameObject);
    }

    public void HunterRun()
    {
        RuntimeManager.PlayOneShotAttached(HunterRunSFX, gameObject);
    }
}
