#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameOverGame : MonoBehaviour{

    [SerializeField] private PlayableAsset gameOverCinematic;
    private float cinematicEnd;
    private bool gameOver = false;

    void Start() {
        CoreHealth.Instance.coreDied += GameOver;
    }
    

    private void GameOver()
    {

        CoreHealth.Instance.coreDied -= GameOver;
        BeatManager.StopMusic();
        HUDManager hud = FindObjectOfType<HUDManager>();
        if(hud) {
            hud.gameObject.SetActive(false);
        }

        gameOver = true;
        PlayableDirector director = FindObjectOfType<PlayableDirector>();
        director.enabled = true;
        director.playableAsset = gameOverCinematic;
        director.Play();
        director.stopped += ShowScoreScreen;
        Transform parent = FindObjectOfType<CameraController>().transform.GetChild(0).transform;
        if(parent) {
            for(int i = 0; i < parent.childCount; i++) {
                parent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void ShowScoreScreen(PlayableDirector obj)
    {
        Transform[] ts = HUDManager.Instance.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform t in ts)
        {
            t.gameObject.SetActive(false);
        }

        HUDManager.Instance.gameObject.SetActive(true);
        ScoreScreenHandler.TurnOnOff(true);

        obj.stopped -= ShowScoreScreen;
    }
    


}