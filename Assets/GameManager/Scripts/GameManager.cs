using System;
using System.Collections;
using System.Collections.Generic;
using StateKraft;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StateMachine Machine;
    public RoundController RoundController;
    public bool SkipTutorial;
    public HUDManager HudManager;
    public Canvas CinematicCanvas;
    private static GameManager instance;
    public static GameManagerState CurrentState => (GameManagerState)instance.Machine.CurrentState;

    public static Action ClosePopup; 
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        HudManager = FindObjectOfType<HUDManager>();
        if(PlayerPrefs.GetInt("ThreatLevel") > 0)
        {
            Debug.Log(PlayerPrefs.GetInt("ThreatLevel"));
            SkipTutorial = true;
        }
    }

    void Start()
    {
        RoundController = FindObjectOfType<RoundController>();
        Machine.Initialize(this);
    }

    // Update is called once per frame
    void Update()
    {
        Machine.Update();
    }
}
