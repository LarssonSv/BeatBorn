using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsVolumeMenu : BaseMenu<SoundEffectsVolumeMenu> {
    [SerializeField] private Image fillImage;
    [SerializeField] private float inputSpeed = 0.01f;
    [SerializeField] private TextMeshProUGUI text;
    private float xInput;
    private float xInputHolder;

    private FMOD.Studio.Bus sfxBus;

    private void Start() {
        float value = SaveManager.LoadSetting("SoundEffectVolume");
        fillImage.fillAmount = value;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";

        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        sfxBus.setVolume(value);
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
        print(xInput);
        float diff = xInput - xInputHolder;
        xInputHolder += diff * inputSpeed * Time.unscaledDeltaTime;
        print(diff);
        print(xInputHolder);
        fillImage.fillAmount = fillImage.fillAmount + xInputHolder;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";
        print(fillImage.fillAmount);
        sfxBus.setVolume(fillImage.fillAmount);
    }

    public override void Back() {
        SaveManager.SaveSettings("SoundEffectVolume", fillImage.fillAmount);
        base.Back();
    }
}