using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "States/PlayerMovement/AttackState")]
public class PlayerAttackState : PlayerState
{
    public GameObject ParticleSpawner;
    public LayerMask AttackLayer;
    public float SnapRadius;
    public float PreferedDistance;
    public float FreezeTime;
    public float IgnoreSnapDistance;
    [Range(0, 360)] public float SnapAngle;
    public float SnapTime;
    public int attackAnimLayer;
    public float RotationSpeed = 10f;
    public static RhythmGrade currentAttackGrade { get; private set; }
    public static Action<string> OnComboExecuted;
    public static Action<RhythmGrade> OnAttack;
    public float FreezeCooldown;
    [Range(0f, 1f)] public float MissFactor;
    [Serializable]
    public struct Combo
    {
        public string Name;
        public AttackStruct[] Attacks;
    }

    [Serializable]
    public struct AttackStruct
    {


        [Header("Animation"), Tooltip("This is the name of the animation state in the animator and the name of the trigger as well")]
        public string AttackTag;
        [Tooltip("This clip determines the time for a animation")] public AnimationClip Clip;
        public bool IgnoreThisData;
        [Header("Attack")]
        [Tooltip("These are the frames the attack does damage")]
        public Vector2Int attackWindow;
        public Vector2Int VfxWindow;
        public float InputBuffer;
        public int Damage;
        [Range(0, 10)] public float GravityFactor;

        [Range(-180f, 180f)] public float AngleOffset;
        [Range(0f, 360f)] public float Angle;

        public float CloseAttackRange;
        [Range(0f, 360f)] public float CloseAttackAngle;
        [Range(-180f, 180f)] public float CloseAttackAngleOffset;

        public float Radius;
        public InputButton Input;

        [Header("Movement")]
        [Tooltip("If rootmotion is applied player will take no input during that attack")] public bool RootMotion;
        public bool ShouldNotRotateInAttack;

        [Header("Feedback")]
        public VFXStruct HitParticle;
        public Material VFXSwingMaterial;

        [Header("Sound")]
        [EventRef] public string AttackSFX;
        [EventRef] public string HitSFX;

        public enum InputButton
        {
            Light, Heavy, Dash
        }
    }

    public bool TakesInput = true;
    [SerializeField] private Combo[] Combos;
    public int AttackIndex = 0;
    private List<Combo> RelevantCombos;
    private Vector3 swordLastPos;
    private List<Transform> hitObjects;
    [SerializeField] private VFXStruct bloodSplat;
    private Vector3 playerLastPos;
    private float timer = 0;

    private AttackStruct.InputButton _bufferdInput;
    private bool hasGivenInput;

    private float currentClipLength => RelevantCombos[0].Attacks[AttackIndex].Clip.length;
    private int currentFrame => (int)(timer * CurrentAttacks[AttackIndex].Clip.frameRate);

    private bool inAttackWindow => currentFrame >= CurrentAttacks[AttackIndex].attackWindow.x &&
                                   currentFrame <= CurrentAttacks[AttackIndex].attackWindow.y;

    private AttackStruct CurrentAttack => CurrentAttacks[AttackIndex];

    private float VFXSimulationSpeed => 1f / ((CurrentAttacks[AttackIndex].VfxWindow.y - CurrentAttacks[AttackIndex].VfxWindow.x) / CurrentAttacks[AttackIndex].Clip.frameRate);

    public AttackStruct[] CurrentAttacks => RelevantCombos[0].Attacks;

