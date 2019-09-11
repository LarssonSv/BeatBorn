using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class RunSFX : MonoBehaviour
{
    [EventRef] public string FootstepSFX;
    [EventRef] public string SpawnSound;

    private int uglySoundSolution = 1;

  public void PlayerRunSFX()
    {
        RuntimeManager.PlayOneShotAttached(FootstepSFX, gameObject);
    }

    public void SpawnSFX()
    {
        if (uglySoundSolution<2)
        {
            uglySoundSolution = uglySoundSolution + 1;
        }
        else
            RuntimeManager.PlayOneShot(SpawnSound);
    }
}
