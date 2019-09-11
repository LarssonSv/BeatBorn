#pragma warning disable 0162
#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
[CreateAssetMenu(menuName = "GameManager/CinematicState", fileName = "CinematicState")]
public class CinematicState : GameManagerState
{
    [SerializeField] private TimelineAsset IntroCinematic;
    [SerializeField] private TimelineAsset ShooterCinematic;
    [SerializeField] private TimelineAsset Deathcinematic;

    private int currentCinematicIndex;
    private PlayableDirector cameraDirector;
    private CameraController cameraController;
    
    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        currentCinematicIndex = 0;
        cameraDirector = FindObjectOfType<PlayableDirector>();
        cameraController = FindObjectOfType<CameraController>();
    }

    public override void Enter()
    {
        cameraController.PlayCinematic();
        cameraDirector.enabled = true;
        cameraDirector.Play(IntroCinematic);
        cameraDirector.stopped += GoToNextState;
        manager.HudManager.SetHudEnabled(false);
        manager.CinematicCanvas.gameObject.SetActive(true);
    }

    public override void StateUpdate()
    {
        if(Input.GetButtonDown("Jump"))
            GoToNextState();
    }

    private void GoToNextState(PlayableDirector obj = null)
    {
        if (currentCinematicIndex == 0)
            TransitionTo<ControllsTutorialState>();
        else
            TransitionTo<InRoundState>();
    }

    private TimelineAsset GetTimeline()
    {
        switch (currentCinematicIndex)
        {
            case 0:
                return IntroCinematic;
                break;
            case 1:
                return ShooterCinematic;
                break;
            case 2:
                return Deathcinematic;
                break;
            
            default: return Deathcinematic;
        }
    }
    
    public override void Exit()
    {
        manager.HudManager.SetHudEnabled(true);
        manager.CinematicCanvas.gameObject.SetActive(false);
        cameraController.StopCinematic();
        cameraDirector.enabled = false;
        currentCinematicIndex++;
    }
}
