using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FMODUnity;

public class WaveWarningUI : MonoBehaviour
{
    private RectTransform WavePromt;
    private TextMeshProUGUI warningThreat;
    [SerializeField] float showForTime = 3f;
    [SerializeField] float preShowTime = 2f;
    [EventRef] [SerializeField] private string[] soundThreat;


    private int lastThreatLevel = -1;

    void Start()
    {
        WavePromt = transform.GetChild(0).GetComponent<RectTransform>();
        RoundController.OnRoundWamup += ShowWarning;
        warningThreat = WavePromt.GetComponentInChildren<TextMeshProUGUI>();

        WavePromt.gameObject.SetActive(false);

    }

    private void ShowWarning(Round round)
    {
        if(lastThreatLevel != round.ThreatLevel)
        {
            RuntimeManager.PlayOneShot(soundThreat[round.ThreatLevel - 1]);
            lastThreatLevel = round.ThreatLevel;
            StopCoroutine(ShowingFor(round.ThreatLevel));
            StartCoroutine(ShowingFor(round.ThreatLevel));
        }

    }

    IEnumerator ShowingFor(int threatLevel)
    {
        float timer = preShowTime;



        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;

        }

        timer = showForTime;

        WavePromt.gameObject.SetActive(true);
        warningThreat.text = threatLevel.ToString();

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        WavePromt.gameObject.SetActive(false);

    }

}
