using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu: BaseMenu<OptionsMenu> {
    private enum ActiveMenu { Music, SoundEffect, Rumble, Quality, None }

    private ActiveMenu activeMenu = ActiveMenu.None;
    private ActiveMenu prevActiveMenu;
    private AnalogInput analog;
    [Header("Music")]
    [SerializeField]
    private Image fillImage;
    [SerializeField]
    private float inputSpeed = 0.01f;
    [SerializeField]
    private TextMeshProUGUI text;
    private float xInput;
    private float xInputHolder;
    private FMOD.Studio.Bus musicBus;

    [Header("Rumble")]
    [SerializeField]
    private Image background;
    [SerializeField]
    private Image visual;
    [SerializeField]
    private Sprite yes;
    [SerializeField]
    private Sprite no;
    private bool on = true;

    [Header("Quality")]
    [SerializeField]
    private Sprite high;
    [SerializeField]
    private Sprite low;
    private int quality = 0;

    private void Start() {
        DisableAdditionalMenus();

        if(!analog) {
            analog = GetComponentInChildren<AnalogInput>();
        }
    }

    private void Update() {
        if(!analog || analog.enabled || activeMenu == ActiveMenu.None) {
            return;
        }
        if(prevActiveMenu != activeMenu) {
            prevActiveMenu = activeMenu;
            return;
        }

        if(activeMenu == ActiveMenu.Music || activeMenu == ActiveMenu.SoundEffect) {
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

        if(Input.GetButtonDown("Jump")) {
            if(activeMenu == ActiveMenu.Rumble) {
                on = !on;
                visual.sprite = on ? yes : no;
            }

            if(activeMenu == ActiveMenu.Quality) {
                quality = (quality + 1) % 2;
                visual.sprite = quality == 0 ? high : low;
                QualitySettings.SetQualityLevel(quality, false);
            }
        }
    }

    public void MusicVolume() {
        analog.enabled = false;
        activeMenu = ActiveMenu.Music;

        fillImage.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        float value = SaveManager.LoadSetting("MusicVolume");
        fillImage.fillAmount = value;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";

        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MyMaster/MUSIC");
        musicBus.setVolume(value);
    }

    public void SoundEffectsVolume() {
        analog.enabled = false;
        activeMenu = ActiveMenu.SoundEffect;

        fillImage.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        float value = SaveManager.LoadSetting("SoundEffectVolume");
        fillImage.fillAmount = value;
        text.text = (fillImage.fillAmount * 100).ToString("000") + "%";

        musicBus = FMODUnity.RuntimeManager.GetBus("bus:/MyMaster/SFX");
        musicBus.setVolume(value);
    }

    public void Rumble() {
        analog.enabled = false;
        activeMenu = ActiveMenu.Rumble;

        on = SaveManager.LoadSetting("Rumble") == 1;
        background.gameObject.SetActive(true);
        visual.gameObject.SetActive(true);
        visual.sprite = on ? yes : no;
    }


    public void ControlScheme() {
        MenuManager.Instance.Create<ControlSchemeMenu>();
    }

    public void QualitySettingsMenu() {
        analog.enabled = false;
        activeMenu = ActiveMenu.Quality;

        visual.gameObject.SetActive(true);
        quality = (int)SaveManager.LoadSetting("Quality");
        visual.sprite = quality == 0 ? high : low;
    }

    private void DisableAdditionalMenus() {
        fillImage.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
        visual.gameObject.SetActive(false);
        activeMenu = ActiveMenu.None;
    }

    public override void Back() {
        if(analog.enabled) {
            base.Back();
        }

        switch(activeMenu) {
            case ActiveMenu.Music:
                SaveManager.SaveSettings("MusicVolume", fillImage.fillAmount);
                break;
            case ActiveMenu.SoundEffect:
                SaveManager.SaveSettings("SoundEffectVolume", fillImage.fillAmount);
                break;
            case ActiveMenu.Rumble:
                SaveManager.SaveSettings("Rumble", on ? 1 : 0);
                break;
            case ActiveMenu.Quality:
                SaveManager.SaveSettings("Quality", quality);
                break;
        }
        analog.enabled = true;
        DisableAdditionalMenus();
    }
}