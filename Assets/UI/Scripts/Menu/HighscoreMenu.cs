using TMPro;
using UnityEngine;

public class HighscoreMenu: BaseMenu<HighscoreMenu> {
    [SerializeField] private Transform rank;
    [SerializeField] private Transform namePlayer;
    [SerializeField] private Transform score;
    private TextMeshProUGUI[] rankText;
    private TMP_InputField[] namePlayerText;
    private TextMeshProUGUI[] scoreText;
    private int maxHighscoreToShow = 10;

    private Save[] saves;
    private Save playerSave;
    private bool showHighscorePure;
    private int playerIndex;

    private void Start() {
        rankText = new TextMeshProUGUI[maxHighscoreToShow];
        namePlayerText = new TMP_InputField[maxHighscoreToShow];
        scoreText = new TextMeshProUGUI[maxHighscoreToShow];

        TextMeshProUGUI initialRank = rank.GetChild(0).GetComponent<TextMeshProUGUI>();
        TMP_InputField initialName = namePlayer.GetChild(0).GetComponent<TMP_InputField>();
        TextMeshProUGUI initialScore = score.GetChild(0).GetComponent<TextMeshProUGUI>();

        rankText[0] = initialRank;
        namePlayerText[0] = initialName;
        scoreText[0] = initialScore;

        for(int i = 1; i < maxHighscoreToShow; i++) {
            TextMeshProUGUI tempRank = Instantiate(initialRank, rank);
            TMP_InputField tempName = Instantiate(initialName, namePlayer);
            TextMeshProUGUI tempScore = Instantiate(initialScore, score);

            rankText[i] = tempRank;
            namePlayerText[i] = tempName;
            scoreText[i] = tempScore;
        }

        showHighscorePure = CameraController.Instance == null;
        ShowHighscore();
    }

    private void ShowHighscore() {
        Highscore score = new Highscore();
        saves = showHighscorePure ? score.GetOrderedHighscore() : score.GetOrderedHighscoreWithPlayer();
        int savesLength = saves.Length;
        for(int i = 0; i < maxHighscoreToShow; i++) {
            if(i >= savesLength || saves[i].value == 0) {
                rankText[i].text = (i + 1).ToString();
                namePlayerText[i].text = "Team 1 player";
                scoreText[i].text = "∞";
                continue;
            }
            rankText[i].text = (i + 1).ToString();
            namePlayerText[i].text = saves[i].name;
            scoreText[i].text = saves[i].value.ToString();
        }

        if(showHighscorePure) {
            return;
        }

        playerIndex = score.PlayerHighscoreIndex;
        playerSave = saves[playerIndex];

        rankText[playerIndex].text = (score.PlayerHighscorePlace + 1).ToString();
        namePlayerText[playerIndex].text = playerSave.name;
        scoreText[playerIndex].text = playerSave.value.ToString();

        namePlayerText[playerIndex].onEndEdit.AddListener(delegate { SaveHighscore(); });
        namePlayerText[playerIndex].interactable = true;
        namePlayerText[playerIndex].Select();
    }

    public void SaveHighscore() {
        if(!showHighscorePure && namePlayerText[playerIndex].text.Length > 0) {
            playerSave.name = namePlayerText[playerIndex].text;
            SaveManager.SaveHighscore(playerSave);
        }

        if(MenuManager.Instance) {
            MenuManager.Instance.Close(this);
        }
    }
}