using UnityEngine;

public class CameraDefault : ScriptableObject {
    protected Vector3 target;
    protected Transform targetTruePosition;
    protected Transform cameraTransform;

    [Header("Offset")]
    [SerializeField] protected float yPositionOffset = 1.7f;
    [SerializeField] protected float xAngleOffset = 6.9f;

    [Header("Zoom On moving/idling")]
    [SerializeField] protected float zoomSpeed = 40;
    [SerializeField] protected float zoomDamping = 1f;
    [MinMax(5, 20, ShowEditRange = true)]
    [SerializeField] protected Vector2 zoomDistance = new Vector2(9.7f, 10.46f);

    [Header("Collision")]
    [Tooltip("Closest distance camera can ever have to the target")]
    [SerializeField] protected float minDistanceToPreserve = 3f;
    [SerializeField] protected float offsetFromWalls = 0.1f;
    protected LayerMask collisionLayer = 1;

    protected float xDeg = 0f;
    protected bool isTargetMoving = true;
    public float CombatDuration { get; protected set; }
    public Vector2 SoftZone { get; protected set; }

    [Header("Transition")]
    [SerializeField] protected float transitionSpeed = 15f;
    [HideInInspector] public float currentDistance;
    [HideInInspector] public float desiredDistance;
    [HideInInspector] public float correctedDistance;
    [HideInInspector] public float tempYPositionOffset;
    [HideInInspector] public float tempXAngleOffset;
    [HideInInspector] public float tempZoomDistancecIdle;
    [HideInInspector] public float tempZoomDistanceMoving;
    protected bool transitioning = false;
    protected float transitionStep;
    protected float transitionTimeHolder = 0;

    public virtual void Init(Transform targetTransform, LayerMask layer, Transform cameraTransform) {
        targetTruePosition = targetTransform;
        collisionLayer = layer;
        this.cameraTransform = cameraTransform;
        float t = 1 / Time.deltaTime;
        transitionStep = transitionSpeed / t;

        tempYPositionOffset = yPositionOffset;
        tempXAngleOffset = xAngleOffset;
        tempZoomDistancecIdle = zoomDistance.x;
        tempZoomDistanceMoving = zoomDistance.y;
    }


    public virtual void UpdateState(Vector3 target, float xDegree, bool isMoving) {

        if (PauseGame.IsPaused())
            return;

        this.target = target;
        xDeg = xDegree;
        isTargetMoving = isMoving;

        if(!transitioning) {
            return;
        }

        if(transitionTimeHolder >= 1) {
            transitioning = false;
            return;
        }

        tempYPositionOffset = Mathf.SmoothStep(tempYPositionOffset, yPositionOffset, transitionTimeHolder);
        tempXAngleOffset = Mathf.LerpAngle(tempXAngleOffset, xAngleOffset, transitionTimeHolder);
        tempZoomDistancecIdle = Mathf.SmoothStep(tempZoomDistancecIdle, zoomDistance.x, transitionTimeHolder);
        tempZoomDistanceMoving = Mathf.SmoothStep(tempZoomDistanceMoving, zoomDistance.y, transitionTimeHolder);

        transitionTimeHolder += Time.deltaTime * transitionStep;
    }

    public virtual void Transition(CameraDefault from, float currentDistance, float desiredDistance, float correctedDistance) {
        transitioning = true;
        transitionTimeHolder = 0;

        this.currentDistance = currentDistance;
        this.desiredDistance = desiredDistance;
        this.correctedDistance = correctedDistance;

        tempYPositionOffset = from.tempYPositionOffset;
        tempXAngleOffset = from.tempXAngleOffset;
        tempZoomDistancecIdle = from.tempZoomDistancecIdle;
        tempZoomDistanceMoving = from.tempZoomDistanceMoving;
    }

    protected virtual void FollowTarget() {
        Quaternion rotation = Quaternion.Euler(tempXAngleOffset, xDeg, 0);

        Vector3 vTargetOffset = new Vector3(0, -tempYPositionOffset, 0);
        Vector3 position = target - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

        currentDistance = !WallCollided(position) || correctedDistance > currentDistance ?
            Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDamping) : correctedDistance;

        currentDistance = Mathf.Clamp(currentDistance, minDistanceToPreserve, tempZoomDistanceMoving);

        position = target - (rotation * Vector3.forward * currentDistance + vTargetOffset);

        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }

    protected bool WallCollided(Vector3 position) {
        Vector3 offsetedTargetPosition = new Vector3(target.x, target.y + yPositionOffset, target.z);
        if(Physics.Linecast(offsetedTargetPosition, position, out RaycastHit hit, collisionLayer)) {
            correctedDistance = Vector3.Distance(offsetedTargetPosition, hit.point) - offsetFromWalls;
            return true;
        }
        return false;
    }

    protected void ZoomBasedOnTargetSpeed() {
        if(isTargetMoving) {
            desiredDistance -= Time.deltaTime * zoomSpeed * Mathf.Abs(desiredDistance);
        } else {
            desiredDistance += Time.deltaTime * zoomSpeed * Mathf.Abs(desiredDistance);
        }

        desiredDistance = Mathf.Clamp(desiredDistance, tempZoomDistancecIdle, tempZoomDistanceMoving);
        correctedDistance = desiredDistance;
    }

    public virtual void ApplyChanges() {
        tempYPositionOffset = yPositionOffset;
        tempXAngleOffset = xAngleOffset;
        tempZoomDistancecIdle = zoomDistance.x;
        tempZoomDistanceMoving = zoomDistance.y;

        float t = 1 / Time.deltaTime;
        transitionStep = transitionSpeed / t;
    }
}