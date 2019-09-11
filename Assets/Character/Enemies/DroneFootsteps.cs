using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class DroneFootsteps : MonoBehaviour
{
    [EventRef] public string DroneFootstepsEvent;

   public void EnemyStep()
    {
        RuntimeManager.PlayOneShotAttached(DroneFootstepsEvent, gameObject);
    }
}
