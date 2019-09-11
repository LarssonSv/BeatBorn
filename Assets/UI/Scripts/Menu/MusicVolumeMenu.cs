using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeMenu: BaseMenu<MusicVolumeMenu> {
    [SerializeField] private Image fillImage;
    [SerializeField] private float inputSpeed = 0.01f;
    [SerializeField] private TextMeshProUGUI text;
    private float xInput;
    private float xInputHolder;

    private FMOD.Studio.Bus musicBus;

    private void Start() {
        float value = SaveManager.LoadSetting("MusicVolume");
        fillImage.fillAmount = value;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";

        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MUSIC");
        musicBus.setVolume(value);
    }

    private void Update() {
        if(Input.GetButtonDown("Jump")) {
            Back();
        }
        xInput = Input.GetAxis("Horizontal");
        if(xInput == 0) {
            xInputHolder = 0;
            return;
        }
        float diff = xInput - xInputHolder;
        xInputHolder += diff * inputSpeed * Time.unscaledDeltaTime;

        fillImage.fillAmount = fillImage.fillAmount + xInputHolder;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";
        musicBus.setVolume(fillImage.fillAmount);
    }

    public override void Back() {
        SaveManager.SaveSettings("MusicVolume", fillImage.fillAmount);
        base.Back();
    }
}