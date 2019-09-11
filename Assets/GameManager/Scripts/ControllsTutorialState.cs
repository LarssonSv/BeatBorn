#pragma warning disable 0649
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialInputType
{
    Rhythm,
    LightAttack,
    HeavyAttack,
    Dash,
    Jump,
    LostStarlightDance,
    DragontailWhipFlash,
    SonicDeathChop,
    Super
}

[CreateAssetMenu(menuName = "GameManager/ControllsTutorial", fileName = "ControllsTutorialState")]
public class ControllsTutorialState : GameManagerState
{


    [Serializable]
    public struct ButtonInfo
    {
        public TutorialInputType  Button;
        [TextArea] public string Info;
        public Sprite[] ButtonImages;
    } 
    
    [SerializeField] private ButtonInfo[] TutorialSteps;
    private int currentTutorialIndex;

    public static Action<ButtonInfo> OnNewTutorialStep;
    public static Action OnTutorialComplete;
    public static Action<string> OnNewPopup;
    public int BeatsToHitInARow = 4;
    private int toTheBeatcounter;
    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        currentTutorialIndex = 0;
        PlayerAttackState.OnComboExecuted += OnComboExecuted;
    }

    public override void Enter()
    {
        if (manager.SkipTutorial)
        {
            TransitionTo<InRoundState>();
            return;
        }
        Debug.Log("eNTER");
        OnNewTutorialStep?.Invoke(TutorialSteps[currentTutorialIndex]);
        BeatManager.Play();
        toTheBeatcounter = 0;
        PlayerAttackState.OnAttack += Attacking;
    }

    private void Attacking(RhythmGrade grade)
    {
        toTheBeatcounter = grade == RhythmGrade.Miss ? 0 : toTheBeatcounter + 1;
    }

    public override void StateUpdate()
    {
        if (currentTutorialIndex >= TutorialSteps.Length)
        {
            TransitionTo<InRoundState>();
            return;
        }


        else if (CheckForInput())
        {
            Debug.Log("You did it!");
        }
    }

    private bool CheckForInput()
    {
        bool inputSuces = false;
        
        switch (TutorialSteps[currentTutorialIndex].Button)
        {
            case TutorialInputType.Rhythm:
                inputSuces = toTheBeatcounter >= BeatsToHitInARow;
            break;
            case TutorialInputType.LightAttack:
                inputSuces = Input.GetButtonDown("LightAttack");
            break;
            case TutorialInputType.HeavyAttack:
                inputSuces = Input.GetButtonDown("HeavyAttack");
            break;
            case TutorialInputType.Dash:
                inputSuces = Input.GetButtonDown("Dash");
            break;
            case TutorialInputType.Jump:
                inputSuces = Input.GetButtonDown("Jump");
            break;     
        }

        if (inputSuces)
        {
            currentTutorialIndex++;
            if(currentTutorialIndex < TutorialSteps.Length)
                OnNewTutorialStep?.Invoke(TutorialSteps[currentTutorialIndex]);
        }
     
        
        return inputSuces;
    }
    
    private void OnComboExecuted(string comboName)
    {              
        if (comboName == TutorialSteps[currentTutorialIndex].Button.ToString())
        {
            currentTutorialIndex++;
            if(currentTutorialIndex < TutorialSteps.Length)
                OnNewTutorialStep?.Invoke(TutorialSteps[currentTutorialIndex]);
        }
            
    }

    public override void Exit()
    {
        PlayerAttackState.OnComboExecuted -= OnComboExecuted;
        PlayerAttackState.OnAttack -= Attacking;
        OnTutorialComplete?.Invoke();
    }
}
