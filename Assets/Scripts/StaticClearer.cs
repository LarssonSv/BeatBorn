using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StaticClearer : MonoBehaviour
{
    private void Awake()
    {
        ClearAll();
    }

    public static void ClearAll()
    {
        Time.timeScale = 1.0f;
        ClearStaticFields(typeof(SyncPowerManager).GetFields());
        ClearStaticFields(typeof(BeatManager).GetFields());
        ClearStaticFields(typeof(RoundController).GetFields());
        ClearStaticFields(typeof(MusicTest).GetFields());
        ClearStaticFields(typeof(PlayerEngine).GetFields());
        ClearStaticFields(typeof(GameManager).GetFields());
        ClearStaticFields(typeof(ControllsTutorialState).GetFields());
        ClearStaticFields(typeof(PlayerDashState).GetFields());
        ClearStaticFields(typeof(PlayerAttackState).GetFields());
        ClearStaticFields(typeof(ObjectPool).GetFields());
        BeatManager.Instance = null;
    }
    
    private static void ClearStaticFields(FieldInfo[] fields)
    {
        foreach (FieldInfo field in fields)
        {
            if(field.IsStatic)
                field.SetValue(null, null);
        }
    }
}
