using RagdollEngine;
using UnityEngine;

public class DropDashPlayerBehaviour : PlayerBehaviour
{
    [SerializeField] RollPlayerBehaviour rollPlayerBehaviour;

    [SerializeField] float minSpeed;

    [SerializeField] float maxSpeed;

    [SerializeField] float chargeTime;

    float chargeTimer;

    void FixedUpdate()
    {
        if (!rollPlayerBehaviour.active || (groundInformation.ground && !groundInformation.enter))
        {
            chargeTimer = chargeTime;

            return;
        }

        if (!groundInformation.ground && chargeTimer > 0)
            chargeTimer -= Time.fixedDeltaTime;
    }

    public override void Execute()
    {
        if (groundInformation.enter)
            additiveVelocity = -RB.velocity
                + (Vector3.ProjectOnPlane(modelTransform.forward, groundInformation.hit.normal).normalized * Mathf.Max(RB.velocity.magnitude, Mathf.Lerp(minSpeed, maxSpeed, 1 - Mathf.Clamp01(chargeTimer / chargeTime))));
    }
}
