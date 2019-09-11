#pragma warning disable 0649
using System;
using System.Collections;
using StateKraft;
using UnityEngine;

public class PlayerEngine : MonoBehaviour, IDamageable {

    [Header("Player Values:")]
    public float RegenTime;
    public float LifeTick;
    public int MaxHP;
    public float InvincibilityTime;
    public Vector3 Velocity;
    public LayerMask MovementMask;
    public Vector2 DirectionInput;

    [Header("Components/References:")]
    public GameObject SpawnPoint;
    public Transform SwordTransform;
    public Camera CameraController;
    [NonSerialized] public CapsuleCollider Collider;

    [Header("VFXs:")]
    [NonSerialized] public EmissionBurster SwordBurstVFX;
    public ParticleSystem SwordSwingVFX;
    public ParticleSystem dashElectricVFX;
    public GameObject DashVFX;

    [Header("Machine")]
    [SerializeField] private StateMachine movementMachine;

    [HideInInspector] public Animator CharacterAnimator;
    private float lastDamagedTime;
    private Transform characterTransform;
    public int CurrentHP {
        get { return currentHP; }
        set {
            currentHP = value;
            OnLifeChanged?.Invoke(currentHP / (MaxHP * 1f));
        }
    }
    private int currentHP;
    private float lifeTickTimer;
    private MaterialFloatLerper _hurtLerper;
    public Action<IDamageable> OnAttackHitt;
    public Action OnAttackedPressed;
    public static Action<float> OnLifeChanged;
    public Action OnPlayerDeath;
    public Action OnPlayerRespawn;
    public float Friction = 5;
    public float TurnFriction = 100;

    private static PlayerEngine _instance;
    public static State CurrentState => _instance.movementMachine.CurrentState;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        movementMachine.Initialize(this);
        characterTransform = CharacterAnimator.transform;
        Collider = GetComponent<CapsuleCollider>();
        SwordBurstVFX = GetComponentInChildren<EmissionBurster>();
        CameraController = FindObjectOfType<Camera>();
        CurrentHP = MaxHP;
        _hurtLerper = GetComponentInChildren<MaterialFloatLerper>();
    }
    void Update() {

        if (PauseGame.IsPaused() || GameManager.CurrentState is CinematicState)
            return;

        Move();
        movementMachine.Update();

        movementMachine.GetState<PlayerDashState>().Cooldowns();

        CheckIfInBounds();

        if (Time.time  - lastDamagedTime >= RegenTime && !(movementMachine.CurrentState is PlayerDeathState) && CurrentHP < MaxHP)
        {
            lifeTickTimer += Time.deltaTime;
            if (lifeTickTimer >= LifeTick)
            {
                CurrentHP++;
                lifeTickTimer -= LifeTick;
            }
        }
    }
    public void AddDirectionInput(Vector2 input) {
        DirectionInput = input;
    }
    private void Move() {
        if (!(movementMachine.CurrentState is PlayerAttackState) && !(movementMachine.CurrentState is PlayerKnockBackState))
            characterTransform.LookAt(characterTransform.position + Vector3.ProjectOnPlane(Velocity, Vector3.up));

        Debug.DrawRay(transform.position, Velocity, Color.blue);
        if (PlayerAttackState.IsAttackFreeze)
        {
//            Debug.Log("hamemmr timre");
            return;
        }
         
        
        MathHelpers.PreventCollision(Movementraycast, ref Velocity, transform, Time.deltaTime, 0.05f);
        transform.position += Velocity * Time.deltaTime;
        /*
        if (!(movementMachine.CurrentState is PlayerAttackState))
            transform.position += Mathf.Approximately(Velocity.normalized.magnitude, 0f) ? Vector3.zero : Velocity * Time.deltaTime;
        */
        
    }
    
    private RaycastHit Movementraycast()
    {
        RaycastHit hit;
        Physics.CapsuleCast(transform.position + Collider.radius * Vector3.up,
            transform.position + Vector3.up * (Collider.height - Collider.radius), Collider.radius, Velocity.normalized, out hit, float.MaxValue, MovementMask);
        if (hit.collider != null && hit.transform == transform)
            hit = new RaycastHit();
        return hit;
    }
    
    
    /*
    public Vector3 ApplyNormalForce(Vector3 vector, float skinWidth = 0.05f) {
        Vector3 point1 = transform.position + Collider.radius * Vector3.up;
        Vector3 point2 = transform.position + (Collider.height - Collider.radius) * Vector3.up;

        int counter = 0;
        RaycastHit hit = new RaycastHit();
        Physics.CapsuleCast(point1, point2, Collider.radius, vector.normalized, out hit, 1000, MovementMask);
        while (hit.collider != null && counter++ < 100) {
            float distanceToCorrect = skinWidth / Vector3.Dot(vector.normalized, hit.normal);
            float distanceToMove = hit.distance + distanceToCorrect;

            if (distanceToMove > vector.magnitude * Time.deltaTime) break;
            if (distanceToMove > 0.0f) transform.position += vector.normalized * distanceToMove;

            float dot = Vector3.Dot(hit.normal, vector);
            vector -= hit.normal * (dot < 0.0f ? dot : 0.0f);
            Physics.CapsuleCast(point1, point2, Collider.radius, vector.normalized, out hit, 1000, MovementMask);

        }
        return vector;
    } */
    public bool CheckSyncPower() {
        if (Input.GetButtonDown("Super") && SyncPowerManager.CurrentSyncProcent() >= movementMachine.GetState<PlayerSuperState>().Supers[0].SyncPercentage) {
            movementMachine.TransitionTo<PlayerSuperState>();
            return true;
        }
        return false;
    }

    IEnumerator DelayedDashVFXStop()
    {
        yield return new WaitForSeconds(0.2f);
        dashElectricVFX.Stop();
    }

    public void TakeDamage(int damage = 1, RhythmGrade grade = RhythmGrade.Miss, Transform attacker = null, float MissMultiplier =  1)
    {
        if (Time.time - lastDamagedTime <= InvincibilityTime || movementMachine.CurrentState is PlayerSuperState)
            return;


        lastDamagedTime = Time.time;
        _hurtLerper.LerpValue(true);
        CurrentHP -= damage;
        if (CurrentHP <= 0)
            Die();
        else {
            if (attacker) {
                Vector3 toAttacker = attacker.position - transform.position;
                toAttacker.y = 0;
                toAttacker = toAttacker.normalized;
                toAttacker = characterTransform.InverseTransformDirection(toAttacker);
                CharacterAnimator.SetFloat("DamageDirectionX", toAttacker.x * -1f);
                CharacterAnimator.SetFloat("DamageDirectionY", toAttacker.z * -1f);
            }
            movementMachine.TransitionTo<PlayerKnockBackState>();
        }
        HUDManager.Instance.UpdatePlayerHPUI(MaxHP,currentHP);
    }
    
    public void Die() {
        if (!(movementMachine.CurrentState is PlayerDeathState))
            movementMachine.TransitionTo<PlayerDeathState>();

    }

    public void ResetPlayer()
    {
        movementMachine.TransitionTo<PlayerSpawningState>();
    }

    private void CheckIfInBounds()
    {
        if(transform.position.y < -100f)
        {
            Debug.Log("Johan och simon tänkte på denna bug haha");
            transform.position = SpawnPoint.transform.position + new Vector3(0f,10f,0f);
        }
    }

}
