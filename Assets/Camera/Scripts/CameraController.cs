#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Analytics;

public class CameraController : MonoBehaviour {
    private Transform target;
    private PlayerEngine player;
    private Vector3 targetPrevPosition;

    [Header("Input settings")]
    [Tooltip("How quickly camera rotates")]
    [SerializeField] private float xRotateSpeed = 200f;
    [Tooltip("How quickly input gravitate to zero when released")]
    [SerializeField] private float inputSmooth = 1;
    [SerializeField] private bool invertX = false;
    private float inputX;

    [Header("Follow settings")]
    [Tooltip("How quickly camera should follow the player")]
    [SerializeField] private float folowSmooth = 1;

    [Header("Camera states")]
    [Tooltip("Put in order: Idle, Attack, Dash")]
    [SerializeField] private CameraDefault[] states;
    private bool playerAggroState = false;
    private float aggroTime = 1f;
    private float aggrotimeHolder;

    private enum CameraState { Idle, Attack, Cinematic };
    private CameraState currentState = CameraState.Idle;
    private CameraState prevState;

    [Header("Other settings")]
    [Tooltip("Layers for camera to collide with")]
    [SerializeField] private LayerMask collisionLayer = 1;
    private float xDeg = 0f;

    private bool playCinematic = false;
    private bool gameOver = false;

    public static CameraController Instance;

    private void Start() {
        Instance = this;
        player = FindObjectOfType<PlayerEngine>();
        if(!player) {
            Debug.Log("There is no player in this scene.");
            return;
        }
        target = player.transform;
        xDeg = transform.eulerAngles.x;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        for(int i = 0; i < states.Length; i++) {
            states[i].Init(target, collisionLayer, transform);
        }
        aggroTime = states[1].CombatDuration;

        StartCameraBehindPlayer();
        player.OnAttackedPressed += PlayerAttacked;

        CoreHealth.Instance.coreDied += GameOver;
    }

    private void Update() {
        if(!EverythingWorks()) {
            return;
        }
        if(Input.GetAxis("Mouse X") != 0) {
            inputX = Input.GetAxis("Mouse X");
        }
        xDeg += inputX * xRotateSpeed * Time.deltaTime * (invertX ? -1 : 1);
        inputX = Mathf.Lerp(inputX, 0, Time.deltaTime * inputSmooth);

        targetPrevPosition = Vector3.Lerp(targetPrevPosition, target.position, Time.deltaTime * folowSmooth);

        if(!target) {
            return;
        }

        if(aggrotimeHolder <= Time.time) {
            playerAggroState = false;
        }

        CheckUpdateState();
        states[(int)currentState].UpdateState(targetPrevPosition, xDeg, player.Velocity.NearZero());
    }

    public void PlayCinematic() {
        playCinematic = true;
    }

    public void StopCinematic() {
        playCinematic = false;
    }

    private void PlayerAttacked() {
        playerAggroState = true;
        aggrotimeHolder = aggroTime + Time.time;
    }

    public void ChangeStateToIdle() {
        aggrotimeHolder = 0;
    }

    private void CheckUpdateState() {
        bool stateChanged = false;
        switch(currentState) {
            case CameraState.Idle:
                if(playerAggroState) {
                    prevState = currentState;
                    currentState = CameraState.Attack;
                    stateChanged = true;
                }
                if(playCinematic) {
                    prevState = currentState;
                    currentState = CameraState.Cinematic;
                    stateChanged = true;
                }
                break;
            case CameraState.Attack:
                if(!playerAggroState) {
                    prevState = currentState;
                    currentState = CameraState.Idle;
                    stateChanged = true;
                }
                if(playCinematic) {
                    prevState = currentState;
                    currentState = CameraState.Cinematic;
                    stateChanged = true;
                }
                break;
            case CameraState.Cinematic:
                if(!playCinematic) {
                    prevState = currentState;
                    currentState = CameraState.Idle;
                    stateChanged = true;
                }
                break;

        }

        if(stateChanged) {
            states[(int)currentState].Transition(states[(int)prevState], states[(int)prevState].currentDistance, states[(int)prevState].desiredDistance, states[(int)prevState].correctedDistance);
        }
    }

    private void StartCameraBehindPlayer() {
        xDeg = Quaternion.LookRotation(target.forward).eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(20, xDeg, 0);
        transform.rotation = rotation;
        Vector3 vTargetOffset = new Vector3(0, -2, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * 7 + vTargetOffset);
        transform.position = position;
    }

    public void UpdateValues() {
        aggroTime = states[1].CombatDuration;
    }

    private void GameOver() {
        gameOver = true;
    }

    private bool EverythingWorks() {
        if(!target) {
            return false;
        }
        if(PauseGame.IsPaused()) {
            return false;
        }

        if(gameOver) {
            return false;
        }

        return true;
    }

    private void OnDisable() {
        if(FindObjectOfType<PlayerEngine>() != null) {
            FindObjectOfType<PlayerEngine>().OnAttackedPressed -= PlayerAttacked;
        }
    }
}