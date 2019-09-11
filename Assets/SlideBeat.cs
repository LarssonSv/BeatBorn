using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideBeat : MonoBehaviour
{
    public GameObject BeatBox;
    public GameObject BeatCircle;
    [Range(0f, 0.5f)] public float PreBeatVisualzsion;
    [Range(0f, 0.5f)] public float PostBeatVisualzsion;
    public int BeatsCount = 2;
    [Range(0f, 1f)]public float ScreenCovragePercentage;
    private Image[] lefImage;
    private Image[] rightImage;
    private Color startColor;
    private float startDistance;
    // Start is called before the first frame update
    void Start()
    {
        startDistance = ScreenCovragePercentage * Screen.width * 0.5f;
        
        lefImage = new Image[BeatsCount];
        rightImage = new Image[BeatsCount];

        for (int i = 0; i < BeatsCount; i++)
        {
            lefImage[i] = Instantiate(BeatBox, transform).GetComponent<Image>();
            rightImage[i] = Instantiate(BeatBox, transform).GetComponent<Image>();
            
            lefImage[i].transform.SetSiblingIndex(0);
            rightImage[i].transform.SetSiblingIndex(1);
        }

        startColor = BeatBox.GetComponent<Image>().color;
        
        Destroy(BeatBox);
    }

    void Update()
    {
        float piceOfMovement = startDistance / BeatsCount;
        for (int i = 1; i <= BeatsCount; i++)
        {
            rightImage[i - 1].rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(piceOfMovement * i, 0), new Vector2(piceOfMovement * (i - 1), 0), BeatManager.GetTimeToQuarterNote());
            lefImage[i - 1].rectTransform.anchoredPosition = Vector2.Lerp(new Vector2(-piceOfMovement * i, 0), new Vector2(-piceOfMovement * (i - 1), 0), BeatManager.GetTimeToQuarterNote());

            if (i == BeatsCount)
            {
                rightImage[i - 1].color = new Color(startColor.r, startColor.g, startColor.b, BeatManager.GetTimeToQuarterNote());
                lefImage[i - 1].color = new Color(startColor.r, startColor.g, startColor.b, BeatManager.GetTimeToQuarterNote());
            }
        }
        float currentBeatTime = (float)BeatManager.OffBeatPercent();
        bool showCircle = currentBeatTime <= PostBeatVisualzsion || currentBeatTime > 1 - PreBeatVisualzsion;
        BeatCircle.SetActive(showCircle);
    }
}
