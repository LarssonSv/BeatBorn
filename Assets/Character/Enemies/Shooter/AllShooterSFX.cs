using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AllShooterSFX : MonoBehaviour
{
    [EventRef] public string ShootSFX;
    [EventRef] public string WalkSFX;
    [EventRef] public string MeleeSFX;

    public void Shoot()
    {
        RuntimeManager.PlayOneShotAttached(ShootSFX, gameObject);
    }

    public void Melee()
    {
        RuntimeManager.PlayOneShotAttached(MeleeSFX, gameObject);
    }

    public void Walk()
    {
        RuntimeManager.PlayOneShotAttached(WalkSFX, gameObject);
    }
}
