#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : BaseMenu<PauseMenu> {
    private HUDManager hud;

    private void Start() {
        hud = FindObjectOfType<HUDManager>();
        if(hud) {
            hud.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if(Input.GetButtonDown("Dash")) {
            Continue();
        }
    }

    public void Continue() {
        PauseGame.UnPause();
    }

    public void Options() {
        MenuManager.Instance.Create<OptionsMenu>();
    }

    public override void Back() {
        BeatManager.StopMusic();
        MenuManager.Instance.CloseAllMenus();
        GameObject go = MenuManager.Instance.gameObject;
        Destroy(go);
        int sceneToMoveTo = SceneManager.GetActiveScene().buildIndex - 1;

        if (sceneToMoveTo >= 0) {
            SceneManager.LoadScene(sceneToMoveTo);
        } else {
            UnityEngine.Debug.Log("There is no scene assigned in build settings!");
        }
    }

    private void OnDestroy() {
        if(hud) {
            hud.gameObject.SetActive(true);
        }
    }
}
