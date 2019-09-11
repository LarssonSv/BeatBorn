using UnityEngine;

[CreateAssetMenu(menuName = "States/Camera/Cinematic")]
public class CameraCinematic : CameraDefault {
    public override void UpdateState(Vector3 target, float xDegree, bool isMoving) {
        base.UpdateState(target, xDegree, isMoving);
        ZoomBasedOnTargetSpeed();
        FollowTarget();
    }

    protected override void FollowTarget() {
        currentDistance = target.x - cameraTransform.position.x;
        desiredDistance = currentDistance;
        correctedDistance = currentDistance;
    }
}