using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupPanel : MonoBehaviour
{
    public float TutorialSpeed = 1f;
    public TextMeshProUGUI TutorialText;
    public TextMeshProUGUI PopupText;
    public Image PopupImage;
    public RectTransform ButtonImageHolder;
    private Animator anime;
    private string tutorialMessage;
    private Sprite[] tutorialImages;
    private Image[] _imageHolders;

    private bool showingTutorial;
    // Start is called before the first frame update
    void OnEnable()
    {
        showingTutorial = false;
        anime = GetComponent<Animator>();
        ControllsTutorialState.OnNewTutorialStep += ShowTutorial;
        ControllsTutorialState.OnTutorialComplete += CompleteTutorial;
        RoundController.OnPopupEvent += ShowPopup;
        GameManager.ClosePopup += delegate { anime.SetTrigger("HidePopup");};
        anime.speed = TutorialSpeed;
    }

    private void Start()
    {
        _imageHolders = new Image[ButtonImageHolder.childCount];
        for (int i = 0; i < _imageHolders.Length; i++)
        {
            _imageHolders[i] = ButtonImageHolder.GetChild(i).GetComponent<Image>();
            _imageHolders[i].gameObject.SetActive(false);
        }

    }

    private void ShowTutorial(ControllsTutorialState.ButtonInfo info)
    {
        PauseGame.Instance.canPause = false;
        tutorialMessage = info.Info;
        tutorialImages = info.ButtonImages;
        if (showingTutorial)
            anime.SetTrigger("HideTutorial");
        else
        {
            TutorialText.text = tutorialMessage;
            tutorialImages = info.ButtonImages;
            showingTutorial = true;
        } 
        anime.SetTrigger("ShowTutorialInfo");

    }

    private void CompleteTutorial()
    {
        anime.SetTrigger("HideTutorial");
        showingTutorial = false;
        PauseGame.Instance.canPause = true;
    }
    
    
    private void ShowPopup(PopupData data)
    {
        anime.SetTrigger("ShowPopup");
        PopupImage.sprite = data.sprite;
        PopupText.text = data.message;
    }
    
    private void SetTutorialText()
    {
        TutorialText.text = tutorialMessage;
        for (int i = 0; i < _imageHolders.Length; i++)
        {
            if (i < tutorialImages.Length)
            {
                _imageHolders[i].gameObject.SetActive(true);
                _imageHolders[i].sprite = tutorialImages[i];
            }
            else
                _imageHolders[i].gameObject.SetActive(false);
            
                
        }
        
    }

    private void OnDisable()
    {
        ControllsTutorialState.OnNewTutorialStep -= ShowTutorial;
        ControllsTutorialState.OnTutorialComplete -= CompleteTutorial;
    }

    
}
