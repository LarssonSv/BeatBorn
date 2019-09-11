using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RhythmGrade
{
    Miss,
    Good,
    Great,
    Excellent,
    Super
}

[CreateAssetMenu(menuName = "Rhythm")]
public class BeatData : ScriptableObject
{
   [Range(0f, 1f)] public float GoodPercentage;
   [Range(0f, 1f)] public float GreatPercentage;
   [Range(0f, 1f)] public float ExcellentPercentage;
   [Range(0f, 1f)] public float SuperPercentage;
   [SerializeField] private float MashSafeTime = 0.25f;
   private float timeOfLastHit;
   private RhythmGrade thisFrameGrade;
   public void Setup()
   {
       timeOfLastHit = 0;
   }
   
   public RhythmGrade GetGrade()
   {
       if (timeOfLastHit == Time.time)
           return thisFrameGrade;
       
       
       bool mashing = Time.time - timeOfLastHit < MashSafeTime;
       timeOfLastHit = Time.time;

       if (mashing)
       {
           thisFrameGrade = RhythmGrade.Miss;
           return thisFrameGrade;
       }
       
       double beatPercentage = BeatManager.OffBeatPercent();
       if (beatPercentage > 0.5d)
           beatPercentage = 1 - beatPercentage;

       beatPercentage *= 2d;
       
       if (beatPercentage > GoodPercentage)
           thisFrameGrade = RhythmGrade.Miss;
       else if (beatPercentage > GreatPercentage)
           thisFrameGrade = RhythmGrade.Good;
       else if (beatPercentage > ExcellentPercentage)
           thisFrameGrade = RhythmGrade.Great;
       else if (beatPercentage > SuperPercentage)
           thisFrameGrade = RhythmGrade.Excellent;
       else
         thisFrameGrade = RhythmGrade.Super;

       return thisFrameGrade;
   }
}
