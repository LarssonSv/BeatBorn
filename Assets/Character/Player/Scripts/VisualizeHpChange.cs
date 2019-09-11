using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualizeHpChange : MonoBehaviour
{
    public AnimationCurve ScreenBleedCurve;
    public AnimationCurve FullBleedCurve;

    public Image ScreenBleed;
    public Image FullBleed;

    void Start()
    {
        PlayerEngine.OnLifeChanged += BleedTheScreen;
    }

    void BleedTheScreen(float Life)
    {
        Debug.Log("My life is: " + Life);
        ScreenBleed.color = new Color(1, 1, 1, ScreenBleedCurve.Evaluate(1 - Life));
        FullBleed.color = new Color(1, 1, 1, FullBleedCurve.Evaluate(1 - Life));
    }
}
