using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboEffectManager : MonoBehaviour, IPooledObject
{
    [SerializeField] private Color32 missColor;
    [SerializeField] private Color32 goodColor;
    [SerializeField] private Color32 greatColor;
    [SerializeField] private Color32 excellentColor;
    [SerializeField] private Color32 superColor;

    [SerializeField] GameObject kawaii;
    [SerializeField] private TextMeshProUGUI textPro;
    [SerializeField] private float despawnTime = 1f;
    [SerializeField] private AnimationCurve fadeOut;
    [SerializeField] private AnimationCurve movementUpwards;


    private CanvasGroup canvas;
    private float timer = 0;
    private bool onoff = false;
    private Color32 defaultColor;
    private Vector3 movement = Vector3.zero;

    public int grade;

    private void Awake()
    {
        defaultColor = textPro.faceColor;
        canvas = GetComponentInChildren<CanvasGroup>();
        
    }

    public void OnObjectSpawn()
    {  
        //textPro.faceColor = defaultColor;
        canvas.alpha = 1f;
        onoff = true;
        textPro.text = StreakCounterUI.PopUpScore.ToString();
        timer = 0;

        switch (grade)
        {
            case (int)RhythmGrade.Miss:
                textPro.faceColor = missColor;
                break;
            case (int)RhythmGrade.Good:
                textPro.faceColor = goodColor;
                break;
            case (int)RhythmGrade.Great:
                textPro.faceColor = greatColor;
                break;
            case (int)RhythmGrade.Excellent:
                textPro.faceColor = excellentColor;
                break;
            case (int)RhythmGrade.Super:
                textPro.faceColor = superColor;
                break;
        }


        textPro.UpdateMeshPadding();
    }

    private void Update()
    {
        if (onoff)
        {
            if (timer > despawnTime)
            {
                Exit();
                return;
            }

            Vector3 heading = Camera.main.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(heading, Vector3.up);


            canvas.alpha = fadeOut.Evaluate(1 - timer / despawnTime);
            movement.y = movementUpwards.Evaluate(1 - timer / despawnTime) / 10f;
            transform.Translate(movement);
            timer += Time.deltaTime;
        }

    }

    private void Exit()
    {
        canvas.alpha = 1f;
        timer = 0;
        onoff = false;
        gameObject.SetActive(false);
    }


}
