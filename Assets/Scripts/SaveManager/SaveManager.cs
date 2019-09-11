using System;
using System.IO;
using UnityEngine;

public static class SaveManager {
    private static string threatLevel = "/ThreatlLevel.json";
    private static string highscoreLevel = "/HighscoreTable.json";
    private static string settings = "/Settings.json";

    public static void SaveThreatLevel(float value) {
        File.WriteAllText(Application.persistentDataPath + threatLevel, value.ToString());
    }

    public static int LoadThreatLevel() {
        int value = 0;
        try {
            string file = File.ReadAllText(Application.persistentDataPath + threatLevel);
            bool success = int.TryParse(file, out value);
            if(!success) {
                Debug.Log("Couldn't load saved file");
                return -1;
            }
        } catch {
            Debug.Log("File does not exist");
            return value;
        }
        return value;
    }

    public static void ResetThreatLevel(string name) {
        ResetFile(Application.persistentDataPath + threatLevel);
    }

    public static void SaveNewHighscoreTable(Save[] saves) {
        ResetHighscore();           
        string json = saves[0].name + "/" + saves[0].value.ToString() + ";";
        File.WriteAllText(Application.persistentDataPath + highscoreLevel, json);
        for(int i = 1; i < saves.Length; i++) {
            json = saves[i].name + "/" + saves[i].value.ToString() + ";";
            File.AppendAllText(Application.persistentDataPath + highscoreLevel, json);
        }
    }

    public static void SaveHighscore(string name, double value) {
        if(value == 0) {
            return;
        }
        string json = name + "/" + value.ToString() + ";";
        try {
            File.ReadAllText(Application.persistentDataPath + highscoreLevel);
            File.AppendAllText(Application.persistentDataPath + highscoreLevel, json);
        } catch {
            File.WriteAllText(Application.persistentDataPath + highscoreLevel, json);
        }
    }

    public static void SaveHighscore(Save save) {
        SaveManager.SaveHighscore(save.name, save.value);
    }

    public static Save[] LoadHighScoreTable() {
        Save[] saves = new Save[0];
        string file;
        try {
            file = File.ReadAllText(Application.persistentDataPath + highscoreLevel);
        } catch {
            Debug.Log("File does not exist");
            return saves;
        }
        string[] split = file.Split(';');
        saves = new Save[split.Length];
        int index = 0;
        foreach(string spl in split) {
            if(spl == "") {
                continue;
            }
            string[] save = spl.Split('/');
            string name = save[0];
            if(name == "") {
                continue;
            }
            double value;
            if(!double.TryParse(save[1], out value)) {
                Debug.Log("Couldn't parse double in load highscore table" + save[1]);
                continue;
            }
            saves[index] = new Save(name, value);
            ++index;
        }
        return saves;
    }

    public static bool DeleteOneHighscoreSave(string name) {
        Save[] saves = new Save[10];
        try {
            string file = File.ReadAllText(Application.persistentDataPath + highscoreLevel);
            if(!file.Contains(name)) {
                Debug.Log("There is no such name in the highscore table");
                return false;
            }

            int q = file.IndexOf(name);
            int w = file.IndexOf(";", q);
            int stringLength = w - q + 1;
            file = file.Remove(q, stringLength);
            File.WriteAllText(Application.persistentDataPath + highscoreLevel, file);
            return true;
        } catch {
            Debug.Log("File does not exist");
            return false;
        }
    }

    public static void ResetHighscore() {
        ResetFile(Application.persistentDataPath + highscoreLevel);
    }

    private static void ResetFile(string path) {
        File.Delete(path);
    }

    public static void SaveSettings(string name, float value) {
        string file;
        string json = name + "/" + value.ToString() + ";";
        try {
            file = File.ReadAllText(Application.persistentDataPath + settings);
        } catch {
            File.WriteAllText(Application.persistentDataPath + settings, json);
            return;
        }
        if(!file.Contains(name)) {
            File.AppendAllText(Application.persistentDataPath + settings, json);
            return;
        }

        int q = file.IndexOf(name);
        int w = file.IndexOf(";", q);
        int stringLength = w - q + 1;
        file = file.Remove(q, stringLength);
        file += json;
        File.WriteAllText(Application.persistentDataPath + settings, file);
    }

    public static float LoadSetting(string name) {
        string file;
        float value = 1;
        try {
            file = File.ReadAllText(Application.persistentDataPath + settings);
        } catch {
            Debug.Log("File does not exist");
            return value;
        }

        if(!file.Contains(name)) {
            return value;
        }
        int q = file.IndexOf(name, StringComparison.Ordinal);
        string result = file.Substring(q + name.Length + 1);
        int w = result.IndexOf(";");
        result = result.Remove(w);
        if(!float.TryParse(result, out value)) {
            Debug.Log("Failed to load setting. Check SaveManager");
            value = 1;
        }
        return value;
    }
}

[System.Serializable]
public struct Save {
    public Save(string n, double v) {
        name = n;
        value = v;
    }
    public string name;
    public double value;
}