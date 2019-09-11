using FMOD;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ThreatMenu : BaseMenu<ThreatMenu> {
    public void ThreatOne() {
        FindObjectOfType<LoadSettingsOnStartUp>().StartUpThreatLevel = 0;
        LoadScene();
        PlayerPrefs.SetInt("ThreatLevel", 0);

    }

    public void ThreatTwo() {
        FindObjectOfType<LoadSettingsOnStartUp>().StartUpThreatLevel = 1;
        PlayerPrefs.SetInt("ThreatLevel", 1);
        LoadScene();
    }

    public void ThreatThree() {
        FindObjectOfType<LoadSettingsOnStartUp>().StartUpThreatLevel = 2;
        PlayerPrefs.SetInt("ThreatLevel", 2);
        LoadScene();
    }

    private void LoadScene() {
        MenuManager.Instance.CloseAllMenus();
        int sceneToMoveTo = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneManager.sceneCountInBuildSettings >= sceneToMoveTo) {
            SceneManager.LoadScene(sceneToMoveTo);
        } else {
            UnityEngine.Debug.Log("There is no scene assigned in build settings!");
        }
    }
}