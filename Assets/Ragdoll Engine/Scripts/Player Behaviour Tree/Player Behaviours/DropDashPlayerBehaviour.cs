using RagdollEngine;
using UnityEngine;

public class DropDashPlayerBehaviour : PlayerBehaviour
{
    [SerializeField] float speed;

    public override void Execute()
    {
        if (groundInformation.enter)
            additiveVelocity = -RB.velocity
                + (Vector3.ProjectOnPlane(modelTransform.forward, groundInformation.hit.normal).normalized * Mathf.Max(RB.velocity.magnitude, speed));
    }
}
