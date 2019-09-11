using UnityEngine;
using UnityEngine.UI;

public class RumbleMenu : BaseMenu<RumbleMenu> {
    [SerializeField] private Button button;
    [SerializeField] private Image visual;
    [SerializeField] private Sprite yes;
    [SerializeField] private Sprite no;
    private bool on = true;

    private void Start() {
        on = SaveManager.LoadSetting("Rumble") == 1 ? true : false;
        visual.sprite = on ? yes : no;
    }

    private void Update() {
        if(Input.GetButtonDown("Jump")) {
            button.onClick.Invoke();
        }
    }

    public void RumbleOnOff() {
        on = !on;
        visual.sprite = on ? yes : no;
    }

    public override void Back() {
        SaveManager.SaveSettings("Rumble", on ? 1 : 0);
        base.Back();
    }
}