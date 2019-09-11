#pragma warning disable 0649
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.VFX;
using UnityEngine.UI;

public class AnalogInput: MonoBehaviour {
    private GameObject inputVisual;
    [Range(0f, 5f), SerializeField] private float buttonOffset = 2;  //need to calculate it somehow
    [SerializeField] private float visualInputOffset = 0.75f;
    [SerializeField] private int defaultChoise = 4;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite[] iconList;
    [EventRef] public string confirmSFX;
    [EventRef] public string highlightSFX;

    private int currentChoice = 0;
    private Button[] children;
    private Vector2 inputValue;

    private int childCount = 0;
    private float anglePerChild = 360;
    private float angleOffset = -180;
    private float angle = 0;

    private bool choiceChanged = false;
    private VisualEffect vfx;

    private void Start() {
        Image background = GetComponent<Image>();
        childCount = transform.childCount;
        children = new Button[childCount];
        anglePerChild = 360 / childCount;

        for(int i = 0; i < childCount; i++) {
            children[i] = transform.GetChild(i).GetComponent<Button>();
            Vector2 point = CalculatePointPosition(i) * buttonOffset;
            children[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(point.x * (background.preferredWidth / 2),
                point.y * (background.preferredHeight / 2));
        }

        if(defaultChoise < children.Length) {
            currentChoice = defaultChoise;
            children[currentChoice].Select();
        }

        if(icon) {
            icon.sprite = iconList[currentChoice];
        }

        inputVisual = GameObject.Find("AttractiveTarget");

        InputVisualStartAtDefault();
        GameObject visualParent = GameObject.Find("VFX_MenuCursor");
        if(visualParent) {
            vfx = visualParent.GetComponent<VisualEffect>();
            if(vfx) {
                vfx.enabled = true;
            }
        }
    }

    private void Update() {
        if(!vfx.isActiveAndEnabled) {
            vfx.enabled = true;
        }

        inputValue = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if(inputValue.magnitude < 0.4f) {
            return;
        }

        CalculateAngle();
        CheckForInputChoice();

        if(Input.GetButtonDown("Jump")) {
            RuntimeManager.PlayOneShot(confirmSFX);
            children[currentChoice].onClick.Invoke();
        }
        if(choiceChanged) {
            RuntimeManager.PlayOneShot(highlightSFX);
            choiceChanged = false;
        }
        children[currentChoice].Select();
    }

    private void CalculateAngle() {
        if(inputValue == new Vector2(0, 0)) {
            return;
        }
        float angleInRadians = Mathf.Atan2(inputValue.y, inputValue.x);
        angle = (180 / Mathf.PI) * angleInRadians + 180;
    }

    private void MoveJoystick(int index) {
        if(inputValue == new Vector2(0, 0)) {
            return;
        }

        if(!inputVisual) {
            return;
        }

        Vector2 point = CalculatePointPosition(index);
        inputVisual.transform.position = point * (buttonOffset * visualInputOffset);
    }

    private void CheckForInputChoice() {
        if(inputValue == new Vector2(0, 0)) {
            return;
        }

        for(int i = 0; i < childCount; ++i) {
            if(angle > anglePerChild * i && angle <= anglePerChild * (i + 1)) {
                if(currentChoice != i) {
                    currentChoice = i;
                    children[currentChoice].Select();
                    choiceChanged = true;
                    if(iconList.Length > 0 && iconList.Length > currentChoice) {
                        icon.sprite = iconList[currentChoice];
                    }
                    MoveJoystick(i);
                    break;
                }
            }
        }
    }

    private Vector2 CalculatePointPosition(int index) {
        float radians = (anglePerChild * index + anglePerChild / 2 + angleOffset) * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    private void InputVisualStartAtDefault() {
        if(!inputVisual) {
            return;
        }

        Vector2 point = CalculatePointPosition(defaultChoise);
        inputVisual.transform.position = point * (buttonOffset * visualInputOffset);
    }

    private void OnEnable() {
        if(vfx) {
            vfx.enabled = true;
        }
    }

    private void OnDisable() {
        if(vfx) {
            vfx.enabled = false;
        }
        if(EventSystem.current)
            EventSystem.current.SetSelectedGameObject(null);
    }
}