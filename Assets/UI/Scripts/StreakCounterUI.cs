using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StreakCounterUI : MonoBehaviour
{
    //Author Jonas Petersson

    private float MissValue = 0;
    private float GoodValue = 20f;
    private float GreatValue = 30f;
    private float ExcecllentValue = 40f;
    private float SuperValue = 50f;

    [SerializeField] private float streakTickDownInSec = 1f;
    [SerializeField] private float resetStreakTime = 3f;

    public static float TopHitsInArow;
    public static int HitsOnBeat;
    public static int HitsOffBeat;
    public static float StreakScoreHolder;

                
    private static StreakCounterUI instance;

    private float streakHitMultiplier = 1f;
    private float onHitScore = 5f;
    public static float scoreToPrint;
    public static float PopUpScore;

    readonly private string StrHandleUI = "ResetGradeAndStartStreakCD";
    readonly private string StrkTickDown = "StreakTickDown";

    public int MaxStreakValue = 10;
    public static int DeathCount;
    
    public TextMeshProUGUI StreakCounter;
    public TextMeshProUGUI CurrentHitGrade;
    public TextMeshProUGUI PlayerScore;

    public static float superDuperUltraScore;
    

    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        instance = this;
    }

    public static void AddMultiplier(RhythmGrade rhythmGrade)
    {
        switch (rhythmGrade)
        {
            case RhythmGrade.Miss:
                instance.streakHitMultiplier = instance.streakHitMultiplier - 1f;
                instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
                instance.UpdateStreak(instance.streakHitMultiplier);
                StreakScoreHolder = StreakScoreHolder + 0;
                StreakScoreHolder = Mathf.Clamp(StreakScoreHolder, 0, instance.MaxStreakValue);
                HitsOffBeat = HitsOffBeat + 1;
                break;
            case RhythmGrade.Good:
                instance.streakHitMultiplier = instance.streakHitMultiplier + 1f;
                instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
                instance.UpdateStreak(instance.streakHitMultiplier);
                StreakScoreHolder = StreakScoreHolder + 1;
                StreakScoreHolder = Mathf.Clamp(StreakScoreHolder, 0, instance.MaxStreakValue);
                HitsOnBeat = HitsOnBeat + 1;
                break;
            case RhythmGrade.Great:
                instance.streakHitMultiplier = instance.streakHitMultiplier + 1f;
                instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
                instance.UpdateStreak(instance.streakHitMultiplier);
                StreakScoreHolder = StreakScoreHolder + 1;
                StreakScoreHolder = Mathf.Clamp(StreakScoreHolder, 0, instance.MaxStreakValue);
                HitsOnBeat = HitsOnBeat + 1;
                break;
            case RhythmGrade.Excellent:
                instance.streakHitMultiplier = instance.streakHitMultiplier + 1f;
                instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
                instance.UpdateStreak(instance.streakHitMultiplier);
                StreakScoreHolder = StreakScoreHolder + 1;
                StreakScoreHolder = Mathf.Clamp(StreakScoreHolder, 0, instance.MaxStreakValue);
                HitsOnBeat = HitsOnBeat + 1;
                break;
            case RhythmGrade.Super:
                instance.streakHitMultiplier = instance.streakHitMultiplier + 1f;
                instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
                instance.UpdateStreak(instance.streakHitMultiplier);
                StreakScoreHolder = StreakScoreHolder + 1;
                StreakScoreHolder = Mathf.Clamp(StreakScoreHolder, 0, instance.MaxStreakValue);
                HitsOnBeat = HitsOnBeat + 1;
                break;
        }
    }


    public static void AddHit(RhythmGrade grade)
    {
        instance.onHitScore = 50f;

        Debug.Log(grade);
        
        switch (grade)
        {
            case RhythmGrade.Miss:
                instance.onHitScore = instance.onHitScore * instance.MissValue;
                instance.UpdateGrade("miss");
                break;
            case RhythmGrade.Good:
                instance.onHitScore = instance.onHitScore + instance.GoodValue;
                instance.UpdateGrade("GOOD");
                break;
            case RhythmGrade.Great:
                instance.onHitScore = instance.onHitScore + instance.GreatValue;
                instance.UpdateGrade("GREAT");
                break;
            case RhythmGrade.Excellent:
                instance.onHitScore = instance.onHitScore + instance.ExcecllentValue;
                instance.UpdateGrade("SUPER");
                break;
            case RhythmGrade.Super:
                instance.onHitScore = instance.onHitScore + instance.SuperValue;
                instance.UpdateGrade("PERFECT");
                break;
        }
        instance.StartInvoke();
        instance.MultiplyScore();
    }

    void StartInvoke()
    {
        if (IsInvoking(StrHandleUI))
            {
                CancelInvoke();
                instance.StartInvoke();
            }
        else
        {
            Invoke(StrHandleUI, resetStreakTime);
        }
    }


    public static float GetCurrentStreakPercentage()
    {
        return (instance.streakHitMultiplier - 1) / (instance.MaxStreakValue - 1);
    }
    
    void MultiplyScore()
    {
        if(streakHitMultiplier > TopHitsInArow)
        {
            TopHitsInArow = streakHitMultiplier;
        }

        PopUpScore = instance.streakHitMultiplier * instance.onHitScore;
        scoreToPrint = scoreToPrint + PopUpScore;
        instance.PlayerScore.text = scoreToPrint.ToString();
    }

    void ResetGradeAndStartStreakCD()
    {
        instance.CurrentHitGrade.text = "";
        Invoke(StrkTickDown, streakTickDownInSec);
    }
    
    private void UpdateGrade(string newGrade)
    {
        instance.CurrentHitGrade.text = newGrade;
    }

    private void UpdateStreak(float newStreak)
    {
        instance.StreakCounter.text = newStreak.ToString() + "x";
    }

    void StreakTickDown()
    {
        if (instance.streakHitMultiplier <= 2f)
        {
            instance.StreakCounter.text = "";
        }
        else
        {
            instance.streakHitMultiplier = instance.streakHitMultiplier - 1f;
            instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1f, instance.MaxStreakValue);
            instance.StreakCounter.text = instance.streakHitMultiplier.ToString() + "x";
            Invoke(StrkTickDown, streakTickDownInSec);
        }
    }

    public static void DeathResetStreak()
    {
        instance.CurrentHitGrade.text = "";
        instance.streakHitMultiplier = instance.streakHitMultiplier - 10f;
        instance.streakHitMultiplier = Mathf.Clamp(instance.streakHitMultiplier, 1, instance.MaxStreakValue);
        instance.StreakCounter.text = "";
    }

    public static void DashTimerReset()
    {
        instance.StartInvoke();
    }

    private void OnDestroy() {
        scoreToPrint = 0;
    }
    public static void CollectDeath()
    {
        DeathCount = DeathCount + 1;
    }

    public static float GetHighScore()
    {
        return superDuperUltraScore;
    }
}