using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarsColorChanger : MonoBehaviour
    
{
    public Animator beatViz;

    private void Start()
    {
        PlayerAttackState.OnAttack += HappensOnAttack;
    }

    private void HappensOnAttack(RhythmGrade grade)
    {
        switch (grade)
        {
            case RhythmGrade.Miss:
                beatViz.SetTrigger("Missed");
                break;
            case RhythmGrade.Good:
                beatViz.SetTrigger("Synced");
                break;
            case RhythmGrade.Great:
                beatViz.SetTrigger("Synced");
                break;
            case RhythmGrade.Excellent:
                beatViz.SetTrigger("Synced");
                break;
            case RhythmGrade.Super:
                beatViz.SetTrigger("Synced");
                break;
        }
    }
}
