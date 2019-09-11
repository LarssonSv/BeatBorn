using UnityEngine;
using TMPro;
using FMODUnity;
using UnityEngine.Playables;


public class ScoreScreenHandler : MonoBehaviour
{
    private float cinematicEnd;
    private static ScoreScreenHandler instance;
    [EventRef] public string ScoreHappenedSFX;

    [SerializeField] private TextMeshProUGUI gameState;
    [SerializeField] private TextMeshProUGUI CurrentScore;
    [SerializeField] private TextMeshProUGUI EnemyKills;
    [SerializeField] private TextMeshProUGUI SyncedHits;
    [SerializeField] private TextMeshProUGUI BestHitStreak;
    [SerializeField] private TextMeshProUGUI Deaths;
    [SerializeField] private TextMeshProUGUI TotalScore;
    [SerializeField] private PlayableAsset gameOverCinematic;

    private float topHitsScore;
    private float scoreToPrintScore;
    private float enemiesKileldScore;
    private float hitsOnBeatScore;
    private float deathcountScore;
    private float superTotalScore;
    public static float SuperScore => instance.superTotalScore;


    private void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        instance = this;
        instance.gameObject.SetActive(false);
     //   instance.enabled = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            gameObject.SetActive(false);
            instance.enabled = false;
            MenuManager.Instance.Create<GameOverMenu>();
            MenuManager.Instance.Create<HighscoreMenu>();
        }
    }

    public static void TurnOnOff(bool setActive, string gameState = "GAME OVER")
    {
        BeatManager.StopMusic();
        instance.gameObject.SetActive(setActive);
        instance.enabled = true;
        Debug.Log("We are scoreer");
        if (setActive)
        {
            //FindObjectOfType<PlayerEngine>().gameObject.SetActive(false);

            BeatManager.StopMusic();
            RoundController.PauseRound(true);
            instance.gameState.text = gameState;
            instance.CurrentScore.text = StreakCounterUI.scoreToPrint.ToString();
            instance.EnemyKills.text = RoundController.EnemiesKilled.ToString();
            instance.SyncedHits.text = StreakCounterUI.HitsOnBeat.ToString();
            instance.BestHitStreak.text = StreakCounterUI.TopHitsInArow.ToString();
            instance.Deaths.text = StreakCounterUI.DeathCount.ToString();
            
            instance.scoreToPrintScore = StreakCounterUI.scoreToPrint;
            instance.topHitsScore = StreakCounterUI.TopHitsInArow; //
            instance.enemiesKileldScore =  RoundController.EnemiesKilled;//
            instance.hitsOnBeatScore = StreakCounterUI.HitsOnBeat; //
            instance.deathcountScore = StreakCounterUI.DeathCount; //
            instance.Invoke("CalculateScore", 0.5f);
        }

    }


    private void CalculateScore()
    {
        RuntimeManager.PlayOneShot(ScoreHappenedSFX);
        instance.deathcountScore = instance.deathcountScore * 2000;
        instance.topHitsScore = instance.topHitsScore * 100;
        instance.enemiesKileldScore = instance.enemiesKileldScore * 100;
        instance.hitsOnBeatScore = instance.hitsOnBeatScore * 100;
        instance.superTotalScore = instance.scoreToPrintScore - instance.deathcountScore + instance.topHitsScore + instance.enemiesKileldScore + instance.hitsOnBeatScore;
        instance.TotalScore.text = instance.superTotalScore.ToString();

        StreakCounterUI.superDuperUltraScore = superTotalScore;
    }


}
