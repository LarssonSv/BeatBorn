using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveData: MonoBehaviour {
    private void Start() {
        RoundController.OnRoundStartInt += SaveThreatLevel;
    }

    private void SaveHighscore(string name) {
        SaveManager.SaveHighscore(name, StreakCounterUI.scoreToPrint);
    }

    public void SaveHighscore(string name, float value) {
        SaveManager.SaveHighscore(name, value);
    }

    private void SaveThreatLevel(int number) {
        int lastSave = SaveManager.LoadThreatLevel();
        int n = number / 5;
        if(lastSave > n) {
            return;
        }

        SaveManager.SaveThreatLevel(n);
    }

    public static int LoadThreatLevel() {
        return SaveManager.LoadThreatLevel();
    }
}