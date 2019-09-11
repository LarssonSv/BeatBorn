using UnityEngine;
using UnityEngine.UI;

public class QualityMenu : BaseMenu<QualityMenu> {
    [SerializeField] private Button button;
    [SerializeField] private Image visual;
    [SerializeField] private Sprite high;
    [SerializeField] private Sprite low;
    private int quality = 0;

    private void Start() {
        quality = (int)SaveManager.LoadSetting("Quality");
        visual.sprite = quality == 0 ? high : low;
    }

    private void Update() {
        if(Input.GetButtonDown("Jump")) {
            button.onClick.Invoke();
        }
    }

    public void ChangeQuality() {
        quality = (quality + 1) % 2;
        visual.sprite = quality == 0 ? high : low;
        QualitySettings.SetQualityLevel(quality, false);
    }

    public override void Back() {
        SaveManager.SaveSettings("Quality", quality);
        base.Back();
    }
}