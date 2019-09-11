using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[System.Serializable]
public struct PopupData
{
    public Sprite sprite;
    public string message;
    public TutorialInputType inputType;
}

public enum Enemy { basicAi, hunterAi, shooterAi }
public class RoundController: MonoBehaviour {
    [SerializeField] private float PreRoundDelay = 5f;
    [SerializeField] private float SpawnRadius = 5f;
    [SerializeField] private Transform[] spawnPoints = new Transform[5];
    [SerializeField] public List<Round> rounds = new List<Round>();
    public bool StartOnAwake = true;
    private bool isSpawning;
    public static int EnemiesKilled = 0;
    public static Action<Round> OnRoundWamup;
    public static Action<Round> OnRoundStart;
    public static Action<int> OnRoundStartInt;
    public static Action<PopupData> OnPopupEvent;
    public int CurrectRoundIndex => rIndex;
    List<SpawnData> spawnQue = new List<SpawnData>();

    private float timer = 0;
    private int rIndex = -1;
    private int sIndex = 0;
    private int alive = 0;
    private ObjectPooler OP;
    private State currentState = State.NewRound;
    private static RoundController instance;

    public enum slotFunction { Spawn, Wait, WaveClear, Sound, PopUp, ShowScore }
    private enum State { NewRound, Playing, Paused, GameOver }

    public enum SpawnPoint { Spawner_0, Spawner_1, Spawner_2, Spawner_3, Spawner_4, Spawner_5 }

    private float spawnTimer;

    private Round currentRound => rounds[rIndex];
    private Slot currentSlot => rounds[rIndex].slots[sIndex];

    private List<SpawnIndicator> spawnGates = new List<SpawnIndicator>();

    public struct SpawnData
    {
        public int spawnIndex;
        public Enemy type;
    }


    [System.Serializable]
    public class Slot {
        public slotFunction function;
        //Spawner
        public Enemy EnemyType;
        public SpawnPoint SpawnPosition;
        public int SpawnAmount;
        public float SpawnTime;

        //Waiter
        public float WaitFor;

        //Canvas
        public Sprite sprite;
        public string message;
        public TutorialInputType type;
        //Sound
        [EventRef] public string EventRef;

    }

    private void Start() {
        instance = this;
        OP = ObjectPooler.Instance;
        LoadSettingsOnStartUp settings = FindObjectOfType<LoadSettingsOnStartUp>();
        if(settings) {
            rIndex = settings.StartUpThreatLevel * 5 - 1;
        }

        foreach(Transform spawner in spawnPoints) {
            SpawnIndicator indicator = spawner.transform.GetChild(0).GetComponent<SpawnIndicator>();
            if(indicator) {
                spawnGates.Add(indicator);
            }
        }

    }

    private void Update() {
        if(PauseGame.IsPaused() || !isSpawning || StartOnAwake)
            return;

        switch(currentState) {
            case State.NewRound:
                StartNewRound();
                break;
            case State.Playing:
                Play();
                break;
            case State.Paused:
                WaitForClear();
                break;
            case State.GameOver:
                //                Debug.Log("GameOver!");
                break;

        }

        if(spawnQue.Count > 0 && spawnTimer <= 0)
        {
            SpawnTest();
        }
        else if(spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
        }


    }

    public void StartRound() {
        isSpawning = true;
    }

    public static void PauseRound(bool value = false) {
        instance.isSpawning = !value;
        //UGLY CODE BUT FUCK ITTTT - ya boi simon
    }

    private void StartNewRound() {
        OnRoundWamup?.Invoke(rounds[rIndex + 1]);
        if(timer > PreRoundDelay) {
            if(rounds.Count > rIndex + 1) {
                rIndex++;
                sIndex = 0;
                timer = 0;
                OnRoundStart?.Invoke(currentRound);
                OnRoundStartInt?.Invoke(rIndex);
                currentState = State.Playing;
            } else {
                currentState = State.GameOver;
            }

        } else {
            timer += Time.deltaTime;
        }
    }

    private void Play() {
        if(timer > currentSlot.SpawnTime) {
            switch(currentSlot.function) {
                case slotFunction.Spawn:
                    SpawnObject();
                    break;
                case slotFunction.Sound:
                    PlaySound();
                    break;
                case slotFunction.Wait:
                    AddWait();
                    break;
                case slotFunction.WaveClear:
                    currentState = State.Paused;
                    break;
                case slotFunction.PopUp:
                    OpenUIByTag();
                    break;
                case slotFunction.ShowScore:
                    ShowScoreScreen();
                    break;

            }

        }
        timer += Time.deltaTime;
    }

    private void WaitForClear() {
        if(alive <= 0) {
            currentState = State.Playing;
            NextSlot();
        }
    }

    private void AddWait() {
        timer -= currentSlot.WaitFor;
        Debug.Log("Added wait!");
        NextSlot();
        currentState = State.Playing;
    }

    private void PlaySound() {
        RuntimeManager.PlayOneShot(currentSlot.EventRef);
        NextSlot();
    }

    private void ShowScoreScreen()
    {
        PauseRound();
        ScoreScreenHandler.TurnOnOff(true, "Victory");
        NextSlot();
    }

    private void OpenUIByTag() {

        PopupData tempData = new PopupData();
        tempData.message = currentSlot.message;
        tempData.sprite = currentSlot.sprite;
        tempData.inputType = currentSlot.type;
        OnPopupEvent?.Invoke(tempData);

        NextSlot();
    }


    private void NextSlot() {
        if(currentRound.slots.Count > sIndex + 1) {
            sIndex++;
        } else {
            currentState = State.NewRound;
        }

    }

    private void SpawnObject() {

        for (int i = 0; i < currentSlot.SpawnAmount; i++)
        {
            SpawnData tempData = new SpawnData();
            tempData.type = currentSlot.EnemyType;
            tempData.spawnIndex = (int)currentSlot.SpawnPosition;
            spawnQue.Add(tempData);
        }

        NextSlot();
    }

    public void ObjectDied() {
        EnemiesKilled++;
        alive--;
    }

    public void ResetRound() {
        AiController[] allEnemies = FindObjectsOfType<AiController>();

        foreach(AiController controller in allEnemies) {
            controller.gameObject.SetActive(false);
        }

        timer = 0;
        rIndex = -1;
        sIndex = 0;
        alive = 0;

        SyncPowerManager.SetSyncPower(0f);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEngine>().ResetPlayer();
        BeatManager.Instance.Setup();
        currentState = State.NewRound;

    }


    private void SpawnTest()
    {
            spawnTimer = 0.5f;
            spawnGates[spawnQue[0].spawnIndex].StartIndication();

            Vector3 spawnLocation = spawnPoints[spawnQue[0].spawnIndex].position;
            spawnLocation.x += UnityEngine.Random.Range(-SpawnRadius, SpawnRadius);
            spawnLocation.z += UnityEngine.Random.Range(-SpawnRadius, SpawnRadius);

            GameObject clone = OP.SpawnFromPool(spawnQue[0].type.ToString(), spawnLocation, Quaternion.identity);
            AiController aiController = clone.GetComponent<AiController>();
            aiController.Setup(spawnLocation);
            aiController.OnDeath += ObjectDied;
            alive++;
        spawnQue.RemoveAt(0);

    }


}
