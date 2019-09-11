using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public RectTransform WarningHolder;
    [SerializeField] private float showWarningTime;
    private float showTimer;
    [Range(0f, 1f)] public float bufferPercentage;
    private bool lookingAtCore;
    
    private void Start()
    {
        CoreHealth.Instance.OnTakingDamage += ShowWarning;
    }

    private void ShowWarning()
    {
        showTimer = showWarningTime;
        WarningHolder.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(showTimer <= 0)
            return;

        lookingAtCore = Vector3.Dot(Camera.main.transform.forward, (CoreHealth.Instance.transform.position - Camera.main.transform.position).normalized) > 0;
        Vector3 pos = Vector3.zero;
        Debug.Log(lookingAtCore);
        if (lookingAtCore)
        {
                            
            WarningHolder.position = Camera.main.WorldToScreenPoint(NexusCrowdManager.Instance.transform.position);
            pos = WarningHolder.anchoredPosition;
            pos.x = Mathf.Clamp(pos.x, -1920 * bufferPercentage * 0.5f, 1920 * bufferPercentage * 0.5f);
            pos.y = Mathf.Clamp(pos.y, -1080 * bufferPercentage * 0.5f, 1080 * bufferPercentage * 0.5f);

        }
        else
        {
            pos.x = Vector3.Dot(Camera.main.transform.right,
                        (CoreHealth.Instance.transform.position - Camera.main.transform.position).normalized) > 0
                ? 1080 * bufferPercentage * 0.5f
                : -1080 * bufferPercentage * 0.5f;
            pos.y = 0;

        }

        
        Debug.Log( "PErcentage: " + Screen.width / 1920f);
        

        
        WarningHolder.anchoredPosition = pos;
        showTimer -= Time.deltaTime;
        if(showTimer <= 0)
            WarningHolder.gameObject.SetActive(false);
    }
}
