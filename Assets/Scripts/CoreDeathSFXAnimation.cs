using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class CoreDeathSFXAnimation : MonoBehaviour
{
    [EventRef] public string CoreDeathSFXYeah;
   
    public void CoreDeath()
    {
        RuntimeManager.PlayOneShot(CoreDeathSFXYeah);
    }
}
