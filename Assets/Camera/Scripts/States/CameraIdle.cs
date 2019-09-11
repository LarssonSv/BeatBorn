using UnityEngine;

[CreateAssetMenu(menuName = "States/Camera/Idle")]
public class CameraIdle : CameraDefault {
    public override void UpdateState(Vector3 target, float xDegree, bool isMoving) {
        base.UpdateState(target, xDegree, isMoving);
        ZoomBasedOnTargetSpeed();
        FollowTarget();
    }
}