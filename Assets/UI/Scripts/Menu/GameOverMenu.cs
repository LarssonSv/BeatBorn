using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu: BaseMenu<GameOverMenu> {
    public void RestartFromThreatLevel() {
        StaticClearer.ClearAll();
        int index = FindObjectOfType<RoundController>().CurrectRoundIndex;
        FindObjectOfType<LoadSettingsOnStartUp>().StartUpThreatLevel = index / 5;
        RestartGame();
    }

    public void RestartGame() {
        StaticClearer.ClearAll();
        MenuManager.Instance.CloseAllMenus();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu() {
        StaticClearer.ClearAll();
        
        if (MenuManager.Instance) {
            MenuManager.Instance.CloseAllMenus();
            GameObject go = MenuManager.Instance.gameObject;
            Destroy(go);
        }

        int sceneToMoveTo = SceneManager.GetActiveScene().buildIndex - 1;

        if (sceneToMoveTo >= 0) {
            SceneManager.LoadScene(sceneToMoveTo);
        } else {
            UnityEngine.Debug.Log("There is no scene assigned in build settings!");
        }
    }
}