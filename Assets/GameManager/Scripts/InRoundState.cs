using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameManager/InRound", fileName = "InRoundState")]

public class InRoundState : GameManagerState
{
    private bool ShowingPopup;
    private TutorialInputType inputType;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        RoundController.OnPopupEvent += delegate(PopupData data)
        {
            inputType = data.inputType;
            ShowingPopup = true;
        };
    }

    public override void Enter()
    {
        BeatManager.Play();
        roundController.StartRound();
    }

    public override void StateUpdate()
    {
        if(!ShowingPopup)
            return;

        switch (inputType)
        {
            case TutorialInputType.LightAttack:
                break;
            case TutorialInputType.HeavyAttack:
                break;
            case TutorialInputType.Dash:
                break;
            case TutorialInputType.Jump:
                break;
            case TutorialInputType.LostStarlightDance:
                break;
            case TutorialInputType.DragontailWhipFlash:
                break;
            case TutorialInputType.SonicDeathChop:
                break;
            case TutorialInputType.Super:
                if (PlayerEngine.CurrentState is PlayerSuperState)
                {
                    ShowingPopup = false;
                    GameManager.ClosePopup?.Invoke();
                }
            
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
