using UnityEngine;
using UnityEngine.UI;
using FMOD;
using FMODUnity;
using UnityEngine.Experimental.VFX;
using Debug = UnityEngine.Debug;


public class SyncPowerUI : MonoBehaviour {
    public Image SyncImage;
    public Sprite SyncFillImage1;
    public Sprite SyncFillImage2;
    public Sprite SyncFillImage3;
    private float maxSyncPower;
    [SerializeField] private PopupData syncPopup;
    [SerializeField] private VisualEffect gainSyncVFX;

    [EventRef] public string SyncReadySFX1;
    [EventRef] public string SyncReadySFX2;

    private bool startTut = false;
    private float lastSyncPercent;
    public void Start()
    {
        SyncPowerManager.OnSyncPowerUpdate += UpdateVisuales;
    }

    public void Init(float maxSync) {
        maxSyncPower = maxSync;
    }

    public void UpdateVisuales(float syncPowerPercentage)
    {
        SyncImage.fillAmount = syncPowerPercentage;
        
        if(lastSyncPercent < 0.5f && syncPowerPercentage >= 0.5f) //to half
        {
            SyncImage.sprite = SyncFillImage2;
            RuntimeManager.PlayOneShot(SyncReadySFX1);
            if (!startTut)
            {
                startTut = true;
                RoundController.OnPopupEvent?.Invoke(syncPopup);
            }
        }

        else if(lastSyncPercent < 1 && syncPowerPercentage == 1F) //to full
        {
            SyncImage.sprite = SyncFillImage3;
            RuntimeManager.PlayOneShot(SyncReadySFX2);
        }

        else if (lastSyncPercent == 1 && syncPowerPercentage <=0.7) //from full to half
        {
            SyncImage.sprite = SyncFillImage2;
        }

        else if (lastSyncPercent == 1 && syncPowerPercentage <= 0) //from full to empty
        {
            SyncImage.sprite = SyncFillImage1;
        }

        else if (lastSyncPercent>=0.5 && syncPowerPercentage<0.5) //from more than half to more than nothing
        {
            SyncImage.sprite = SyncFillImage1;
        }

        if(lastSyncPercent < syncPowerPercentage) {
            if(gainSyncVFX) {
                gainSyncVFX.SendEvent("OnBeat");
            }
        }


        lastSyncPercent = syncPowerPercentage;
    }
}