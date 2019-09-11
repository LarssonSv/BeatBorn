using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu: BaseMenu<MainMenu> {
    public void StartGame() {
        GameObject go = GameObject.Find("MenuMusic");
        if(go) {
            Destroy(go);
        }
        MenuManager.Instance.CloseAllMenus();
        FindObjectOfType<AnimaticController>().PlayAnimaticAndLoadSceene();
    }

    public void Options() {
        MenuManager.Instance.Create<OptionsMenu>();
    }

    public void Threat() {
        MenuManager.Instance.Create<ThreatMenu>();
    }

    public void Credits() {
        MenuManager.Instance.Create<CreditsMenu>();
    }

    public void Highscore() {
        MenuManager.Instance.Create<HighscoreMenu>();
    }

    public override void Back() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }
}