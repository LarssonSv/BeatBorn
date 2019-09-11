using UnityEngine;

[CreateAssetMenu(menuName = "States/Camera/Combat")]
public class CameraCombat : CameraDefault {
    [Tooltip("Time before combat camera transitions back")]
    public float combatDuration = 1f;

    [Header("Soft zone")]
    [Tooltip("Y axis represents Z axis")] 
    [SerializeField] private Vector2 softZone = new Vector2(2, 2);

    public override void Init(Transform targetTransform, LayerMask layer, Transform cameraTransform) {
        base.Init(targetTransform, layer, cameraTransform);
        CombatDuration = combatDuration;
        SoftZone = softZone;
    }

    public override void UpdateState(Vector3 target, float xDegree, bool isMoving) {
        base.UpdateState(target, xDegree, isMoving);

        float x = targetTruePosition.position.x - target.x;
        float z = targetTruePosition.position.z - target.z;

        if(Mathf.Abs(x) > softZone.x) {
            target.x = x > 0 ? targetTruePosition.position.x - softZone.x : targetTruePosition.position.x + softZone.x;
        }
        if(Mathf.Abs(z) > softZone.y) {
            target.z = z > 0 ? targetTruePosition.position.z - softZone.y : targetTruePosition.position.z + softZone.y;
        }
        ZoomBasedOnTargetSpeed();
        FollowTarget();
    }

    public override void ApplyChanges() {
        base.ApplyChanges();
        CombatDuration = combatDuration;
        SoftZone = softZone;

        FindObjectOfType<CameraController>().UpdateValues();
    }
}