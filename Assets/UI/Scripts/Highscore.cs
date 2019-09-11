public class Highscore {
    private Save playerSave;
    private int maxHighscorePlace = 10;
    public int PlayerHighscorePlace { get; private set; }
    public int PlayerHighscoreIndex { get; private set; }

    public Highscore(int highscoreLength = 10) {
        maxHighscorePlace = highscoreLength;
    }

    public Save[] GetOrderedHighscore() {
        Save[] saves = SaveManager.LoadHighScoreTable();
        OrderHighscore(ref saves);
        return saves;
    }

    public Save[] GetOrderedHighscoreWithPlayer() {
        Save[] saves = SaveManager.LoadHighScoreTable();
        AddPlayerScore(ref saves);
        OrderHighscore(ref saves);
        SetPlayerHighscorePlaceAndIndex(saves);
        InsurePlayerIsInTheScoreListShown(ref saves);
        return saves;
    }

    private void AddPlayerScore(ref Save[] saves) {
        playerSave.name = "Insert your name";
        playerSave.value = StreakCounterUI.GetHighScore();
        int savesLength = saves.Length + 1;
        Save[] newSave = new Save[savesLength + 1];
        for(int i = 0; i < savesLength - 1; i++) {
            newSave[i] = saves[i];
        }
        newSave[savesLength - 1] = playerSave;
        saves = newSave;
    }

    private void OrderHighscore(ref Save[] saves) {
        for(int i = 1; i < saves.Length; i++) {
            for(int j = 0; j < i; j++) {
                if(saves[j].value < saves[i].value) {
                    Save tempSave = saves[j];
                    saves[j] = saves[i];
                    saves[i] = tempSave;
                }
            }
        }
    }

    private void SetPlayerHighscorePlaceAndIndex(Save[] saves) {
        int index = -1;
        for(int i = 0; i < saves.Length; i++) {
            if(saves[i].name == playerSave.name) {
                index = i;
                break;
            }
        }

        PlayerHighscorePlace = index;
        PlayerHighscoreIndex = index >= maxHighscorePlace ? maxHighscorePlace - 1 : PlayerHighscorePlace;
    }

    private void InsurePlayerIsInTheScoreListShown(ref Save[] saves) {
        if(PlayerHighscorePlace < maxHighscorePlace) {
            return;
        }
        Save tempSave = saves[maxHighscorePlace - 1];
        saves[maxHighscorePlace - 1] = playerSave;
        saves[PlayerHighscorePlace] = tempSave;
    }
}