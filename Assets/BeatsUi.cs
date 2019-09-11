#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatsUi : MonoBehaviour
{
    [Serializable]
    private struct BeatImagePair
    {
        public Image LeftImage;
        public Image RightImage;
        public void SetImageActive(bool active)
        {
            LeftImage.enabled = active;
            RightImage.enabled = active;
        }
    }
    [SerializeField] private BeatImagePair Outer;
    [SerializeField] private BeatImagePair Middle;
    [SerializeField] private BeatImagePair Inner;
    [SerializeField] private Image CentreBeat;
    private int beatCounter;



    // Start is called before the first frame update
    void Start()
    {
        Outer.SetImageActive(false);
        Middle.SetImageActive(false);
        Inner.SetImageActive(false);
        CentreBeat.enabled = false;
        BeatManager.OnBeat += BeatHappaned;
    }

    // Update is called once per frame
    void Update()
    {
        if (!BeatManager.IsSetup()) return;
        switch (beatCounter)
        {
            case 0:
                if (BeatManager.OffBeatPercent() >= 0.25)
                {
                    CentreBeat.enabled = false;
                    Outer.SetImageActive(true);
                    beatCounter = 1;
                }
            break;
            case 1:
                if (BeatManager.OffBeatPercent() >= 0.5)
                {
                    Outer.SetImageActive(false);
                    Middle.SetImageActive(true);
                    beatCounter = 2;
                }
            break;
            case 2:
                if (BeatManager.OffBeatPercent() >= 0.75)
                {
                    Middle.SetImageActive(false);
                    Inner.SetImageActive(true);
                    beatCounter = 3;
                }
                break;
            default:
                break;
        }
        
    }
    
    private void BeatHappaned()
    {
        beatCounter = 0;
        CentreBeat.enabled = true;
        Inner.SetImageActive(false);
    }

}
