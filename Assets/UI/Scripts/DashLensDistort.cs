using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashLensDistort : MonoBehaviour
{
    public GameObject dashLensDistortion;
    private static DashLensDistort instance;
    [SerializeField] private float LensDistTime;

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        instance = this;
        instance.dashLensDistortion.SetActive(false);
    }

    public static void DashDistortion()
    {
        instance.dashLensDistortion.SetActive(true);
        instance.Invoke("NoDist", instance.LensDistTime);
    }

    private void NoDist()
    {
        instance.dashLensDistortion.SetActive(false);
    }

}
