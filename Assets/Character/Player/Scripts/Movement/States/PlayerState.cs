using System.Linq;
using StateKraft;
using UnityEngine;

public class PlayerState : State
{
    protected PlayerEngine player;

    protected Vector3 Velocity { get => player.Velocity; set => player.Velocity = value; }
    protected Vector3 Position { get => player.transform.position; set => player.transform.position = value;}
    protected Vector2 DirectionInput => player.DirectionInput.magnitude > 1 ? player.DirectionInput.normalized : player.DirectionInput;
    protected Animator PlayerAnimator => player.CharacterAnimator;
    protected Transform SwordTransform => player.SwordTransform;
    protected CapsuleCollider PlayerCollider => player.Collider;
    protected Vector3 Forward => PlayerAnimator.transform.forward;
    protected ParticleSystem SwordSwingVFX => player.SwordSwingVFX;
    protected EmissionBurster SwordBurstVFX => player.SwordBurstVFX;

    public Vector3 GetInputRelativeToCamera()
    {
        Vector3 cameraDirection = new Vector3(player.DirectionInput.x, 0, DirectionInput.y);
        float mag = cameraDirection.magnitude;
        cameraDirection = player.CameraController.transform.TransformDirection(cameraDirection);
        cameraDirection.y = 0;
        cameraDirection = cameraDirection.normalized * mag;

        return cameraDirection;
    }
    
    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        player = (PlayerEngine) owner;
    }

    protected bool GroundCheck(float dist = 0.05f)
    {
        Vector3 start = Position + Vector3.up * PlayerCollider.radius;
        Vector3 direction = Vector3.down; //* Mathf.Clamp( dist * Vector3.Dot(Vector3.down, Velocity), dist, 1000f);
        RaycastHit[] hits = Physics.SphereCastAll(start, PlayerCollider.radius, direction,  direction.magnitude,
             ~LayerMask.GetMask("Player"));
        return hits.Length > 0;

    }
}