    private bool hasStarted;
    private bool firstFrame;
    private Vector3 halfExtends;
    private string dashButtonString = "Dash";
    public static bool IsAttackFreeze = false;
    private float freezTimer = 0;
    private float timeOfLastFreeze;
    private float timeOfLastAttack;
    private Material swordSwingMat;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        swordSwingMat = SwordSwingVFX.GetComponent<ParticleSystemRenderer>().material;
    }


    public override void Enter()
    {
        timeOfLastFreeze = 0;
        IsAttackFreeze = false;
        freezTimer = 0;

        if (RelevantCombos == null)
        {
            Debug.Log("O noes the relevanbt stuff is not relavant");
            RelevantCombos = Combos.ToList();
        }

        if (Time.time - timeOfLastAttack > CurrentAttack.InputBuffer)
        {
            AttackIndex = 0;
            RelevantCombos = Combos.ToList();
        }
        else
            AttackIndex++;

        timer = 0;

        bool lightAttack = Input.GetButtonDown("LightAttack");


        SetRelevantCombos(lightAttack ? AttackStruct.InputButton.Light : AttackStruct.InputButton.Heavy);
        Velocity = Vector3.zero;
        PlayerAnimator.SetFloat("Velocity", 0);
        PlayerAnimator.SetTrigger(CurrentAttacks[AttackIndex].AttackTag);
        hitObjects = new List<Transform>();

        hasStarted = false;
        firstFrame = true;
        player.OnAttackedPressed?.Invoke();
        DoAttack();
    }



    public override void StateUpdate()
    {
        if (IsAttackFreeze)
        {
            if (freezTimer <= 0)
            {
                Time.timeScale = 1;
                IsAttackFreeze = false;
            }
            freezTimer -= Time.unscaledDeltaTime;
            return;
        }

        if (player.CheckSyncPower())
            return;

        if (Input.GetButtonDown("Dash"))
        {
            TransitionTo<PlayerDashState>();
            AttackIndex = 0;
            SetRelevantCombos(AttackStruct.InputButton.Light);
            hasGivenInput = false;
            return;
        }
            


        if (currentFrame > CurrentAttacks[AttackIndex].VfxWindow.x && !SwordSwingVFX.isPlaying && currentFrame < CurrentAttacks[AttackIndex].VfxWindow.y)
            SwordSwingVFX.Play();
        else if (currentFrame > CurrentAttacks[AttackIndex].VfxWindow.y)
        {
            SwordSwingVFX.Clear();
        }

        if (DirectionInput.magnitude > 0)
        {

            Vector3 lookDirection = Vector3.Lerp(PlayerAnimator.transform.position + PlayerAnimator.transform.forward,
                PlayerAnimator.transform.position + Vector3.ProjectOnPlane(GetInputRelativeToCamera(), Vector3.up),
                Time.deltaTime * RotationSpeed);
            PlayerAnimator.transform.LookAt(lookDirection, Vector3.up);

        }

        Attacking();
        if (RelevantCombos.Count == 0)
            return;
        CheckForMovementCollsion();


        timer += Time.deltaTime;
        firstFrame = false;

        if (!hasStarted)
        {
            hasStarted = PlayerAnimator.GetCurrentAnimatorStateInfo(attackAnimLayer).IsName(CurrentAttacks[AttackIndex].AttackTag)
                          || PlayerAnimator.GetNextAnimatorStateInfo(attackAnimLayer).IsName(CurrentAttacks[AttackIndex].AttackTag);
            return;
        }

        if (inAttackWindow)
            HighruleSwordCollision();


        if (timer < currentClipLength) return;

        TransitionTo<PlayerGroundState>();
    }

    private void Attacking()
    {


        if (timer <= CurrentAttack.attackWindow.y / CurrentAttack.Clip.frameRate - CurrentAttack.InputBuffer)
            return;

        if (Input.GetButtonDown("LightAttack"))
        {
            _bufferdInput = AttackStruct.InputButton.Light;
            hasGivenInput = true;
        }
        else if (Input.GetButtonDown("HeavyAttack"))
        {
            _bufferdInput = AttackStruct.InputButton.Heavy;
            hasGivenInput = true;
        }
        else if (Input.GetButtonDown("Dash"))
        {
            _bufferdInput = AttackStruct.InputButton.Dash;
            hasGivenInput = true;
        }


        if (firstFrame || currentFrame <= CurrentAttacks[AttackIndex].attackWindow.y || !hasGivenInput)
            return;



        switch (_bufferdInput)
        {
            case AttackStruct.InputButton.Light:
                AttackIndex++;
                SetRelevantCombos(AttackStruct.InputButton.Light);
                break;
            case AttackStruct.InputButton.Heavy:
                AttackIndex++;
                SetRelevantCombos(AttackStruct.InputButton.Heavy);
                break;
            case AttackStruct.InputButton.Dash:
                AttackIndex++;
                SetRelevantCombos(AttackStruct.InputButton.Dash);
                if (RelevantCombos.Count == 0)
                {
                    AttackIndex = 0;
                    SetRelevantCombos(AttackStruct.InputButton.Light);
                    TransitionTo<PlayerDashState>();
                    return;
                }
                break;
        }


        hasGivenInput = false;

        DoAttack();

    }

    private void DoAttack()
    {
        currentAttackGrade = BeatManager.GetGrade();
        SetupVFX();
        OnAttack?.Invoke(currentAttackGrade);
        if (currentAttackGrade != RhythmGrade.Miss)
            SyncPowerManager.KeepTheBeatGoing();
        else
            SyncPowerManager.Miss();

        player.OnAttackedPressed?.Invoke();
        PlayerAnimator.SetTrigger(CurrentAttack.AttackTag);
        RuntimeManager.PlayOneShotAttached(CurrentAttack.AttackSFX, player.gameObject);
        timer = 0;
        hitObjects.Clear();
        timeOfLastAttack = Time.time + CurrentAttack.Clip.length;

        if (DirectionInput.magnitude > 0)
            PlayerAnimator.transform.LookAt(PlayerAnimator.transform.position + Vector3.ProjectOnPlane(GetInputRelativeToCamera(), Vector3.up));
        AutoTargeting();
    }

    private void SetupVFX()
    {

        SwordSwingVFX.Stop();
        SwordSwingVFX.transform.localScale = Vector3.one * CurrentAttacks[AttackIndex].Radius * (CurrentAttacks[AttackIndex].AngleOffset < 0 ? -1f : 1f);
        SwordSwingVFX.transform.localRotation = (Quaternion.Euler(0, (CurrentAttacks[AttackIndex].AngleOffset < 0 ? 180 : 0),
            90));

        ParticleSystem.MainModule main = SwordSwingVFX.main;
        main.simulationSpeed = VFXSimulationSpeed;
        //Cached our sword mat, if anything start bugging out, replace swordswingmat with GetComponent
        swordSwingMat = CurrentAttacks[AttackIndex].VFXSwingMaterial;
        SwordSwingVFX.GetComponent<ParticleSystemRenderer>().material.SetFloat("_IntensityBlend", currentAttackGrade == RhythmGrade.Miss ? 0 : 1);
        Debug.Log("Grade is: " + swordSwingMat.GetFloat("_IntensityBlend"));
    }

    private void SetRelevantCombos(AttackStruct.InputButton input, int tryIndex = 0)
    {
        List<Combo> newCombos = new List<Combo>();


        foreach (Combo relevantCombo in RelevantCombos)
        {
            if (AttackIndex >= relevantCombo.Attacks.Length)
                continue;

            if (relevantCombo.Attacks[AttackIndex].Input == input)
            {
                newCombos.Add(relevantCombo);
                if (AttackIndex == relevantCombo.Attacks.Length - 1)
                    OnComboExecuted?.Invoke(relevantCombo.Name);
            }

        }



        if (newCombos.Count == 0 && tryIndex < 1)
        {
            AttackIndex = 0;
            RelevantCombos = Combos.ToList();
            SetRelevantCombos(input, tryIndex + 1);
            return;
        }

        RelevantCombos = newCombos;
    }

    private void CheckForMovementCollsion()
    {
        Vector3 movement = PlayerAnimator.transform.position - Position;
        Velocity = movement / Time.deltaTime;
        Velocity += Vector3.down * CurrentAttacks[AttackIndex].GravityFactor;
        PlayerAnimator.transform.position = Position;
        if (currentAttackGrade != RhythmGrade.Miss)
            DragThemAlong(movement);
    }

    private RaycastHit AttackMovementRaycast()
    {
        RaycastHit hit;
        Physics.CapsuleCast(Position + PlayerCollider.radius * Vector3.up,
            Position + Vector3.up * (PlayerCollider.height - PlayerCollider.radius), PlayerCollider.radius, Velocity.normalized, out hit, float.MaxValue, AttackLayer);
        if (hit.collider != null && hit.transform == player.transform)
            hit = new RaycastHit();
        return hit;
    }

    private void HighruleSwordCollision()
    {
        bool newhit = false;
        RaycastHit[] hits = Physics.SphereCastAll(Position, CurrentAttacks[AttackIndex].Radius, Vector3.up, 0.01f,
            AttackLayer);
        Vector3 toTarget = Vector3.zero;


        foreach (RaycastHit raycastHit in hits)
        {
            if (hitObjects.Contains(raycastHit.transform.root) || currentAttackGrade == RhythmGrade.Miss && MissFactor == 0 || raycastHit.transform.root.GetComponent<IDamageable>() == null)
                continue;
            toTarget = raycastHit.transform.position;
            toTarget.y = Position.y;
            toTarget -= Position;
            bool isClose = toTarget.magnitude <= CurrentAttack.CloseAttackRange;
            toTarget = toTarget.normalized;
            if (Vector3.Angle(toTarget, Quaternion.Euler(0, isClose ? CurrentAttack.CloseAttackAngleOffset : CurrentAttack.AngleOffset, 0) * PlayerAnimator.transform.forward) > (isClose ? CurrentAttack.CloseAttackAngle : CurrentAttacks[AttackIndex].Angle) * 0.5f) continue;
            hitObjects.Add(raycastHit.transform.root);
            player.OnAttackHitt?.Invoke(raycastHit.transform.root.GetComponent<IDamageable>());
            CurrentAttacks[AttackIndex].HitParticle.Play(raycastHit.collider.ClosestPoint(Position + PlayerCollider.height * Vector3.up), Quaternion.LookRotation((raycastHit.transform.root.position - Position).normalized));
            RuntimeManager.PlayOneShotAttached(CurrentAttacks[AttackIndex].HitSFX, player.gameObject);
            raycastHit.transform.root.GetComponent<IDamageable>()?.TakeDamage(CurrentAttacks[AttackIndex].Damage, currentAttackGrade, PlayerAnimator.transform, MissFactor);
            StreakCounterUI.AddHit(currentAttackGrade);
            newhit = true;
            Quaternion rot = Quaternion.LookRotation(player.transform.InverseTransformDirection(Vector3.left * CurrentAttacks[AttackIndex].AngleOffset), Vector3.up);
            bloodSplat.Play(raycastHit.collider.ClosestPoint(Position + PlayerCollider.height * Vector3.up), rot);
            AttackFreeze();
        }

        if (newhit)
        {
            //Can we do this only once per attack on enemy?
            SyncPowerManager.AddSyncPower(CurrentAttacks[AttackIndex].Damage);
            StreakCounterUI.AddMultiplier(currentAttackGrade);
        }


        swordLastPos = SwordTransform.position;
    }

    private void AttackFreeze()
    {
        if (Time.time - timeOfLastFreeze < FreezeCooldown) return;

        timeOfLastFreeze = Time.time;
        Time.timeScale = 0;
        freezTimer = FreezeTime;
        IsAttackFreeze = true;
    }

    private void DragThemAlong(Vector3 movement)
    {
        if (hitObjects == null)
            return;

        movement.y = 0;

        foreach (Transform transform in hitObjects)
        { //Try to optimize this somehow if we have time
            if(transform.GetComponent<AiController>().EnemyType == Enemy.basicAi)
                transform.position += movement;
        }
    }

    private void AutoTargeting()
    {
        RaycastHit[] hits = Physics.SphereCastAll(Position, SnapRadius, Vector3.up, 0.01f, AttackLayer);
        Vector3 toTarget = Vector3.zero;
        List<Vector3> directions = new List<Vector3>();
        foreach (RaycastHit raycastHit in hits)
        {
            if (raycastHit.transform.root.GetComponent<IDamageable>() == null)
                continue;
            toTarget = raycastHit.point;
            toTarget.y = Position.y;
            toTarget -= toTarget.normalized * PlayerCollider.radius;
            if (toTarget.magnitude <= IgnoreSnapDistance) return;

            toTarget = toTarget.normalized;
            Debug.DrawRay(Position, toTarget, Color.red, 1f);
            Debug.DrawRay(Position, GetInputRelativeToCamera(), Color.green, 1f);

            if (Vector3.Angle(toTarget, GetInputRelativeToCamera()) < IgnoreSnapDistance * 0.5f)
                directions.Add(toTarget);
        }

        if (directions.Count == 0)
            return;

        directions.OrderBy(direction => direction.magnitude);
        directions[0] = new Vector3(directions[0].x, 0, directions[0].z);
        Velocity += directions[0] / Time.deltaTime;
    }

    public override void Exit()
    {
        hasGivenInput = false;
        Time.timeScale = 1;
        SwordSwingVFX.Stop();
        IsAttackFreeze = false;
        PlayerAnimator.transform.localPosition = Vector3.zero;
        if (RelevantCombos.Count == 0 || CurrentAttacks[AttackIndex].RootMotion) Velocity = Vector3.zero;
        PlayerAnimator.SetTrigger("ToIdle");
    }



}